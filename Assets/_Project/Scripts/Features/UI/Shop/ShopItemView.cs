using _Project.Scripts.Features.Player.Services;
using _Project.Scripts.Features.UI.Shop.Settings;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.UI.Shop
{
    public class ShopItemView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private Button _buyButton;
        [SerializeField] private CanvasGroup _canvasGroup;

        private ShopItemConfig _config;
        private IPlayerDataService _dataService;

        public void Initialize(ShopItemConfig config, IPlayerDataService dataService)
        {
            _config = config;
            _dataService = dataService;

            _iconImage.sprite = config.Icon;
            _nameText.text = config.DisplayName;
            
            _buyButton.onClick.RemoveAllListeners();
            _buyButton.onClick.AddListener(TryBuy);

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
            bool isBought = _dataService.IsUpgradeBought(_config.ID);

            if (isBought)
            {
                _priceText.text = "ХХ";
                _buyButton.interactable = false;
                _canvasGroup.alpha = 0.5f; 
            }
            else
            {
                _priceText.text = _config.Price.ToString();
                _buyButton.interactable = true;
                _canvasGroup.alpha = 1f;
            }
        }
    }
}