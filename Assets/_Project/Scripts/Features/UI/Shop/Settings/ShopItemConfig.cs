using _Project.Scripts.Features.Utils;
using UnityEngine;

namespace _Project.Scripts.Features.Data
{
    [CreateAssetMenu(fileName = "ShopItem", menuName = "SO/New Item")]
    public class ShopItemConfig : ScriptableObject
    {
        public string id;
        public string displayName;
        public int price;
        public Sprite icon;

        public UpgradeType type;
        public float value;
    }
}