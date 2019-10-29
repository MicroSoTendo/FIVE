using FIVE.EventSystem;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Interactive.Blacksmith
{
    public class Blacksmith
    {
        private static Dictionary<GameObject, List<GameObject>> CompositeItems = new Dictionary<GameObject, List<GameObject>>();
        private static Dictionary<GameObject, List<GameObject>> InventoryList = new Dictionary<GameObject, List<GameObject>>();
        //maybe not use static
        public static void AddForComposite(GameObject owner, GameObject item)
        {
            if (CompositeItems.TryGetValue(owner, out List<GameObject> items))
            {
                items.Add(item);
            }
            else
            {
                CompositeItems.Add(owner, new List<GameObject> { item });
            }
        }
        public static void RemoveFromComposite(GameObject owner, GameObject item)
        {
            if (CompositeItems.TryGetValue(owner, out List<GameObject> items))
            {
                items.Remove(item);
            }
        }

        public static void DoComposite(GameObject owner)
        {
            if (CompositeItems.TryGetValue(owner, out List<GameObject> items))
            {
                //Do composite with given formula
            }
        }
        public static int CompositeListCount()
        {
            return CompositeItems.Count;
        }
        public static GameObject GetCompositeItem(GameObject owner, int index)
        {
            List<GameObject> blacksmithHolder = CompositeItems[owner];
            return blacksmithHolder[index];
        }
        public static void ResetList(GameObject owner)
        {
            CompositeItems[owner] = new List<GameObject>();
        }

    }
}
