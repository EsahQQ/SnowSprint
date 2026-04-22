using _Project.Scripts.Features.Network;
using _Project.Scripts.Features.SceneConstants;
using _Project.Scripts.Features.UI.Lobby;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Features.AppStates.Network
{
    public class LobbyState : BaseState
    {
        private readonly INetworkSessionService _networkSession;
        private readonly ILobbyView _lobbyView;

        public LobbyState(IStateMachine stateMachine, INetworkSessionService networkSession, ILobbyView lobbyView) 
            : base(stateMachine)
        {
            _networkSession = networkSession;
            _lobbyView = lobbyView;
        }

        public override async UniTask OnEnter()
        {
            Debug.Log("LobbyState Enter");
            
            _networkSession.OnAllPlayersReady += StartGame;
            await _lobbyView.ProcessLobbyAsync();
            _networkSession.SetLocalPlayerReady(true);
        }

        public override UniTask OnExit()
        {
            _networkSession.OnAllPlayersReady -= StartGame;
            
            return UniTask.CompletedTask;
        }

        private void StartGame()
        {
            if (_networkSession.IsHost)
                StateMachine.RequestSwitchState<LoadSceneState, string>(SceneNames.Game);
        }
    }
}