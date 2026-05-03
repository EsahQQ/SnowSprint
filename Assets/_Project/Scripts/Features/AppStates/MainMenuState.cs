using _Project.Scripts.Features.Network.Server.Auth.Controller;
using _Project.Scripts.Features.SceneConstants;
using _Project.Scripts.Features.UI.Menu;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Features.AppStates
{
    public class MainMenuState : BaseState
    {
        private readonly IClientAuthController _clientAuthController;

        public MainMenuState(IStateMachine stateMachine, IClientAuthController clientAuthController) : base(stateMachine)
        {
            _clientAuthController = clientAuthController;
        }

        public override UniTask OnEnter()
        {
            _clientAuthController.RequestLobby += ToLobby;
            return UniTask.CompletedTask;
        }

        private void ToLobby()
        {
            StateMachine.RequestSwitchState<LoadSceneState, string>(SceneNames.LobbyMenu);
        }

        public override UniTask OnExit()
        {
            return UniTask.CompletedTask;
        }
    }
}