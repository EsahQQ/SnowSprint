using _Project.Scripts.Features.Utils;
using UnityEngine;

namespace _Project.Scripts.Features.UI.Shop.Settings
{
    public class ShopItemConfig : ScriptableObject
    {
        public string ID;
        public string DisplayName;
        public int Price;
        public Sprite Icon;

        public UpgradeType Type;
        public float Value;
    }
}