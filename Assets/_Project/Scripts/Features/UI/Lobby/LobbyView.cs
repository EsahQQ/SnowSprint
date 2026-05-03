using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.UI.Lobby
{
    public class LobbyView : MonoBehaviour, ILobbyView
    {
        [SerializeField] private Button _readyButton;
        [SerializeField] private TextMeshProUGUI _readyStatusText;
        
        private UniTaskCompletionSource _playCompletionSource;

        private void Start() => _readyButton.onClick.AddListener(OnReadyClicked);

        private void OnReadyClicked()
        {
            SetInteractable(false); 
            _playCompletionSource?.TrySetResult();
        }
        
        public async UniTask ProcessLobbyAsync()
        {
            SetInteractable(true);
            _playCompletionSource = new UniTaskCompletionSource();
            await _playCompletionSource.Task;
        }

        public void UpdateReadyCount(int readyCount, int totalCount)
        {
            Debug.Log($"[UI] Обновление текста лобби: {readyCount} / {totalCount}");
            _readyStatusText.text = $"{readyCount} / {totalCount}";
        }
        public void SetInteractable(bool isInteractable) => _readyButton.interactable = isInteractable;
    }
}