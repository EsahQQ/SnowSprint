using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Features.UI
{
    public interface IMainMenuView
    {
        UniTask ProcessMenuAsync();
        
        void UpdateProfileUI(bool isSignedIn, string playerName);
        void ShowAuthPanel(bool show);
        
        UniTask<(bool isLogin, bool isCanceled, string username, string password)> WaitForAuthInputAsync();
        
        event System.Action OnLogoutClicked;
    }
}