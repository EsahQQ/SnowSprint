using System;
using _Project.Scripts.Data;

namespace _Project.Scripts.Infrastructure.Services
{
    public interface IPlayerDataService
    {
        int Coins { get; }
        void AddCoins(int amount);
        bool TrySpendCoins(int amount);
        bool IsUpgradeBought(string upgradeId);
        void UnlockUpgrade(string upgradeId);
        event Action<int> OnCoinsChanged; 
        bool TryBuyUpgrade(ShopItemConfig item);
    }
}