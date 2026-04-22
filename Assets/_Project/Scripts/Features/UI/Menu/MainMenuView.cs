using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.UI
{
    public class MainMenuView : MonoBehaviour, IMainMenuView
    {
        [Header("Panels")]
        [SerializeField] private CanvasGroup _menuGroup;
        [SerializeField] private CanvasGroup _settingsGroup;

        [Header("Buttons")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _backButton; 
        
        [Header("Settings")]
        [SerializeField] private float _fadeDuration = 0.4f;

        private UniTaskCompletionSource _playCompletionSource;

        private void Start()
        {
            _playButton.onClick.AddListener(PlayGame);
            _exitButton.onClick.AddListener(QuitGame);
            _settingsButton.onClick.AddListener(() => SwitchPanels(_menuGroup, _settingsGroup));
            _backButton.onClick.AddListener(() => SwitchPanels(_settingsGroup, _menuGroup));

            ResetState();
        }

        public async UniTask ProcessMenuAsync()
        {
            _playCompletionSource = new UniTaskCompletionSource();
            await _playCompletionSource.Task;
        }
        
        private void PlayGame() => _playCompletionSource?.TrySetResult();

        private void ResetState()
        {
            _menuGroup.alpha = 1;
            _menuGroup.blocksRaycasts = true;
            _menuGroup.interactable = true;

            _settingsGroup.alpha = 0;
            _settingsGroup.blocksRaycasts = false;
            _settingsGroup.interactable = false;
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