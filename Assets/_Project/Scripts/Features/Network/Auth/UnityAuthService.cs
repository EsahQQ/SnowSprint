using System;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace _Project.Scripts.Features.Network.Auth
{
    public class UnityAuthService : IAuthService
    {
        public bool IsSignedIn => AuthenticationService.Instance.IsSignedIn;
        public string PlayerName { get; private set; }

        public event Action OnAuthStateChanged;

        public async UniTask InitializeAsync()
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                var options = new InitializationOptions();
                #if UNITY_EDITOR
                if (ParrelSync.ClonesManager.IsClone())
                    options.SetProfile(ParrelSync.ClonesManager.GetArgument());
                else
                    options.SetProfile("Host");
                #endif
                
                await UnityServices.InitializeAsync(options);
            }
            
            if (AuthenticationService.Instance.SessionTokenExists)
            {
                try
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync(); 
                    
                    PlayerName = await AuthenticationService.Instance.GetPlayerNameAsync();
                    OnAuthStateChanged?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[Auth] Ошибка восстановления сессии: {e.Message}");
                }
            }
        }

        public async UniTask<bool> SignInAsync(string username, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
                PlayerName = username;

                await AuthenticationService.Instance.UpdatePlayerNameAsync(username);
                
                OnAuthStateChanged?.Invoke();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Auth] Ошибка входа: {e.Message}");
                return false;
            }
        }

        public async UniTask<bool> SignUpAsync(string username, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                PlayerName = username;
                await AuthenticationService.Instance.UpdatePlayerNameAsync(username);
                
                OnAuthStateChanged?.Invoke();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Auth] Ошибка регистрации: {e.Message}");
                return false;
            }
        }

        public void SignOut()
        {
            AuthenticationService.Instance.SignOut();
            AuthenticationService.Instance.ClearSessionToken();
            PlayerName = string.Empty;
            OnAuthStateChanged?.Invoke();
        }
    }
}