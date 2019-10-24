using FIVE.EventSystem;
using FIVE.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;

namespace FIVE.Interactive
{
    public class DropedItemToInventoryEventArgs : EventArgs
    {
        public GameObject Picker { get; }
        public int? Index { get; }
        public GameObject Item { get; }

        public DropedItemToInventoryEventArgs(GameObject picker, int? index, GameObject item)
        {
            Index = index;
            Item = item;
            Picker = picker;
        }
    }

    public class RemoveItemRequestedEventArgs : EventArgs
    {
        public GameObject Item { get; }
        public RemoveItemRequestedEventArgs(GameObject item) => Item = item;
    }

    public class OnDropItemToInventory : IEventType<DropedItemToInventoryEventArgs> { }

    public class OnRemoveItemRequested : IEventType<RemoveItemRequestedEventArgs> { }

    public enum InventoryChangedAction
    {
        Add,
        Remove,
        RemoveAt,
        Insert,
        Replace
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
        private readonly List<GameObject> items = new List<GameObject>();
        public int Capacity { get; set; } = 100;
        public int Count => items.Count;

        public GameObject Owner { get; private set; }

        public List<GameObject> ListItem()
        {
            return items;
        }
        public Inventory(GameObject owner)
        {
            Owner = owner;
            EventManager.Subscribe<OnDropItemToInventory, DropedItemToInventoryEventArgs>(OnDropItemToInventory);
            EventManager.Subscribe<OnRemoveItemRequested, RemoveItemRequestedEventArgs>(RemovedItem);
        }

        private void RemovedItem(object sender, RemoveItemRequestedEventArgs e)
        {
            Remove(e.Item);
        }

        private void OnDropItemToInventory(object sender, DropedItemToInventoryEventArgs e)
        {
            //TODO: Size checking
            if (e.Index == null)
            {
                Add(e.Item);
            }
            else
            {
                Insert(e.Index.Value, e.Item);
            }
        }

        private void OnChanged(GameObject gameObject, int index, InventoryChangedAction action)
        {
            EventManager.RaiseImmediate<OnInventoryChanged>(this, new InventoryChangedEventArgs(gameObject, index, action));
        }

        public void ChangeOwner(GameObject newOwner)
        {
            Owner = newOwner;
        }

        public void Add(GameObject item)
        {
            OnChanged(item, items.Count, InventoryChangedAction.Add);
            item.GetComponent<Item>().Owner = Owner;
            items.Add(item);
        }

        public void Insert(int index, GameObject item)
        {
            OnChanged(item, index, InventoryChangedAction.Insert);
            items.Insert(index, item);
        }

        public void Replace(GameObject oldItem, GameObject newItem)
        {
            int index = items.IndexOf(oldItem);
            items[index] = newItem;
            OnChanged(newItem, index, InventoryChangedAction.Replace);
        }

        public void Remove(GameObject item)
        {
            OnChanged(item, items.IndexOf(item), InventoryChangedAction.Remove);
            items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            OnChanged(items[index], index, InventoryChangedAction.RemoveAt);
            items.RemoveAt(index);
        }
    }
}