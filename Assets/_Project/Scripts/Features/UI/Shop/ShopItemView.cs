using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.Services;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.UI.Shop
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
            if (_dataService.TryBuyUpgrade(_config))
            {
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