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
        private LobbyNetworkManager _lobby;

        public LobbyController(ILobbyView lobbyView)
        {
            _lobbyView = lobbyView;
        }

        public void Initialize()
        {
            RunAsync().Forget();
        }

        public void Dispose()
        {
            if (_lobby == null) return;
            _lobby.OnAllPlayersReady -= OnAllPlayersReady;
            _lobby.OnReadyStatsChanged -= OnReadyStatsChanged;
            _lobby = null;
        }

        private async UniTaskVoid RunAsync()
        {
            Debug.Log("[LobbyController] Ожидаем LobbyNetworkManager...");

            await UniTask.WaitUntil(() => LobbyNetworkManager.Singleton != null);

            Debug.Log("[LobbyController] LobbyNetworkManager готов!");

            _lobby = LobbyNetworkManager.Singleton;
            _lobby.OnAllPlayersReady += OnAllPlayersReady;
            _lobby.OnReadyStatsChanged += OnReadyStatsChanged;

            // Актуализируем UI сразу после подписки
            OnReadyStatsChanged(_lobby.PlayersReadyCount, _lobby.TotalPlayersCount);

            // Ждём нажатия кнопки Ready
            await _lobbyView.ProcessLobbyAsync();

            _lobby.CmdSetPlayerReady();
        }

        private void OnReadyStatsChanged(int ready, int total)
        {
            Debug.Log($"[LobbyController] Готовы: {ready}/{total}");
            _lobbyView.UpdateReadyCount(ready, total);
        }

        private void OnAllPlayersReady()
        {
            if (!NetworkServer.active) return;

            Debug.Log("[LobbyController] Все готовы — запускаем игру");
            NetworkManager.singleton.ServerChangeScene(SceneNames.Game);
        }
    }
}
