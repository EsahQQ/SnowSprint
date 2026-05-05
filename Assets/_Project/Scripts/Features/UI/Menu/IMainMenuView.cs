using System;
using _Project.Scripts.Features.Network.Server.Auth.Data;

namespace _Project.Scripts.Features.UI.Menu
{
    public interface IMainMenuView
    {
        event Action OnPlayClicked;
        event Action OnLogoutClicked;
        public event Action<AuthData> OnAuthActionSubmitted;

        void UpdateProfileUI(bool isLoggedIn, string playerName);
        void ShowLoginPanel(bool show);
        void ShowRegisterPanel(bool show);
        void ShowVerifyPanel(bool show);
        void SwitchBetweenAuthPanels(AuthAction action);
        void ShowSelectActionPanel(bool show); 
    }
}