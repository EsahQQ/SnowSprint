using System; // Добавь это
using _Project.Scripts.Features.Network.Lobby;
using _Project.Scripts.Features.SceneConstants;
using Mirror;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Features.Network
{
    public class GameNetworkManager : NetworkManager
    {
        // --- СТАТИЧЕСКИЕ СОБЫТИЯ ---
        public static event Action<RoomListResponse> OnRoomListReceived;
        public static event Action<JoinRoomSuccess> OnJoinSuccess;
        public static event Action<JoinRoomFailure> OnJoinFailure;
        public static event Action<LobbyStatusMessage> OnLobbyStatusReceived;

        public override void OnStartServer()
        {
            base.OnStartServer();
            if (LobbyNetworkManager.Singleton != null)
                LobbyNetworkManager.Singleton.RegisterServerHandlers();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            NetworkClient.RegisterHandler<RoomListResponse>(msg => OnRoomListReceived?.Invoke(msg));
            NetworkClient.RegisterHandler<JoinRoomSuccess>(msg => OnJoinSuccess?.Invoke(msg));
            NetworkClient.RegisterHandler<JoinRoomFailure>(msg => OnJoinFailure?.Invoke(msg));
        
            // РЕГИСТРИРУЕМ СТАТУС ЛОББИ ГЛОБАЛЬНО
            NetworkClient.RegisterHandler<LobbyStatusMessage>(msg => OnLobbyStatusReceived?.Invoke(msg));

            UnityEngine.Debug.Log("[GNM] Все обработчики зарегистрированы.");
        }

        public override void OnClientSceneChanged()
        {
            base.OnClientSceneChanged();
            
            if (SceneManager.GetActiveScene().name == SceneNames.OnlineScene) 
            {
                // Вызываем AddPlayer только если его еще нет
                if (NetworkClient.isConnected && NetworkClient.localPlayer == null)
                {
                    NetworkClient.AddPlayer();
                }
            }
        }
    }
}