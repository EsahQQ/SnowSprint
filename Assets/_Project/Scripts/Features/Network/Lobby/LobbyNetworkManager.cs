using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace _Project.Scripts.Features.Network.Lobby
{
    public class LobbyNetworkManager : MonoBehaviour
    {
        public static LobbyNetworkManager Singleton { get; private set; }

        private readonly HashSet<int> _readyClients = new();
        private int _totalPlayers;
        private int _readyCount;

        private void Awake() => Singleton = this;

        private void OnDestroy()
        {
            if (Singleton == this) Singleton = null;
        }

        [ServerCallback]
        public void ServerInitialize()
        {
            NetworkServer.RegisterHandler<LobbyReadyMessage>(OnClientReady);
            Debug.Log("[LNM] Сервер лобби инициализирован");
        }

        [ServerCallback]
        private void Update()
        {
            // ✅ Считаем только аутентифицированных
            int authenticated = GetAuthenticatedCount();
            if (authenticated != _totalPlayers)
            {
                _totalPlayers = authenticated;
                BroadcastStatus();
            }
        }

        [ServerCallback]
        private void OnClientReady(NetworkConnectionToClient conn, LobbyReadyMessage msg)
        {
            if (_readyClients.Add(conn.connectionId))
            {
                _readyCount = _readyClients.Count;
                BroadcastStatus();
                CheckAllReady();
            }
        }

        [ServerCallback]
        private void BroadcastStatus()
        {
            var msg = new LobbyStatusMessage
            {
                ReadyCount = _readyCount,
                TotalCount = _totalPlayers
            };

            // ✅ Отправляем только аутентифицированным
            foreach (var conn in NetworkServer.connections.Values)
            {
                if (conn.isAuthenticated)
                    conn.Send(msg);
            }

            Debug.Log($"[LNM] Broadcast: {_readyCount}/{_totalPlayers}");
        }

        [ServerCallback]
        private void CheckAllReady()
        {
            if (_totalPlayers > 0 && _readyCount >= _totalPlayers)
            {
                Debug.Log("[LNM] Все готовы!");
                foreach (var conn in NetworkServer.connections.Values)
                    if (conn.isAuthenticated)
                        conn.Send(new LobbyStartGameMessage());
            }
        }

        // ✅ Новый метод — вызвать после аутентификации клиента
        [ServerCallback]
        public void OnPlayerAuthenticated(NetworkConnectionToClient conn)
        {
            _totalPlayers = GetAuthenticatedCount();
            // Отправляем текущий статус только что вошедшему игроку
            conn.Send(new LobbyStatusMessage
            {
                ReadyCount = _readyCount,
                TotalCount = _totalPlayers
            });
            BroadcastStatus();
        }

        private int GetAuthenticatedCount()
        {
            int count = 0;
            foreach (var conn in NetworkServer.connections.Values)
                if (conn.isAuthenticated) count++;
            return count;
        }
    }
}
