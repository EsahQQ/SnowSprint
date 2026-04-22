using System;
using _Project.Scripts.Features.UI.Shop.Settings;
using UnityEngine;

namespace _Project.Scripts.Features.Player.Services
{
    public class PlayerDataService : IPlayerDataService
    {
        private const string CoinsKey = "PLAYER_COINS";
        
        public int Coins 
        {
            get => PlayerPrefs.GetInt(CoinsKey, 0);
            private set 
            {
                PlayerPrefs.SetInt(CoinsKey, value);
                PlayerPrefs.Save();
                OnCoinsChanged?.Invoke(value);
            }
        }

        public event Action<int> OnCoinsChanged;

        public void AddCoins(int amount) => Coins += amount;

        public bool TrySpendCoins(int amount)
        {
            if (Coins < amount) 
                return false;
            
            Coins -= amount;
            return true;
        }

        public bool IsUpgradeBought(string upgradeId) => PlayerPrefs.GetInt($"UPGRADE_{upgradeId}", 0) == 1;

        public void UnlockUpgrade(string upgradeId)
        {
            PlayerPrefs.SetInt($"UPGRADE_{upgradeId}", 1);
            PlayerPrefs.Save();
        }

        public bool TryBuyUpgrade(ShopItemConfig item)
        {
            if (IsUpgradeBought(item.ID)) return false;

            if (!TrySpendCoins(item.Price)) return false;
            
            UnlockUpgrade(item.ID);
            return true;
        }
    }
}