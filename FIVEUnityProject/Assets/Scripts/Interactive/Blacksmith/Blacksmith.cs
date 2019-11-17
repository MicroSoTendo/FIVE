using FIVE.Robot;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Interactive.Blacksmith
{
    public class Blacksmith
    {
        private static List<GameObject> ItemsIn = new List<GameObject>();

        public static GameObject ItemOut { get; set; }

        public static void AddIn(GameObject item)
        {
            ItemsIn.Add(item);
        }

        public static void RemoveIn(GameObject item)
        {
            ItemsIn.Remove(item);
        }

        public static GameObject GenerateOut()
        {
            ItemOut = ComponentsRead.GenerateItem(ItemsIn);
            return ItemOut;
        }

        public static void RemoveOut()
        {
            ItemOut = null;
        }

        public static void ResetIn()
        {
            ItemsIn = new List<GameObject>();
        }
    }
}