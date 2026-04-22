using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.UI.Lobby
{
    public class LobbyView : MonoBehaviour, ILobbyView
    {
        [Header("Buttons")]
        [SerializeField] private Button readyButton;
        
        private UniTaskCompletionSource _playCompletionSource;

        private void Start() => readyButton.onClick.AddListener(PlayGame);

        public async UniTask ProcessLobbyAsync()
        {
            _playCompletionSource = new UniTaskCompletionSource();
            await _playCompletionSource.Task;
        }
        
        private void PlayGame() => _playCompletionSource?.TrySetResult();
    }
}