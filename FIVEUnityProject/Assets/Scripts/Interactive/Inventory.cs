using FIVE.EventSystem;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

    public class OnInventoryChanged : IEventType<NotifyCollectionChangedEventArgs> { }

    public class Inventory
    {
        public ObservableCollection<GameObject> Items { get; } = new ObservableCollection<GameObject>();

        public Inventory()
        {
            EventManager.Subscribe<OnDropItemToInventory, DropedItemToInventoryEventArgs>(OnDropItemToInventory);
            EventManager.Subscribe<OnRemoveItemRequested, RemoveItemRequestedEventArgs>(RemovedItem);
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private static void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            sender.RaiseImmediate<OnInventoryChanged>(e);
        }

        private void RemovedItem(object sender, RemoveItemRequestedEventArgs e)
        {
            Items.Remove(e.Item);
        }

        private void OnDropItemToInventory(object sender, DropedItemToInventoryEventArgs e)
        {
            Items.Add(e.Item);
        }

    }
}