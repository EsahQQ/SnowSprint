using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;
using _Project.Scripts.Core;
using _Project.Scripts.Data;
using DG.Tweening;

namespace _Project.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject shopPanel;

        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI coinsText;

        [Header("Shop Elements")]
        [SerializeField] private Transform shopContainer; 
        [SerializeField] private ShopItemView shopItemPrefab;
        [SerializeField] private List<ShopItemConfig> shopConfigs;

        private GameManager _gameManager;
        private IPlayerDataService _playerData;
        private List<ShopItemView> _spawnedItems = new List<ShopItemView>();

        [Inject]
        public void Construct(GameManager gameManager, IPlayerDataService playerData)
        {
            _gameManager = gameManager;
            _playerData = playerData;
        }

        private void Start()
        {
            _gameManager.OnLevelStarted += ShowHUD;
            _gameManager.OnLevelFinished += ShowShop;
            _playerData.OnCoinsChanged += UpdateCoinsUI;

            UpdateCoinsUI(_playerData.Coins);
            GenerateShop();

            shopPanel.SetActive(false);
            hudPanel.SetActive(true);
        }

        private void OnDestroy()
        {
            _gameManager.OnLevelStarted -= ShowHUD;
            _gameManager.OnLevelFinished -= ShowShop;
            _playerData.OnCoinsChanged -= UpdateCoinsUI;
        }

        private void GenerateShop()
        {
            foreach (Transform child in shopContainer) Destroy(child.gameObject);
                _spawnedItems.Clear();
            
            foreach (var config in shopConfigs)
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

            foreach (var item in _spawnedItems) 
                item.RefreshVisuals();
        }

        private void ShowHUD()
        {
            shopPanel.SetActive(false);
        }

        private void ShowShop()
        {
            shopPanel.SetActive(true);

            shopPanel.transform.localPosition = new Vector3(0, 1000, 0);
            shopPanel.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutBack).SetUpdate(true); 
        }

        public void OnNextLevelClick()
        {
            _gameManager.RestartLevel();
        }
    }
}