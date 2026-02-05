using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using _Project.Scripts.Core;
using _Project.Scripts.Data;
using DG.Tweening; // Анимации

namespace _Project.Scripts.UI
{
    public class ShopItemView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Button buyButton;
        [SerializeField] private CanvasGroup canvasGroup;

        private ShopItemConfig _config;
        private IPlayerDataService _dataService;

        public void Initialize(ShopItemConfig config, IPlayerDataService dataService)
        {
            _config = config;
            _dataService = dataService;

            iconImage.sprite = config.icon;
            nameText.text = config.displayName;
            
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(TryBuy);

            RefreshVisuals();
        }

        private void TryBuy()
        {
            if (_dataService.TrySpendCoins(_config.price))
            {
                _dataService.UnlockUpgrade(_config.id);

                transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
                RefreshVisuals();
            }
            else
            {
                transform.DOShakePosition(0.5f, 10f);
            }
        }

        public void RefreshVisuals()
        {
            bool isBought = _dataService.IsUpgradeBought(_config.id);

            if (isBought)
            {
                priceText.text = "ХХ";
                buyButton.interactable = false;
                canvasGroup.alpha = 0.5f; 
            }
            else
            {
                priceText.text = _config.price.ToString();
                buyButton.interactable = true;
                canvasGroup.alpha = 1f;
            }
        }
    }
}