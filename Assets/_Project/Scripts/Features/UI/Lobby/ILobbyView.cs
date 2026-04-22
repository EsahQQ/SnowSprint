using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Features.UI.Lobby
{
    public interface ILobbyView
    {
        UniTask ProcessLobbyAsync(); 
    }
}