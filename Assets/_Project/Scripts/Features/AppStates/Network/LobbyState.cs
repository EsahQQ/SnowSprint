using _Project.Scripts.Features.Network;
using _Project.Scripts.Features.SceneConstants;
using _Project.Scripts.Features.UI.Lobby;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;
using Unity.Netcode;

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
           
            UpdateUI(_lobbyNetworkManager.PlayersReadyCount.Value, _lobbyNetworkManager.TotalPlayersCount.Value);

            await _lobbyView.ProcessLobbyAsync();
            
            if (_lobbyNetworkManager.IsSpawned)
                _lobbyNetworkManager.SetPlayerReadyServerRpc();
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
            if (NetworkManager.Singleton.IsServer)
                StateMachine.RequestSwitchState<LoadSceneState, string>(SceneNames.Game);
        }
    }
}