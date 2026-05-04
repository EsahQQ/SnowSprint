using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Features.UI.Shop.Settings
{
    public class ShopDatabase : ScriptableObject
    {
        public List<ShopItemConfig> AllItems;

        public ShopItemConfig GetItemById(string id) =>
            AllItems.FirstOrDefault(item => item.ID == id);
    }
}