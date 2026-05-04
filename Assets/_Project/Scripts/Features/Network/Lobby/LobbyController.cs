using System;
using _Project.Scripts.Features.SceneConstants;
using _Project.Scripts.Features.UI.Lobby;
using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Network.Lobby
{
    public class LobbyController : IInitializable, IDisposable
    {
        private readonly ILobbyView _lobbyView;

        public LobbyController(ILobbyView lobbyView)
        {
            _lobbyView = lobbyView;
        }

        public void Initialize()
        {
            NetworkClient.RegisterHandler<LobbyStatusMessage>(OnStatusReceived);

            if (NetworkServer.active)
            {
                var lnm = LobbyNetworkManager.Singleton;
                if (lnm != null)
                    lnm.ServerInitialize();
                else
                    Debug.LogError("[LobbyController] LobbyNetworkManager не найден в сцене!");
            }

            RunAsync().Forget();
        }

        public void Dispose()
        {
            NetworkClient.UnregisterHandler<LobbyStatusMessage>();
        }

        private async UniTaskVoid RunAsync()
        {
            Debug.Log("[LobbyController] Ждём нажатия Ready...");
            await _lobbyView.ProcessLobbyAsync();

            Debug.Log("[LobbyController] Отправляем Ready серверу");
            NetworkClient.Send(new LobbyReadyMessage());
        }

        private void OnStatusReceived(LobbyStatusMessage msg)
        {
            Debug.Log($"[LobbyController] Статус: {msg.ReadyCount}/{msg.TotalCount}");
            _lobbyView.UpdateReadyCount(msg.ReadyCount, msg.TotalCount);
        }
    }
}
