using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private CanvasGroup menuGroup;
        [SerializeField] private CanvasGroup settingsGroup;

        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button backButton; 
        
        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.4f;
        [SerializeField] private string gameSceneName = "Game";

        private void Start()
        {
            playButton.onClick.AddListener(PlayGame);
            exitButton.onClick.AddListener(QuitGame);
        
            settingsButton.onClick.AddListener(() => SwitchPanels(menuGroup, settingsGroup));
            backButton.onClick.AddListener(() => SwitchPanels(settingsGroup, menuGroup));

            ResetState();
        }

        private void ResetState()
        {
            menuGroup.alpha = 1;
            menuGroup.blocksRaycasts = true;
            menuGroup.interactable = true;

            settingsGroup.alpha = 0;
            settingsGroup.blocksRaycasts = false;
            settingsGroup.interactable = false;
        }

        private void SwitchPanels(CanvasGroup from, CanvasGroup to)
        {
            from.blocksRaycasts = false;
            to.blocksRaycasts = false;

            Sequence seq = DOTween.Sequence();

            seq.Append(from.DOFade(0, fadeDuration));

            seq.Append(to.DOFade(1, fadeDuration));

            seq.OnComplete(() => 
            {
                to.blocksRaycasts = true;
                to.interactable = true;

                from.interactable = false;
            });
        }

        private void PlayGame()
        {
            SceneManager.LoadScene(gameSceneName);
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