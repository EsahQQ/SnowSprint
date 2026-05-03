using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;
using _Project.Scripts.Features.UI.Shop.Settings;

namespace _Project.Scripts.Features.Player.Services
{
    public class CloudPlayerDataService : IPlayerDataService
    {
        private const string CoinsKey = "PLAYER_COINS";
        private const string UpgradeKeyPrefix = "UPGRADE_";

        private readonly Dictionary<string, int> _cache = new();

        public event Action<int> OnCoinsChanged;

        public int Coins => GetCached(CoinsKey, defaultValue: 0);

        public async UniTask LoadProfileFromCloudAsync()
        {
            _cache.Clear();
            try
            {
                var cloudData = await CloudSaveService.Instance.Data.Player.LoadAllAsync();

                foreach (var item in cloudData)
                {
                    try
                    {
                        _cache[item.Key] = item.Value.Value.GetAs<int>();
                    }
                    catch
                    {
                        // ignored
                    }
                }

                Debug.Log($"[CloudSave] Profile loaded. Coins: {Coins}");
                OnCoinsChanged?.Invoke(Coins);
            }
            catch (Exception e)
            {
                Debug.LogError($"[CloudSave] Load failed: {e.Message}");
            }
        }

        public void AddCoins(int amount)
        {
            SetCached(CoinsKey, Coins + amount);
            OnCoinsChanged?.Invoke(Coins);
        }

        public bool TrySpendCoins(int amount)
        {
            if (Coins < amount) return false;

            SetCached(CoinsKey, Coins - amount);
            OnCoinsChanged?.Invoke(Coins);
            return true;
        }

        public bool IsUpgradeBought(string upgradeId) =>
            GetCached(UpgradeKeyPrefix + upgradeId, defaultValue: 0) == 1;

        public bool TryBuyUpgrade(ShopItemConfig item)
        {
            if (IsUpgradeBought(item.ID)) return false;
            if (!TrySpendCoins(item.Price)) return false;

            UnlockUpgrade(item.ID);
            return true;
        }

        private void UnlockUpgrade(string upgradeId)
        {
            SetCached(UpgradeKeyPrefix + upgradeId, 1);
            Debug.Log($"[CloudSave] Upgrade '{upgradeId}' unlocked.");
        }
        
        private int GetCached(string key, int defaultValue) =>
            _cache.TryGetValue(key, out int val) ? val : defaultValue;

        private void SetCached(string key, int value)
        {
            _cache[key] = value;
            SaveToCloudAsync(key, value).Forget();
        }

        private async UniTaskVoid SaveToCloudAsync(string key, int value)
        {
            try
            {
                await CloudSaveService.Instance.Data.Player.SaveAsync(
                    new Dictionary<string, object> { { key, value } });
            }
            catch (Exception e)
            {
                Debug.LogError($"[CloudSave] Save failed for '{key}': {e.Message}");
            }
        }
    }
}
