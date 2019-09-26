using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Interactive
{
    public class InventoryManager
    {
        private static Dictionary<GameObject, Inventory> Inventories = new Dictionary<GameObject, Inventory>();
        public static Inventory AddInventory(GameObject owner)
        {
            var inventory = new Inventory(owner);
            Inventories.Add(owner, inventory);
            return inventory;
        }
    }
}
