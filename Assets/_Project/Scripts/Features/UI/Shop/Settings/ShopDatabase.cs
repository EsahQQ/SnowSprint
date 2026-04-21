using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Features.Data
{
    [CreateAssetMenu(fileName = "ShopDatabase", menuName = "SO/ShopDatabase")]
    public class ShopDatabase : ScriptableObject
    {
        public List<ShopItemConfig> allItems;
    }
}