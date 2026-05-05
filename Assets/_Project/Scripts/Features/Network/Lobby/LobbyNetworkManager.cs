using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Features.Gameplay.Level;
using _Project.Scripts.Features.Network.Gameplay;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Features.Utils;
using Mirror;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Network.Lobby
{
    public class LobbyNetworkManager : NetworkBehaviour
    {
        public static LobbyNetworkManager Singleton { get; private set; }
        
        [SerializeField] private LevelInstance _levelPrefab;
        [SerializeField] private int _playersPerMatch = 2;
        
        [Inject] private DiContainer _container;
        
        private int _activeMatchesCount = 0;
        private readonly List<MatchRoom> _rooms = new List<MatchRoom>();
        private readonly Dictionary<int, MatchRoom> _playerToRoomMap = new Dictionary<int, MatchRoom>();

        private void Awake()
        {
            if (Singleton != null && Singleton != this) { Destroy(gameObject); return; }
            Singleton = this;
            // Объект уже глобальный благодаря ProjectInstaller, 
            // но на всякий случай можно оставить DontDestroyOnLoad если он не в контексте
        }

        // Mirror вызывает это автоматически при запуске сервера
        public void RegisterServerHandlers()
        {
            NetworkServer.RegisterHandler<CreateRoomRequest>(OnCreateRoomRequest);
            NetworkServer.RegisterHandler<GetRoomListRequest>(OnGetRoomListRequest);
            NetworkServer.RegisterHandler<JoinRoomRequest>(OnJoinRoomRequest);
            NetworkServer.RegisterHandler<LobbyReadyMessage>(OnClientReady);
            NetworkServer.RegisterHandler<ReturnToLobbyMessage>(OnClientReturnToLobby);
        
            NetworkServer.OnDisconnectedEvent += OnPlayerDisconnected;

            Debug.Log("[LNM] Серверные обработчики комнат зарегистрированы вручную.");
        }
        
        private void OnDestroy() { if (Singleton == this) Singleton = null; }
        
        private void OnCreateRoomRequest(NetworkConnectionToClient conn, CreateRoomRequest msg)
        {
            // Создаем новую комнату
            var room = new MatchRoom(_playersPerMatch);
            _rooms.Add(room);

            // Сразу добавляем создателя в нее
            JoinPlayerToRoom(conn, room);
        }

// Когда клиент просит список комнат
        private void OnGetRoomListRequest(NetworkConnectionToClient conn, GetRoomListRequest msg)
        {
            var roomInfos = _rooms.Select(r => new RoomInfo
            {
                RoomId = r.RoomId,
                RoomName = r.RoomName,
                PlayerCount = r.Players.Count,
                MaxPlayers = _playersPerMatch
            }).ToArray();

            conn.Send(new RoomListResponse { Rooms = roomInfos });
        }

// Когда клиент просит войти в конкретную комнату
        private void OnJoinRoomRequest(NetworkConnectionToClient conn, JoinRoomRequest msg)
        {
            var room = _rooms.FirstOrDefault(r => r.RoomId == msg.RoomId);

            if (room == null)
            {
                conn.Send(new JoinRoomFailure { Reason = "Комната не найдена." });
                return;
            }

            if (room.IsFull)
            {
                conn.Send(new JoinRoomFailure { Reason = "Комната заполнена." });
                return;
            }
    
            JoinPlayerToRoom(conn, room);
        }

// Вспомогательный метод, чтобы не дублировать код
        private async void JoinPlayerToRoom(NetworkConnectionToClient conn, MatchRoom room)
        {
            room.AddPlayer(conn);
            _playerToRoomMap[conn.connectionId] = room;
            conn.Send(new JoinRoomSuccess { RoomName = room.RoomName });

            // Ждем немного, чтобы клиент успел загрузить OnlineScene и проинициализировать LobbyController
            await System.Threading.Tasks.Task.Delay(500);
            
            if (conn != null) SendRoomStatusUpdate(room);
        }

        [ServerCallback]
        public void OnPlayerAuthenticated(NetworkConnectionToClient conn)
        {
            // ОСТАВЬ ПУСТЫМ или просто логируй
            Debug.Log($"[LNM] Игрок {conn.connectionId} вошел в систему, ожидает выбора действия.");
    
            // МЫ НЕ ВЫЗЫВАЕМ ТУТ JoinPlayerToRoom!
            // Игрок сам пришлет запрос CreateRoomRequest или JoinRoomRequest позже.
        }

        [ServerCallback]
        private void OnClientReady(NetworkConnectionToClient conn, LobbyReadyMessage msg)
        {
            if (_playerToRoomMap.TryGetValue(conn.connectionId, out var room))
            {
                room.SetPlayerReady(conn);
                SendRoomStatusUpdate(room);

                // Если комната полная И все готовы -> старт
                if (room.IsFull && room.AreAllPlayersReady())
                {
                    StartMatchForRoom(room);
                }
            }
        }
        
        private void SendRoomStatusUpdate(MatchRoom room)
        {
            var msg = new LobbyStatusMessage
            {
                ReadyCount = room.ReadyCount,
                TotalCount = _playersPerMatch // Теперь всегда показываем размер комнаты
            };

            foreach (var playerConn in room.Players)
            {
                playerConn.Send(msg);
            }
        }

        private void StartMatchForRoom(MatchRoom room)
        {
            Debug.Log($"[LNM] Старт матча для комнаты {room.RoomId}");
            room.ResetReadyState(); 
            
            _rooms.Remove(room);
            foreach (var player in room.Players)
                _playerToRoomMap.Remove(player.connectionId);

            string matchId = room.RoomId.ToString();
            Vector3 offset = new Vector3(0, _activeMatchesCount * 1000, 0);
            
            // Спавним уровень
            LevelInstance level = Instantiate(_levelPrefab, offset, Quaternion.identity);
            level.Setup(matchId, room.Players);
            NetworkServer.Spawn(level.gameObject);
            _activeMatchesCount++;

            foreach(var conn in room.Players)
            {
                // --- РЕШЕНИЕ ПРОБЛЕМЫ IDENTITY ---
                if (conn.identity == null)
                {
                    Debug.LogWarning($"[LNM] У игрока {conn.connectionId} нет тела. Спавним принудительно...");
                    
                    // Используем Zenject для спавна префаба, чтобы сработали [Inject] внутри PlayerController
                    GameObject playerObj = _container.InstantiatePrefab(NetworkManager.singleton.playerPrefab);
                    
                    // Привязываем объект к соединению в Mirror
                    NetworkServer.AddPlayerForConnection(conn, playerObj);
                }

                // Теперь identity точно не null
                if (conn.identity.TryGetComponent<NetworkMatch>(out var playerMatch))
                {
                    playerMatch.matchId = matchId.ToGuid();
                }

                var playerController = conn.identity.GetComponent<PlayerController>();
                playerController.transform.position = level.SpawnPoint.position;
                playerController.SetActive(true);
            }
            
            _rooms.Remove(room);
        }
        
        [ServerCallback]
        private void OnPlayerDisconnected(NetworkConnectionToClient conn)
        {
            if (_playerToRoomMap.TryGetValue(conn.connectionId, out var room))
            {
                Debug.Log($"[LNM] Игрок {conn.connectionId} отключился от комнаты {room.RoomId}");
                _playerToRoomMap.Remove(conn.connectionId);
                room.RemovePlayer(conn);
                
                // Если комната опустела, удаляем ее
                if (room.Players.Count == 0)
                {
                    _rooms.Remove(room);
                }
                else
                {
                    // Иначе обновляем статус для оставшихся
                    SendRoomStatusUpdate(room);
                }
            }
        }

        [ServerCallback]
        private void OnClientReturnToLobby(NetworkConnectionToClient conn, ReturnToLobbyMessage msg)
        {
            Debug.Log($"[LNM] Игрок {conn.connectionId} вернулся из матча и переходит в меню.");

            if (conn.identity != null)
            {
                // Сбрасываем ID матча, чтобы игрок снова был "глобальным"
                var playerMatch = conn.identity.GetComponent<NetworkMatch>();
                playerMatch.matchId = System.Guid.Empty; 

                // Выключаем гоночную логику
                var player = conn.identity.GetComponent<PlayerController>();
                player.SetActive(false); 
                
                // Телепортируем в "буферную зону" или оставляем как есть, 
                // так как клиент сейчас всё равно сменит сцену
                player.transform.position = Vector3.zero;
            }
            
            // Мы больше не вызываем SendRoomStatusUpdate здесь, 
            // так как игрок больше не принадлежит ни к какой комнате.
        }
    }
}