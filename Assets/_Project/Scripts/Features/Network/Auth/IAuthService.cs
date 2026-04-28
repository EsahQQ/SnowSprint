using System;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Features.Network.Auth
{
    public interface IAuthService
    {
        bool IsSignedIn { get; }
        string PlayerName { get; }
        
        event Action OnAuthStateChanged;

        UniTask InitializeAsync();
        UniTask<bool> SignInAsync(string username, string password);
        UniTask<bool> SignUpAsync(string username, string password);
        void SignOut();
    }
}

