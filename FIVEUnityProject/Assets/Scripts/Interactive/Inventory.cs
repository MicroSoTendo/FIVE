using FIVE.EventSystem;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Interactive
{
    public class DropedItemToInventoryEventArgs : EventArgs
    {
        public GameObject Picker { get; }
        public GameObject Item { get; }

        public DropedItemToInventoryEventArgs(GameObject picker, GameObject item)
        {
            Item = item;
            Picker = picker;
        }
    }

    public class RemoveItemRequestedEventArgs : EventArgs
    {
        public GameObject Item { get; }

        public RemoveItemRequestedEventArgs(GameObject item)
        {
            Item = item;
        }
    }

    public class OnDropItemToInventory : IEventType<DropedItemToInventoryEventArgs> { }

    public class OnRemoveItemRequested : IEventType<RemoveItemRequestedEventArgs> { }

    public enum InventoryChangedAction
    {
        Add,
        Remove,
    }

    public class InventoryChangedEventArgs : EventArgs
    {
        public GameObject Item { get; }
        public int Index { get; }
        public InventoryChangedAction Action { get; }

        public InventoryChangedEventArgs(GameObject item, int index, InventoryChangedAction action)
        {
            Action = action;
            Index = index;
            Item = item;
        }
    }

    public class OnInventoryChanged : IEventType<InventoryChangedEventArgs> { }

    public class Inventory
    {
        private List<GameObject> items = new List<GameObject>();

        public IEnumerable<GameObject> Items => from item in items where item != null select item;

        public Inventory()
        {
            EventManager.Subscribe<OnDropItemToInventory, DropedItemToInventoryEventArgs>(OnDropItemToInventory);
            EventManager.Subscribe<OnRemoveItemRequested, RemoveItemRequestedEventArgs>(RemovedItem);
        }

        private void RemovedItem(object sender, RemoveItemRequestedEventArgs e)
        {
            Remove(e.Item);
        }

        private void OnDropItemToInventory(object sender, DropedItemToInventoryEventArgs e)
        {
            Add(e.Item);
        }

        private void OnChanged(GameObject gameObject, int index, InventoryChangedAction action)
        {
            EventManager.RaiseImmediate<OnInventoryChanged>(this, new InventoryChangedEventArgs(gameObject, index, action));
        }

        public void Add(GameObject item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == null)
                {
                    items[i] = item;
                    OnChanged(item, i, InventoryChangedAction.Add);
                    return;
                }
            }
            items.Add(item);
            OnChanged(item, items.Count - 1, InventoryChangedAction.Add);
        }

        public void Remove(GameObject item)
        {
            int i = items.IndexOf(item);
            items[i] = null;
            OnChanged(item, i, InventoryChangedAction.Remove);
        }
    }
}