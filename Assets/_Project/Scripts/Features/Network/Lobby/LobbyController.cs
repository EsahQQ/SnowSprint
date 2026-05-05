using System;
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
            // Подписываемся на ГЛОБАЛЬНОЕ событие из менеджера
            GameNetworkManager.OnLobbyStatusReceived += OnStatusReceived;

            ShowLobby(); 
        
            // Устанавливаем сохраненное имя комнаты
            _lobbyView.SetRoomCode(LobbyBrowserController.LastJoinedRoomName);
        }


        public void Dispose()
        {
            // Отписываемся
            GameNetworkManager.OnLobbyStatusReceived -= OnStatusReceived;
        }

        public void ShowLobby()
        {
            if (_lobbyView is MonoBehaviour mb) mb.gameObject.SetActive(true);
            RunAsync().Forget(); 
        }

        public void HideLobby()
        {
            if (_lobbyView is MonoBehaviour mb) mb.gameObject.SetActive(false);
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
            // Здесь UI обновится данными конкретно твоей комнаты
            _lobbyView.UpdateReadyCount(msg.ReadyCount, msg.TotalCount);
        }
    }
}