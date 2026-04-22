using _Project.Scripts.Features.Network;
using _Project.Scripts.Features.SceneConstants;
using _Project.Scripts.Features.UI;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Features.AppStates
{
    public class MainMenuState : BaseState
    {
        private readonly IMainMenuView _mainMenuView;
        private readonly INetworkSessionService _networkSession;

        public MainMenuState(IStateMachine stateMachine, IMainMenuView mainMenuView, INetworkSessionService networkSession) : base(stateMachine)
        {
            _mainMenuView = mainMenuView;
            _networkSession = networkSession;
        }
        
        public override async UniTask OnEnter()
        {
            await _mainMenuView.ProcessMenuAsync();
            await _networkSession.QuickJoinOrCreateAsync();
            
            StateMachine.RequestSwitchState<LoadSceneState, string>(SceneNames.LobbyMenu);
        }
    }
}