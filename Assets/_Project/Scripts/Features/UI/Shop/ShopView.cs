using System.Collections.Generic;
using _Project.Scripts.Features.Player.Services;
using _Project.Scripts.Features.UI.Shop.Settings;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.Features.UI.Shop
{
    public class ShopView : MonoBehaviour, IShopView
    {
        [Header("Screens")]
        [SerializeField] private GameObject _shopPanel;
        [SerializeField] private TextMeshProUGUI _coinsText;
        [SerializeField] private Transform _shopContainer; 
        [SerializeField] private ShopItemView _shopItemPrefab;
        [SerializeField] private Button _readyButton;
        
        private readonly List<ShopItemView> _spawnedItems = new ();
        private IPlayerDataService _playerData;
        private ShopDatabase _shopDatabase; 
        
        private UniTaskCompletionSource _shopCompletionSource;

        [Inject]
        public void Construct(IPlayerDataService playerData, ShopDatabase shopDatabase)
        {
            _playerData = playerData;
            _shopDatabase = shopDatabase;
        }

        private void Start()
        {
            _shopPanel.SetActive(false);
            _readyButton.onClick.AddListener(OnNextLevelClick);
            GenerateShop();
        }

        public async UniTask ProcessShopAsync()
        {
            _playerData.OnCoinsChanged += UpdateCoinsUI;
            
            _shopPanel.SetActive(true);
            UpdateCoinsUI(_playerData.Coins);
            _shopPanel.transform.localPosition = new Vector3(0, 1000, 0);
            _shopPanel.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);

            _shopCompletionSource = new UniTaskCompletionSource();

            await _shopCompletionSource.Task;

            _playerData.OnCoinsChanged -= UpdateCoinsUI;
            _shopPanel.SetActive(false);
        }

        private void OnNextLevelClick() => _shopCompletionSource?.TrySetResult();

        private void GenerateShop()
        {
            foreach (Transform child in _shopContainer) 
                Destroy(child.gameObject);
            
            _spawnedItems.Clear();
            
            foreach (var config in _shopDatabase.AllItems)
            {
                var item = Instantiate(_shopItemPrefab, _shopContainer);
                item.Initialize(config, _playerData);
                _spawnedItems.Add(item);
            }
        }

        private void UpdateCoinsUI(int coins)
        {
            _coinsText.text = coins.ToString();
            _coinsText.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
            foreach (var item in _spawnedItems) 
                item.RefreshVisuals();
        }
    }
}