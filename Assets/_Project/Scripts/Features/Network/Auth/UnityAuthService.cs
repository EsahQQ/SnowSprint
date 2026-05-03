using System;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace _Project.Scripts.Features.Network.Auth
{
    public class UnityAuthService : IAuthService
    {
        private const string PlayerNameKey = "SavedPlayerName";
        private bool _isInitialized;

        public bool IsSignedIn => AuthenticationService.Instance.IsSignedIn;
        
        public bool IsLoggedInAsUser { get; private set; }
        public string PlayerName { get; private set; }

        public event Action OnAuthStateChanged;

        public async UniTask InitializeAsync()
        {
            if (_isInitialized) return;

            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                var options = new InitializationOptions();
#if UNITY_EDITOR
                options.SetProfile(ParrelSync.ClonesManager.IsClone()
                    ? ParrelSync.ClonesManager.GetArgument()
                    : "Host");
#endif
                await UnityServices.InitializeAsync(options);
            }

            _isInitialized = true;
            
            await SignInAnonymouslyAsync();

            try
            {
                var playerInfo = await AuthenticationService.Instance.GetPlayerInfoAsync();
                IsLoggedInAsUser = playerInfo.Identities != null && playerInfo.Identities.Exists(id => id.TypeId == "username");
                PlayerName = IsLoggedInAsUser ? PlayerPrefs.GetString(PlayerNameKey, "Player") : "Guest";
            }
            catch
            {
                IsLoggedInAsUser = false;
                PlayerName = "Guest";
            }
            
            OnAuthStateChanged?.Invoke();
        }

        public async UniTask<bool> SignUpAsync(string username, string email, string password)
        {
            try
            {
                if (AuthenticationService.Instance.IsSignedIn)
                    AuthenticationService.Instance.SignOut();
                
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(email, password);
                
                SetPlayerAsRealUser(username);
                UpdateCloudNameAsync(username).Forget();
        
                return true;
            }
            catch (AuthenticationException ex)
            {
                Debug.LogWarning($"[Auth] Ошибка регистрации (возможно, Email уже занят): {ex.Message}");
                await SignInAnonymouslyAsync();
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Auth] Неизвестная ошибка: {e.Message}");
                await SignInAnonymouslyAsync();
                return false;
            }
        }

        public async UniTask<bool> SignInAsync(string email, string password)
        {
            try
            {
                if (AuthenticationService.Instance.IsSignedIn)
                    AuthenticationService.Instance.SignOut();
                
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(email, password);
                
                string loadedName = await AuthenticationService.Instance.GetPlayerNameAsync();
                SetPlayerAsRealUser(string.IsNullOrEmpty(loadedName) ? "Player" : loadedName);
        
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Auth] Ошибка входа (неверная почта или пароль): {e.Message}");
                await SignInAnonymouslyAsync();
                return false;
            }
        }

        public void SignOut()
        {
            PlayerPrefs.DeleteKey(PlayerNameKey);
            PlayerPrefs.Save();
            
            AuthenticationService.Instance.SignOut();
            
            IsLoggedInAsUser = false;
            PlayerName = "Guest";
            OnAuthStateChanged?.Invoke();
            
            SignInAnonymouslyAsync().Forget();
        }

        private async UniTask SignInAnonymouslyAsync()
        {
            if (AuthenticationService.Instance.IsSignedIn) return;

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Auth] Anonymous sign-in failed: {e.Message}");
            }
        }

        private async UniTaskVoid UpdateCloudNameAsync(string name)
        {
            try { await AuthenticationService.Instance.UpdatePlayerNameAsync(name); }
            catch { Debug.LogWarning("[Auth] Name service unavailable. Name saved locally."); }
        }

        private void SetPlayerAsRealUser(string username)
        {
            IsLoggedInAsUser = true;
            PlayerName = username;
            PlayerPrefs.SetString(PlayerNameKey, username);
            PlayerPrefs.Save();
            OnAuthStateChanged?.Invoke();
        }
    }
}