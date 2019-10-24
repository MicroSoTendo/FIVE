using FIVE.Robot;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Interactive
{
    public class InventoryManager
    {
        private static readonly Dictionary<GameObject, Inventory> Inventories = new Dictionary<GameObject, Inventory>();

        public static Inventory AddInventory(GameObject owner)
        {
            Inventory inventory = new Inventory(owner);
            Inventories.Add(owner, inventory);
            return inventory;
        }
        public static Inventory FindInventory()
        {
            Inventory inventory =  null;
            if (RobotManager.ActiveRobot != null) {
                inventory = Inventories[RobotManager.ActiveRobot];
            }
            return inventory;
        }
    }
}