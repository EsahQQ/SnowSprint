using _Project.Scripts.Utils;
using UnityEngine;

namespace _Project.Scripts.Data
{
    [CreateAssetMenu(fileName = "ShopItem", menuName = "SO/New Item")]
    public class ShopItemConfig : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        public int Price;
        public Sprite Icon;

        public Enums.UpgradeType Type;
        public float Value;
    }
}