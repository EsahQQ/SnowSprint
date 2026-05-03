using _Project.Scripts.Features.Network.Lobby;
using _Project.Scripts.Features.SceneConstants;
using _Project.Scripts.Features.UI.Lobby;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;

namespace _Project.Scripts.Features.AppStates.Network
{
    public class LobbyState : BaseState
    {
        private readonly LobbyNetworkManager _lobbyNetworkManager;
        private readonly ILobbyView _lobbyView;
        
        public LobbyState(IStateMachine stateMachine, LobbyNetworkManager lobbyNetworkManager, ILobbyView lobbyView) 
            : base(stateMachine)
        {
            _lobbyNetworkManager = lobbyNetworkManager;
            _lobbyView = lobbyView;
        }

        public override async UniTask OnEnter()
        {
            _lobbyNetworkManager.OnAllPlayersReady += StartGame;
            _lobbyNetworkManager.OnReadyStatsChanged += UpdateUI;
            await UniTask.WaitUntil(() => NetworkClient.active);
            
            if (!NetworkClient.ready)
                NetworkClient.Ready();
            
            UpdateUI(_lobbyNetworkManager.PlayersReadyCount, _lobbyNetworkManager.TotalPlayersCount);
            
            await _lobbyView.ProcessLobbyAsync();
            
            _lobbyNetworkManager.CmdSetPlayerReady(); 
        }

        public override UniTask OnExit()
        {
            _lobbyNetworkManager.OnAllPlayersReady -= StartGame;
            _lobbyNetworkManager.OnReadyStatsChanged -= UpdateUI;
            return UniTask.CompletedTask;
        }

        private void UpdateUI(int readyCount, int totalCount) => _lobbyView.UpdateReadyCount(readyCount, totalCount);

        private void StartGame()
        {
            if (NetworkServer.active)
                StateMachine.RequestSwitchState<LoadSceneState, string>(SceneNames.Game);
        }
    }
}