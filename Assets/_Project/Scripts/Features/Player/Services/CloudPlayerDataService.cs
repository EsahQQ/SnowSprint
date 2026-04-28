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

        private readonly Dictionary<string, int> _cloudDataCache = new();

        public event Action<int> OnCoinsChanged;

        public int Coins => GetInt(CoinsKey, 0);

        public async UniTask LoadProfileFromCloudAsync()
        {
            _cloudDataCache.Clear();
            try
            {
                var cloudData = await CloudSaveService.Instance.Data.Player.LoadAllAsync();
                
                foreach (var item in cloudData)
                {
                    _cloudDataCache[item.Key] = item.Value.Value.GetAs<int>();
                }
                
                Debug.Log($"[CloudSave] Профиль загружен! Монет: {Coins}");
                OnCoinsChanged?.Invoke(Coins); 
            }
            catch (Exception e)
            {
                Debug.LogError($"[CloudSave] Ошибка загрузки профиля: {e.Message}");
            }
        }

        public void AddCoins(int amount)
        {
            SetInt(CoinsKey, Coins + amount);
            OnCoinsChanged?.Invoke(Coins);
        }

        public bool TrySpendCoins(int amount)
        {
            if (Coins < amount) 
                return false;
            
            SetInt(CoinsKey, Coins - amount);
            OnCoinsChanged?.Invoke(Coins);
            return true;
        }

        public bool IsUpgradeBought(string upgradeId) => GetInt($"UPGRADE_{upgradeId}", 0) == 1;

        public void UnlockUpgrade(string upgradeId)
        {
            SetInt($"UPGRADE_{upgradeId}", 1);
            Debug.Log($"[CloudSave] Апгрейд {upgradeId} куплен и сохранен в облако!");
        }

        public bool TryBuyUpgrade(ShopItemConfig item)
        {
            if (IsUpgradeBought(item.ID)) return false;
            if (!TrySpendCoins(item.Price)) return false;
            
            UnlockUpgrade(item.ID);
            return true;
        }

        private int GetInt(string key, int defaultValue)
        {
            return _cloudDataCache.TryGetValue(key, out var val) ? val : defaultValue;
        }

        private void SetInt(string key, int value)
        {
            _cloudDataCache[key] = value;
            
            SaveToCloudAsync(key, value).Forget(); 
        }

        private async UniTaskVoid SaveToCloudAsync(string key, int value)
        {
            try
            {
                var data = new Dictionary<string, object> { { key, value } };
                await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            }
            catch (Exception e)
            {
                Debug.LogError($"[CloudSave] Ошибка сохранения {key}: {e.Message}");
            }
        }
    }
}