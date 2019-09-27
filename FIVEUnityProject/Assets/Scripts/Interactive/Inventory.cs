using FIVE.EventSystem;
using FIVE.UI;
using System;
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

    public class OnDropItemToInventory : IEventType<DropedItemToInventoryEventArgs> { }

    public class OnInventoryChanged : IEventType<NotifyCollectionChangedEventArgs> { }

    public class Inventory : Observable<NotifyCollectionChangedEventArgs>
    {
        private readonly ObservableCollection<GameObject> items = new ObservableCollection<GameObject>();
        public int Capacity { get; set; } = 100;
        public int Count => items.Count;

        public GameObject Owner { get; private set; }

        public Inventory(GameObject owner)
        {
            Owner = owner;
            items.CollectionChanged += OnCollectionChanged;
            EventManager.Subscribe<OnDropItemToInventory, DropedItemToInventoryEventArgs>(OnDropItemToInventory);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            sender.RaiseEvent<OnInventoryChanged, NotifyCollectionChangedEventArgs>(e);
        }

        private void OnDropItemToInventory(object sender, DropedItemToInventoryEventArgs e)
        {
            //TODO: Size checking
            if (e.Index != null)
            {
                items.Insert(e.Index.Value, e.Item);
            }
            else
            {
                items.Add(e.Item);
            }
        }


        public void ChangeOwner(GameObject newOwner)
        {
            Owner = newOwner;
        }


        public void Add(GameObject item)
        {
            items.Add(item);
        }

        public void Insert(int index, GameObject item)
        {
            items.Insert(index, item);
        }

        public void Replace(GameObject oldItem, GameObject newItem)
        {
            int index = items.IndexOf(oldItem);
            items[index] = newItem;
        }

        public void Remove(GameObject item)
        {
            items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

    }
}
