using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Features.UI.Lobby
{
    public interface ILobbyView
    {
        UniTask ProcessLobbyAsync(); 
        void UpdateReadyCount(int readyCount, int totalCount);
        void SetInteractable(bool isInteractable);
    }
}