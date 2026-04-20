using System;
using _Project.Scripts.Features.Data;
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

        public void AddCoins(int amount)
        {
            Coins += amount;
        }

        public bool TrySpendCoins(int amount)
        {
            if (Coins >= amount)
            {
                Coins -= amount;
                return true;
            }
            return false;
        }

        public bool IsUpgradeBought(string upgradeId)
        {
            return PlayerPrefs.GetInt($"UPGRADE_{upgradeId}", 0) == 1;
        }

        public void UnlockUpgrade(string upgradeId)
        {
            PlayerPrefs.SetInt($"UPGRADE_{upgradeId}", 1);
            PlayerPrefs.Save();
            Debug.Log($"Upgrade {upgradeId} unlocked!");
        }

        public bool TryBuyUpgrade(ShopItemConfig item)
        {
            if (IsUpgradeBought(item.id)) return false;

            if (!TrySpendCoins(item.price)) return false;
            
            UnlockUpgrade(item.id);
            return true;
        }
    }
}