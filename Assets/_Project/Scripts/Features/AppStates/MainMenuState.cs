using _Project.Scripts.Features.Network;
using _Project.Scripts.Features.Network.Auth;
using _Project.Scripts.Features.Player.Services;
using _Project.Scripts.Features.SceneConstants;
using _Project.Scripts.Features.UI.Menu;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Features.AppStates
{
    public class MainMenuState : BaseState
    {
        private readonly IMainMenuView _view;
        private readonly INetworkSessionService _networkSession;
        private readonly IAuthService _authService;
        private readonly IPlayerDataService _playerDataService;
        private readonly AuthFlowController _authFlowController;

        public MainMenuState(IStateMachine stateMachine, IMainMenuView view, INetworkSessionService networkSession,
            IAuthService authService, IPlayerDataService playerDataService, AuthFlowController authFlowController) : base(stateMachine)
        {
            _view = view;
            _networkSession = networkSession;
            _authService = authService;
            _playerDataService = playerDataService;
            _authFlowController = authFlowController;
        }

        public override async UniTask OnEnter()
        {
            _view.OnLogoutClicked += HandleLogout;
            _view.OnPlayClicked += HandlePlayClicked;

            await _authService.InitializeAsync();
            RefreshProfileUI();

            if (_authService.IsLoggedInAsUser)
                await _playerDataService.LoadProfileFromCloudAsync();
        }

        public override UniTask OnExit()
        {
            _view.OnLogoutClicked -= HandleLogout;
            _view.OnPlayClicked -= HandlePlayClicked;
            return UniTask.CompletedTask;
        }

        private async void HandlePlayClicked()
        {
            if (_authService.IsLoggedInAsUser)
            {
                await _networkSession.QuickJoinOrCreateAsync();
                StateMachine.RequestSwitchState<LoadSceneState, string>(SceneNames.LobbyMenu);
                return;
            }

            bool authSuccess = await _authFlowController.RunAsync();
            
            if (authSuccess)
            {
                RefreshProfileUI();
                await _playerDataService.LoadProfileFromCloudAsync();
            }
        }

        private void HandleLogout()
        {
            _authService.SignOut();
            RefreshProfileUI();
        }

        private void RefreshProfileUI() =>
            _view.UpdateProfileUI(_authService.IsLoggedInAsUser, _authService.PlayerName);
    }
}