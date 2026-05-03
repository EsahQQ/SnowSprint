using System;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Features.Network.Auth
{
    public interface IAuthService
    {
        bool IsSignedIn { get; }
        bool IsLoggedInAsUser { get; }
        string PlayerName { get; }

        event Action OnAuthStateChanged;

        UniTask InitializeAsync();
        UniTask<bool> SignInAsync(string username, string password);
        UniTask<bool> SignUpAsync(string username, string email, string password);
        void SignOut();
    }   
}

