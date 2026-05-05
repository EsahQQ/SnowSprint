using System;
using _Project.Scripts.Features.UI.Menu;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
// Укажи правильный namespace

namespace _Project.Scripts.Features.Network.Lobby
{
    public class LobbyBrowserController : IInitializable, IDisposable
    {
        private readonly MainMenuView _mainMenuView;
        private readonly IInstantiator _instantiator;
        
        public static string LastJoinedRoomName;

        public LobbyBrowserController(MainMenuView mainMenuView)
        {
            _mainMenuView = mainMenuView;
        
            // Подписываемся на события от UI
            _mainMenuView.OnCreateRoomClicked += HandleCreateRoom;
            _mainMenuView.OnJoinLobbyClicked += HandleShowRoomList;
            _mainMenuView.OnRefreshRoomsClicked += HandleRefreshRooms;
            _mainMenuView.OnJoinRoomRequested += HandleJoinRoomRequest;
        }

        public void Initialize()
        {
            // Подписываемся на статические события менеджера
            GameNetworkManager.OnRoomListReceived += OnRoomListReceived;
            GameNetworkManager.OnJoinSuccess += OnJoinSuccess;
            GameNetworkManager.OnJoinFailure += OnJoinFailure;
            
            Debug.Log("[LobbyBrowser] Подписан на события сетевого менеджера.");
        }

        public void Dispose()
        {
            // ОБЯЗАТЕЛЬНО отписываемся при уничтожении сцены
            GameNetworkManager.OnRoomListReceived -= OnRoomListReceived;
            GameNetworkManager.OnJoinSuccess -= OnJoinSuccess;
            GameNetworkManager.OnJoinFailure -= OnJoinFailure;

            _mainMenuView.OnCreateRoomClicked -= HandleCreateRoom;
            _mainMenuView.OnJoinLobbyClicked -= HandleShowRoomList;
            _mainMenuView.OnRefreshRoomsClicked -= HandleRefreshRooms;
            _mainMenuView.OnJoinRoomRequested -= HandleJoinRoomRequest;
        }
    
        // --- Обработчики событий от UI ---

        private void HandleCreateRoom()
        {
            NetworkClient.Send(new CreateRoomRequest());
        }

        private void HandleShowRoomList()
        {
            _mainMenuView.ShowRoomListPanel(true);
            NetworkClient.Send(new GetRoomListRequest());
        }
    
        private void HandleRefreshRooms()
        {
            NetworkClient.Send(new GetRoomListRequest());
        }
    
        private void HandleJoinRoomRequest(System.Guid roomId)
        {
            NetworkClient.Send(new JoinRoomRequest { RoomId = roomId });
        }

        // --- Обработчики ответов от Сервера ---
    
        public void OnRoomListReceived(RoomListResponse msg)
        {
            _mainMenuView.DisplayRooms(msg.Rooms);
        }

        public void OnJoinSuccess(JoinRoomSuccess msg)
        {
            Debug.Log($"[LobbyBrowser] Успешный вход в: {msg.RoomName}");
        
            LastJoinedRoomName = msg.RoomName; // Сохраняем имя перед загрузкой!
        
            SceneManager.LoadScene("OnlineScene");
        }

        public void OnJoinFailure(JoinRoomFailure msg)
        {
            Debug.LogError($"Не удалось войти в комнату: {msg.Reason}");
            // Здесь можно показать всплывающее окно с ошибкой
        }
    }
}