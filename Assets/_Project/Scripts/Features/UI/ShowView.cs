using System.Collections.Generic;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.Player.Services;
using _Project.Scripts.Features.Services;
using _Project.Scripts.Features.UI.Shop;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.UI
{
    public class ShopView : MonoBehaviour, IShopView
    {
        [Header("Screens")]
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private Transform shopContainer; 
        [SerializeField] private ShopItemView shopItemPrefab;

        private IPlayerDataService _playerData;
        private ShopDatabase _shopDatabase; 
        private List<ShopItemView> _spawnedItems = new List<ShopItemView>();
        
        private UniTaskCompletionSource _shopCompletionSource;

        [Inject]
        public void Construct(IPlayerDataService playerData, ShopDatabase shopDatabase)
        {
            _playerData = playerData;
            _shopDatabase = shopDatabase;
        }

        private void Start()
        {
            shopPanel.SetActive(false);
            GenerateShop();
        }

        public async UniTask ProcessShopAsync()
        {
            _playerData.OnCoinsChanged += UpdateCoinsUI;
            
            shopPanel.SetActive(true);
            UpdateCoinsUI(_playerData.Coins);
            shopPanel.transform.localPosition = new Vector3(0, 1000, 0);
            shopPanel.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);

            _shopCompletionSource = new UniTaskCompletionSource();

            await _shopCompletionSource.Task;

            _playerData.OnCoinsChanged -= UpdateCoinsUI;
            shopPanel.SetActive(false);
        }

        public void OnNextLevelClick()
        {
            _shopCompletionSource?.TrySetResult();
        }

        private void GenerateShop()
        {
            foreach (Transform child in shopContainer) Destroy(child.gameObject);
            _spawnedItems.Clear();
            
            foreach (var config in _shopDatabase.allItems)
            {
                var item = Instantiate(shopItemPrefab, shopContainer);
                item.Initialize(config, _playerData);
                _spawnedItems.Add(item);
            }
        }

        private void UpdateCoinsUI(int coins)
        {
            coinsText.text = coins.ToString();
            coinsText.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
            foreach (var item in _spawnedItems) item.RefreshVisuals();
        }
    }
}