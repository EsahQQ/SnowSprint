using System;
using _Project.Scripts.Features.UI.Shop.Settings;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Features.Player.Services
{
    public interface IPlayerDataService
    {
        int Coins { get; }
        UniTask LoadProfileFromCloudAsync(); 
        void AddCoins(int amount);
        bool TrySpendCoins(int amount);
        bool IsUpgradeBought(string upgradeId);
        void UnlockUpgrade(string upgradeId);
        event Action<int> OnCoinsChanged; 
        bool TryBuyUpgrade(ShopItemConfig item);
    }
}