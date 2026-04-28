using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.UI
{
    public class MainMenuView : MonoBehaviour, IMainMenuView
    {
        [Header("Panels")]
        [SerializeField] private CanvasGroup _menuGroup;
        [SerializeField] private CanvasGroup _settingsGroup;
        [SerializeField] private CanvasGroup _authGroup;

        [Header("Menu Buttons")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _backFromSettingsButton; 
        
        [Header("Profile UI (on Menu Panel)")]
        [SerializeField] private GameObject _profilePanel;
        [SerializeField] private TextMeshProUGUI _usernameText;
        [SerializeField] private Button _logoutButton;

        [Header("Auth Panel Elements")]
        [SerializeField] private TMP_InputField _usernameInput;
        [SerializeField] private TMP_InputField _passwordInput;
        [SerializeField] private Button _loginSubmitButton;
        [SerializeField] private Button _registerSubmitButton;
        [SerializeField] private Button _backFromAuthButton;

        [Header("Settings")]
        [SerializeField] private float _fadeDuration = 0.4f;

        private UniTaskCompletionSource _playCompletionSource;
        private UniTaskCompletionSource<(bool, bool, string, string)> _authCompletionSource;

        public event System.Action OnLogoutClicked;

        private void Start()
        {
            _playButton.onClick.AddListener(PlayGame);
            _exitButton.onClick.AddListener(QuitGame);
            _settingsButton.onClick.AddListener(() => SwitchPanels(_menuGroup, _settingsGroup));
            _backFromSettingsButton.onClick.AddListener(() => SwitchPanels(_settingsGroup, _menuGroup));
  
            _logoutButton.onClick.AddListener(() => OnLogoutClicked?.Invoke());

            _backFromAuthButton.onClick.AddListener(CancelAuth);
            _loginSubmitButton.onClick.AddListener(() => SubmitAuth(true));
            _registerSubmitButton.onClick.AddListener(() => SubmitAuth(false));

            ResetState();
        }

        public async UniTask ProcessMenuAsync()
        {
            _playCompletionSource = new UniTaskCompletionSource();
            await _playCompletionSource.Task;
        }
        
        public void UpdateProfileUI(bool isSignedIn, string playerName)
        {
            _profilePanel.SetActive(isSignedIn);
            if (isSignedIn)
                _usernameText.text = $"Привет, {playerName}!";
        }
        
        public void ShowAuthPanel(bool show)
        {
            if (show)
            {
                _usernameInput.text = "";
                _passwordInput.text = "";
                SwitchPanels(_menuGroup, _authGroup);
            }
            else
            {
                SwitchPanels(_authGroup, _menuGroup);
            }
        }

        public async UniTask<(bool isLogin, bool isCanceled, string username, string password)> WaitForAuthInputAsync()
        {
            _authCompletionSource = new UniTaskCompletionSource<(bool, bool, string, string)>();
            return await _authCompletionSource.Task;
        }

        private void SubmitAuth(bool isLogin)
        {
            if (string.IsNullOrEmpty(_usernameInput.text) || string.IsNullOrEmpty(_passwordInput.text))
                return; 

            _authCompletionSource?.TrySetResult((isLogin, false, _usernameInput.text, _passwordInput.text));
        }

        private void CancelAuth() => _authCompletionSource?.TrySetResult((false, true, "", ""));

        private void PlayGame() => _playCompletionSource?.TrySetResult();

        private void ResetState()
        {
            _menuGroup.alpha = 1;
            _menuGroup.blocksRaycasts = true;
            _menuGroup.interactable = true;

            _settingsGroup.alpha = 0;
            _settingsGroup.blocksRaycasts = false;
            _settingsGroup.interactable = false;
            
            _authGroup.alpha = 0;
            _authGroup.blocksRaycasts = false;
            _authGroup.interactable = false;
        }

        private void SwitchPanels(CanvasGroup from, CanvasGroup to)
        {
            from.blocksRaycasts = false;
            to.blocksRaycasts = false;

            var seq = DOTween.Sequence();
            seq.Append(from.DOFade(0, _fadeDuration));
            seq.Append(to.DOFade(1, _fadeDuration));
            seq.OnComplete(() => 
            {
                to.blocksRaycasts = true;
                to.interactable = true;
                from.interactable = false;
            });
        }

        private void QuitGame()
        {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}