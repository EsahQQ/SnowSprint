using System;
using System.Collections.Generic;
using _Project.Scripts.Features.UI.Shop.Settings;
using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Player.Services
{
    public class ClientPlayerDataService : IPlayerDataService, IInitializable, IDisposable
    {
        private int _coins;
        private readonly HashSet<string> _unlockedUpgrades = new();

        public event Action<int> OnCoinsChanged;
        public int Coins => _coins;

        private UniTaskCompletionSource _profileLoadTask;

        public void Initialize()
        {
            NetworkClient.RegisterHandler<ProfileDataResponse>(OnProfileDataReceived, false);
        }

        public void Dispose()
        {
            NetworkClient.UnregisterHandler<ProfileDataResponse>();
        }
        
        public async UniTask LoadProfileFromCloudAsync()
        {
            _profileLoadTask = new UniTaskCompletionSource();
            
            NetworkClient.Send(new GetProfileRequest());
            
            await _profileLoadTask.Task; 
            
            Debug.Log($"[LocalDB] Профиль загружен с сервера. Монеты: {Coins}");
        }

        private void OnProfileDataReceived(ProfileDataResponse msg)
        {
            _coins = msg.Coins;
            _unlockedUpgrades.Clear();
            
            if (msg.UnlockedUpgrades != null)
                foreach (var upgrade in msg.UnlockedUpgrades)
                    _unlockedUpgrades.Add(upgrade);

            OnCoinsChanged?.Invoke(_coins);
            _profileLoadTask?.TrySetResult();
        }

        public bool TryBuyUpgrade(ShopItemConfig item)
        {
            if (IsUpgradeBought(item.ID)) return false;
            if (_coins < item.Price) return false;

            _coins -= item.Price;
            _unlockedUpgrades.Add(item.ID);
            OnCoinsChanged?.Invoke(_coins);

            NetworkClient.Send(new BuyUpgradeRequest { UpgradeId = item.ID, Price = item.Price });
            
            return true;
        }

        public bool IsUpgradeBought(string upgradeId) => _unlockedUpgrades.Contains(upgradeId);
        
        public void AddCoins(int amount) {  }
        public bool TrySpendCoins(int amount) => false;
    }
}