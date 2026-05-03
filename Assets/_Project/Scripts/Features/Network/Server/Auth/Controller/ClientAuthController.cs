using System;
using _Project.Scripts.Features.Network.Server.Auth.Data;
using _Project.Scripts.Features.Player.Services;
using _Project.Scripts.Features.UI.Menu;
using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Network.Server.Auth.Controller
{
    public class ClientAuthController : IInitializable, IDisposable, IClientAuthController
    {
        private readonly IMainMenuView _view;
        private readonly IPlayerDataService _playerDataService;

        private string _pendingEmail;
        private bool _isLoggedIn;
        private GameAuthenticator _authenticator;
        private AuthRequestMessage? _pendingMessage;

        public ClientAuthController(IMainMenuView view, IPlayerDataService playerDataService)
        {
            _view = view;
            _playerDataService = playerDataService;
        }

        public void Initialize()
        {
            _view.OnPlayClicked += HandlePlayClicked;
            _view.OnAuthActionSubmitted += HandleAuthAction;
            _view.OnLogoutClicked += HandleLogout;

            _authenticator = NetworkManager.singleton.authenticator as GameAuthenticator;
            if (_authenticator != null)
                _authenticator.OnAuthResponseReceived += OnAuthResponseReceived;
            else
                Debug.LogError("[Auth] GameAuthenticator не найден в NetworkManager!");
        }

        public void Dispose()
        {
            _view.OnPlayClicked -= HandlePlayClicked;
            _view.OnAuthActionSubmitted -= HandleAuthAction;
            _view.OnLogoutClicked -= HandleLogout;

            if (_authenticator != null)
                _authenticator.OnAuthResponseReceived -= OnAuthResponseReceived;

            NetworkClient.OnConnectedEvent -= OnConnectedFlushPending;
        }

        public void HandlePlayClicked()
        {
            if (_isLoggedIn) return; // уже в лобби через OnlineScene

            if (!NetworkClient.isConnected)
            {
                NetworkManager.singleton.StartClient();
                NetworkClient.OnConnectedEvent += OnConnectedFlushPending;
            }

            _view.ShowLoginPanel(true);
        }

        public void HandleLogout()
        {
            _isLoggedIn = false;
            _view.UpdateProfileUI(false, string.Empty);

            if (NetworkServer.active && NetworkClient.isConnected)
                NetworkManager.singleton.StopHost();
            else if (NetworkClient.isConnected)
                NetworkManager.singleton.StopClient();
        }

        public void HandleAuthAction(AuthData input)
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
                    SendOrBuffer(msg);
                    break;

                case AuthAction.TryRegister:
                    msg.Type = AuthRequestType.Register;
                    SendOrBuffer(msg);
                    break;

                case AuthAction.SubmitCode:
                    msg.Type = AuthRequestType.Verify;
                    SendOrBuffer(msg);
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
                    if (!NetworkServer.active)
                        NetworkManager.singleton.StopClient();
                    break;
            }
        }

        private void SendOrBuffer(AuthRequestMessage msg)
        {
            if (NetworkClient.isConnected)
                NetworkClient.Send(msg);
            else
            {
                Debug.Log("[Auth] Соединение ещё не установлено — буферизуем сообщение");
                _pendingMessage = msg;
            }
        }

        private void OnConnectedFlushPending()
        {
            NetworkClient.OnConnectedEvent -= OnConnectedFlushPending;

            if (_pendingMessage.HasValue)
            {
                Debug.Log("[Auth] Соединение установлено — отправляем буферизованное сообщение");
                NetworkClient.Send(_pendingMessage.Value);
                _pendingMessage = null;
            }
        }

        private void OnAuthResponseReceived(AuthResponseMessage msg)
        {
            HandleResponseAsync(msg).Forget();
        }

        private async UniTaskVoid HandleResponseAsync(AuthResponseMessage msg)
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
                        _view.ShowVerifyPanel(false);
                        _view.ShowLoginPanel(true);
                        Debug.Log("[Auth] Аккаунт активирован. Войдите в систему.");
                    }
                    else
                        Debug.LogError($"[Auth] Неверный код: {msg.Message}");
                    break;

                case AuthRequestType.Login:
                    if (msg.Success)
                    {
                        _isLoggedIn = true;
                        _view.UpdateProfileUI(true, msg.Message);
                        _view.ShowLoginPanel(false);

                        Debug.Log("[Auth] Загружаем профиль с сервера...");
                        await _playerDataService.LoadProfileFromCloudAsync();
                        Debug.Log($"[Auth] Профиль загружен. Монеты: {_playerDataService.Coins}");

                        // Переход в лобби Mirror делает сам через OnlineScene после ServerAccept
                    }
                    else
                        Debug.LogError($"[Auth] Ошибка входа: {msg.Message}");
                    break;
            }
        }
    }
}
