using _Project.Scripts.Features.Player.Services;
using _Project.Scripts.Features.Player.Settings;
using _Project.Scripts.Features.UI.Shop.Settings;
using _Project.Scripts.Features.Utils;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Player
{
    public class PlayerStatsHandler : MonoBehaviour
    {
        private PlayerSettings _playerSettings;

        public float CurrentMaxSpeed { get; private set; }
        public float CurrentAcceleration { get; private set; }
        public float CurrentBoostForce { get; private set; }
        public float CurrentJumpForce { get; private set; }

        private IPlayerDataService _playerData;
        private ShopDatabase _shopDatabase;

        [Inject]
        public void Construct(IPlayerDataService playerData, ShopDatabase shopDatabase, PlayerSettings playerSettings)
        {
            _playerData = playerData;
            _shopDatabase = shopDatabase;
            _playerSettings = playerSettings;
        }

        public void Initialize() => RecalculateStats();

        private void RecalculateStats()
        {
            CurrentMaxSpeed = _playerSettings.BaseMaxSpeed;
            CurrentAcceleration = _playerSettings.BaseAcceleration;
            CurrentBoostForce = _playerSettings.BaseBoostForce;
            CurrentJumpForce = _playerSettings.BaseJumpForce;

            foreach (var item in _shopDatabase.AllItems)
                if (_playerData.IsUpgradeBought(item.ID))
                    ApplyUpgrade(item);
        }

        private void ApplyUpgrade(ShopItemConfig item)
        {
            switch (item.Type)
            {
                case UpgradeType.MaxSpeed: CurrentMaxSpeed += item.Value; break;
                case UpgradeType.Acceleration: CurrentAcceleration += item.Value; break;
                case UpgradeType.BoostForce: CurrentBoostForce += item.Value; break;
                case UpgradeType.JumpForce: CurrentJumpForce += item.Value; break;
            }
        }
    }
}