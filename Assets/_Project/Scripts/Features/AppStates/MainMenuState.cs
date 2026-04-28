using _Project.Scripts.Features.Network;
using _Project.Scripts.Features.Network.Auth;
using _Project.Scripts.Features.SceneConstants;
using _Project.Scripts.Features.UI;
using _Project.Scripts.Features.Player.Services; 
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
        private readonly IAuthService _authService;

        private readonly IPlayerDataService _playerDataService;

        public MainMenuState(
            IStateMachine stateMachine, 
            IMainMenuView mainMenuView, 
            INetworkSessionService networkSession, 
            IAuthService authService,
            IPlayerDataService playerDataService) : base(stateMachine)
        {
            _mainMenuView = mainMenuView;
            _networkSession = networkSession;
            _authService = authService;
            _playerDataService = playerDataService;
        }
        
        public override async UniTask OnEnter()
        {
            Debug.Log("MainMenuState Enter");

            await _authService.InitializeAsync();
            _mainMenuView.OnLogoutClicked += HandleLogout;
            RefreshProfileUI();
            
            if (_authService.IsSignedIn) await _playerDataService.LoadProfileFromCloudAsync();

            while (true)
            {
                await _mainMenuView.ProcessMenuAsync(); 

                if (!_authService.IsSignedIn)
                {
                    bool authSuccess = await HandleAuthenticationFlowAsync();
                    if (!authSuccess) continue; 

                    await _playerDataService.LoadProfileFromCloudAsync();
                }

                break;
            }

            await _networkSession.QuickJoinOrCreateAsync();
            StateMachine.RequestSwitchState<LoadSceneState, string>(SceneNames.LobbyMenu);
        }

        public override UniTask OnExit()
        {
            _mainMenuView.OnLogoutClicked -= HandleLogout;
            return UniTask.CompletedTask;
        }

        private async UniTask<bool> HandleAuthenticationFlowAsync()
        {
            _mainMenuView.ShowAuthPanel(true);

            while (true)
            {
                var authData = await _mainMenuView.WaitForAuthInputAsync();

                if (authData.isCanceled)
                {
                    _mainMenuView.ShowAuthPanel(false);
                    return false; 
                }
                
                bool success = authData.isLogin
                    ? await _authService.SignInAsync(authData.username, authData.password)
                    : await _authService.SignUpAsync(authData.username, authData.password);

                if (success)
                {
                    _mainMenuView.ShowAuthPanel(false);
                    RefreshProfileUI();
                    return true;
                }

                Debug.LogWarning("[Auth] Неверный логин или пароль");
            }
        }

        private void HandleLogout()
        {
            _authService.SignOut();
            RefreshProfileUI();
        }

        private void RefreshProfileUI() => _mainMenuView.UpdateProfileUI(_authService.IsSignedIn, _authService.PlayerName);
    }
}