using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Interactive
{
    public class InventoryManager
    {
        private static readonly Dictionary<GameObject, Inventory> Inventories = new Dictionary<GameObject, Inventory>();

        public static Inventory AddInventory(GameObject owner)
        {
            var inventory = new Inventory(owner);
            Inventories.Add(owner, inventory);
            return inventory;
        }

        public static Inventory GetInventory(GameObject owner)
        {
            if (owner == null)
            {
                return null;
            }

            if (Inventories.TryGetValue(owner, out Inventory inventory))
            {
                return inventory;
            }
            return null;
        }
    }
}