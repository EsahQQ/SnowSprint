using System;
using _Project.Scripts.Features.Network.Lobby;
using _Project.Scripts.Features.Network.Server.Auth.Data;
using _Project.Scripts.Features.UI.Menu.Panels;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.UI.Menu
{
    public class MainMenuView : MonoBehaviour, IMainMenuView
    {
        [Header("Canvas Groups")]
        [SerializeField] private CanvasGroup _menuGroup;
        [SerializeField] private CanvasGroup _settingsGroup;
        [SerializeField] private CanvasGroup _loginGroup;
        [SerializeField] private CanvasGroup _registerGroup;
        [SerializeField] private CanvasGroup _verifyGroup;

        [Header("Lobby Browser Groups")]
        [SerializeField] private CanvasGroup _selectGroup; // Группа с выбором "Создать/Присоединиться"
        [SerializeField] private CanvasGroup _roomsGroup;  // Группа со списком комнат

        [Header("Lobby Browser Buttons")]
        [SerializeField] private Button _createRoomButton;
        [SerializeField] private Button _joinRoomButton;
        [SerializeField] private Button _backFromSelectButton;
        [SerializeField] private Button _refreshRoomsButton;
        [SerializeField] private Button _backFromRoomsButton;
        
        [Header("Room List")]
        [SerializeField] private Transform _roomListContent;   // Объект Content в ScrollView
        [SerializeField] private RoomListItemView _roomItemPrefab; // Префаб элемента списка (создадим ниже)
        
        [Header("Menu Buttons")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _backFromSettingsButton;

        [Header("Sub-Panels")]
        [SerializeField] private ProfilePanel _profile;
        [SerializeField] private AuthLoginPanel _login;
        [SerializeField] private AuthRegisterPanel _register;
        [SerializeField] private AuthVerifyPanel _verify;

        [Header("Settings")]
        [SerializeField] private float _fadeDuration = 0.4f;
        
        public event Action OnPlayClicked;
        public event Action OnLogoutClicked;
        public event Action<AuthData> OnAuthActionSubmitted;
        
        public event Action OnCreateRoomClicked;
        public event Action OnJoinLobbyClicked; // Клик на "Присоединиться"
        public event Action OnRefreshRoomsClicked;
        public event Action<System.Guid> OnJoinRoomRequested; // Клик на конкретную комнату

        private void Start()
        {
            BindMenuButtons();
            BindAuthButtons();
            BindLobbyBrowserButtons(); 
            ResetAllPanels();
        }
        
        public void ShowSelectActionPanel(bool show)
        {
            SwitchPanels(show ? _menuGroup : _selectGroup, show ? _selectGroup : _menuGroup);
        }

        public void ShowRoomListPanel(bool show)
        {
            SwitchPanels(show ? _selectGroup : _roomsGroup, show ? _roomsGroup : _selectGroup);
        }

        public void DisplayRooms(RoomInfo[] rooms)
        {
            // Очищаем старый список
            foreach (Transform child in _roomListContent)
            {
                Destroy(child.gameObject);
            }

            // Создаем новые элементы
            foreach (var roomInfo in rooms)
            {
                var item = Instantiate(_roomItemPrefab, _roomListContent);
                item.Setup(roomInfo, OnJoinRoomRequested);
            }
        }

        private void BindLobbyBrowserButtons()
        {
            _createRoomButton.onClick.AddListener(() => OnCreateRoomClicked?.Invoke());
            _joinRoomButton.onClick.AddListener(() => OnJoinLobbyClicked?.Invoke());
            _refreshRoomsButton.onClick.AddListener(() => OnRefreshRoomsClicked?.Invoke());

            _backFromSelectButton.onClick.AddListener(() => SwitchPanels(_selectGroup, _menuGroup));
            _backFromRoomsButton.onClick.AddListener(() => SwitchPanels(_roomsGroup, _selectGroup));
        }

        public void UpdateProfileUI(bool isLoggedIn, string playerName)
        {
            _profile.Root.SetActive(isLoggedIn);
            if (isLoggedIn)
                _profile.UsernameText.text = $"Привет, {playerName}!";
        }

        public void ShowLoginPanel(bool show) => SwitchPanels(show ? _menuGroup : _loginGroup, show ? _loginGroup : _menuGroup);
        public void ShowRegisterPanel(bool show) => SwitchPanels(show ? _menuGroup : _registerGroup, show ? _registerGroup : _menuGroup);

        public void ShowVerifyPanel(bool show)
        {
            if (show) _verify.CodeInput.text = string.Empty;
            SwitchPanels(show ? _menuGroup : _verifyGroup, show ? _verifyGroup : _menuGroup);
        }

        public void SwitchBetweenAuthPanels(AuthAction action)
        {
            switch (action)
            {
                case AuthAction.GoToRegister: SwitchPanels(_loginGroup, _registerGroup); break;
                case AuthAction.GoToLogin: SwitchPanels(_registerGroup, _loginGroup); break;
                case AuthAction.TryRegister:
                    _verify.CodeInput.text = string.Empty;
                    SwitchPanels(_registerGroup, _verifyGroup);
                    break;
            }
        }
        
        private void BindMenuButtons()
        {
            _playButton.onClick.AddListener(() => OnPlayClicked?.Invoke());
            _exitButton.onClick.AddListener(QuitGame);
            _settingsButton.onClick.AddListener(() => SwitchPanels(_menuGroup, _settingsGroup));
            _backFromSettingsButton.onClick.AddListener(() => SwitchPanels(_settingsGroup, _menuGroup));
            _profile.LogoutButton.onClick.AddListener(() => OnLogoutClicked?.Invoke());
        }

        private void BindAuthButtons()
        {
            _login.SubmitButton.onClick.AddListener(() => SubmitAuth(AuthAction.TryLogin));
            _login.GoToRegisterButton.onClick.AddListener(() => SubmitAuth(AuthAction.GoToRegister));
            _login.CloseButton.onClick.AddListener(() => SubmitAuth(AuthAction.Cancel));

            _register.SubmitButton.onClick.AddListener(() => SubmitAuth(AuthAction.TryRegister));
            _register.GoToLoginButton.onClick.AddListener(() => SubmitAuth(AuthAction.GoToLogin));
            _register.CloseButton.onClick.AddListener(() => SubmitAuth(AuthAction.Cancel));

            _verify.SubmitButton.onClick.AddListener(() => SubmitAuth(AuthAction.SubmitCode));
            _verify.CancelButton.onClick.AddListener(() => SubmitAuth(AuthAction.Cancel));
        }
        
        private void SubmitAuth(AuthAction action)
        {
            var data = new AuthData { Action = action };

            switch (action)
            {
                case AuthAction.TryLogin:
                    data.Username = _login.UsernameInput.text; 
                    data.Email = _login.UsernameInput.text; 
                    data.Password = _login.PasswordInput.text;
                    break;
            
                case AuthAction.TryRegister:
                    data.Username = _register.UsernameInput.text;
                    data.Email = _register.EmailInput.text;
                    data.Password = _register.PasswordInput.text;
                    break;
            
                case AuthAction.SubmitCode:
                    data.Code = _verify.CodeInput.text;
                    break;
            }

            OnAuthActionSubmitted?.Invoke(data);
        }

        private void SwitchPanels(CanvasGroup from, CanvasGroup to)
        {
            from.DOKill();
            to.DOKill();

            from.interactable = false;
            from.blocksRaycasts = false;
            to.interactable = false;
            to.blocksRaycasts = false;

            from.DOFade(0f, _fadeDuration);
            to.DOFade(1f, _fadeDuration).OnComplete(() =>
            {
                to.interactable = true;
                to.blocksRaycasts = true;
            });
        }

        private void ResetAllPanels()
        {
            SetGroupState(_menuGroup, true);
            SetGroupState(_settingsGroup, false);
            SetGroupState(_loginGroup, false);
            SetGroupState(_registerGroup, false);
            SetGroupState(_verifyGroup, false);
            SetGroupState(_selectGroup, false);
            SetGroupState(_roomsGroup, false);
        }

        private static void SetGroupState(CanvasGroup group, bool visible)
        {
            group.alpha = visible ? 1f : 0f;
            group.blocksRaycasts = visible;
            group.interactable = visible;
        }

        private static void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        
        private void OnDestroy()
        {
            _menuGroup.DOKill();
            _settingsGroup.DOKill();
            _loginGroup.DOKill();
            _registerGroup.DOKill();
            _verifyGroup.DOKill();
            _selectGroup.DOKill();
            _roomsGroup.DOKill();
        }
    }
}