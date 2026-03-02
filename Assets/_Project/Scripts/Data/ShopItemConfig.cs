using _Project.Scripts.Core;
using UnityEngine;

namespace _Project.Scripts.Data
{
    [CreateAssetMenu(fileName = "ShopItem", menuName = "SO/New Item")]
    public class ShopItemConfig : ScriptableObject
    {
        public string id;
        public string displayName;
        public int price;
        public Sprite icon;

        public Enums.UpgradeType type;
    }
}