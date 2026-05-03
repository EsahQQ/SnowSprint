using System;
using _Project.Scripts.Features.AppStates;
using _Project.Scripts.Features.AppStates.Network;
using _Project.Scripts.Features.Network.Server.Auth.Data;
using _Project.Scripts.Features.SceneConstants;
using _Project.Scripts.Features.UI.Menu;
using _Project.Scripts.Infrastructure.StateMachine;
using Mirror;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Network.Server.Auth
{
    public class ClientAuthController : IInitializable, IDisposable
    {
        private readonly IMainMenuView _view;
        private readonly IStateMachine _stateMachine;
        private string _pendingEmail;
        private bool _isInitialized; 
        private bool _isLoggedIn;

        public ClientAuthController(IMainMenuView view, IStateMachine stateMachine)
        {
            _view = view;
            _stateMachine = stateMachine;
        }

        public void Initialize()
        {
            _view.OnPlayClicked += HandlePlayClicked;
            _view.OnAuthActionSubmitted += HandleAuthAction;
            _view.OnLogoutClicked += HandleLogout;
            
            if (!_isInitialized)
            {
                NetworkClient.RegisterHandler<AuthResponseMessage>(OnClientReceiveResponse, false);
                _isInitialized = true;
            }
        }

        public void Dispose()
        {
            _view.OnPlayClicked -= HandlePlayClicked;
            _view.OnAuthActionSubmitted -= HandleAuthAction;
            _view.OnLogoutClicked -= HandleLogout;
        }

        private void HandlePlayClicked()
        {
            if (_isLoggedIn)
            {
                _stateMachine.RequestSwitchState<LoadSceneState, string>(SceneNames.LobbyMenu);
                return;
            }

            if (!NetworkClient.isConnected)
            {
#if UNITY_EDITOR
                NetworkManager.singleton.StartHost();
#else
                NetworkManager.singleton.StartClient();
#endif
            }
            _view.ShowLoginPanel(true);
        }
        
        private void HandleLogout()
        {
            _isLoggedIn = false;
            _view.UpdateProfileUI(false, string.Empty);
            NetworkManager.singleton.StopHost();
        }

        private void HandleAuthAction(AuthData input)
        {
            if (!string.IsNullOrEmpty(input.Email))
                _pendingEmail = input.Email;

            var msg = new AuthRequestMessage
            {
                Email = _pendingEmail,
                Password = input.Password,
                Username = input.Username,
                Code = input.Code
            };

            switch (input.Action)
            {
                case AuthAction.TryLogin:
                    msg.Type = AuthRequestType.Login;
                    NetworkClient.Send(msg);
                    break;
                case AuthAction.TryRegister:
                    msg.Type = AuthRequestType.Register;
                    NetworkClient.Send(msg);
                    break;
                case AuthAction.SubmitCode:
                    msg.Type = AuthRequestType.Verify;
                    NetworkClient.Send(msg);
                    break;
                case AuthAction.GoToRegister:
                    _view.SwitchBetweenAuthPanels(AuthAction.GoToRegister);
                    break;
                case AuthAction.GoToLogin:
                    _view.SwitchBetweenAuthPanels(AuthAction.GoToLogin);
                    break;
                case AuthAction.Cancel:
                    _view.ShowLoginPanel(false);
                    _view.ShowRegisterPanel(false);
                    _view.ShowVerifyPanel(false);
                    break;
            }
        }

        private void OnClientReceiveResponse(AuthResponseMessage msg)
        {
            switch (msg.Type)
            {
                case AuthRequestType.Register:
                    if (msg.Success) 
                        _view.SwitchBetweenAuthPanels(AuthAction.TryRegister);
                    else 
                        Debug.LogError($"[Auth] Ошибка регистрации: {msg.Message}");
                    break;

                case AuthRequestType.Verify:
                    if (msg.Success)
                    {
                        Debug.Log("[Auth] Успешная активация! Теперь войдите в аккаунт.");
                        _view.ShowVerifyPanel(false);
                        _view.ShowLoginPanel(true); 
                    }
                    else 
                        Debug.LogError($"[Auth] Ошибка кода: {msg.Message}");
                    break;

                case AuthRequestType.Login:
                    if (msg.Success)
                    {
                        Debug.Log($"[Auth] Авторизован как: {msg.Message}");
                        _isLoggedIn = true; 
                        
                        _view.UpdateProfileUI(true, msg.Message);
                        _view.ShowLoginPanel(false);
         
                    }
                    else Debug.LogError($"[Auth] Ошибка: {msg.Message}");
                    break;
            }
        }
    }
}