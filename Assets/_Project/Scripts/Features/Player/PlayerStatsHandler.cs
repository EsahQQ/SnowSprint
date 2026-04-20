using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.Player.Services;
using _Project.Scripts.Features.Services;
using _Project.Scripts.Features.Utils;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Player
{
    public class PlayerStatsHandler : MonoBehaviour
    {
        [Header("Base Settings")]
        [SerializeField] private float baseMaxSpeed = 10f;
        [SerializeField] private float baseAcceleration = 2f;
        [SerializeField] private float baseBoostForce = 2f;
        [SerializeField] private float baseJumpForce = 5f;

        public float CurrentMaxSpeed { get; private set; }
        public float CurrentAcceleration { get; private set; }
        public float CurrentBoostForce { get; private set; }
        public float CurrentJumpForce { get; private set; }

        private IPlayerDataService _playerData;
        private ShopDatabase _shopDatabase;

        [Inject]
        public void Construct(IPlayerDataService playerData, ShopDatabase shopDatabase)
        {
            _playerData = playerData;
            _shopDatabase = shopDatabase;
        }

        private void Start()
        {
            RecalculateStats();
        }

        private void RecalculateStats()
        {
            CurrentMaxSpeed = baseMaxSpeed;
            CurrentAcceleration = baseAcceleration;
            CurrentBoostForce = baseBoostForce;
            CurrentJumpForce = baseJumpForce;

            foreach (var item in _shopDatabase.allItems)
                if (_playerData.IsUpgradeBought(item.id))
                    ApplyUpgrade(item);
        }

        private void ApplyUpgrade(ShopItemConfig item)
        {
            switch (item.type)
            {
                case UpgradeType.MaxSpeed: CurrentMaxSpeed += item.value; break;
                case UpgradeType.Acceleration: CurrentAcceleration += item.value; break;
                case UpgradeType.BoostForce: CurrentBoostForce += item.value; break;
                case UpgradeType.JumpForce: CurrentJumpForce += item.value; break;
            }
        }
    }
}