using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.UI.Lobby
{
    public class LobbyView : MonoBehaviour, ILobbyView
    {
        [Header("Buttons")]
        [SerializeField] private Button _readyButton;
        
        private UniTaskCompletionSource _playCompletionSource;

        private void Start() => _readyButton.onClick.AddListener(PlayGame);
        private void PlayGame() => _playCompletionSource?.TrySetResult();
        
        public async UniTask ProcessLobbyAsync()
        {
            _playCompletionSource = new UniTaskCompletionSource();
            await _playCompletionSource.Task;
        }
    }
}