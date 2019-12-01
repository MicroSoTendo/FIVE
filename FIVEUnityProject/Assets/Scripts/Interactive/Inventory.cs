using FIVE.EventSystem;
using FIVE.Robot;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;

namespace FIVE.Interactive
{
    public class Inventory
    {
        public ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>();

        public Inventory()
        {
            Items.CollectionChanged += ItemsCollectionChanged;
            Item.LeftClicked += OnItemLeftClicked;
            Item.ItemUsed += OnItemUsed;
        }

        private void OnItemUsed(Item item)
        {
            Items.Remove(item);
        }

        private void Purify(Item item)
        {
            MeshCollider mc = item.gameObject.GetComponent<MeshCollider>();
            if (mc != null)
            {
                mc.enabled = false;
            }
        }

        private void OnItemLeftClicked(Item item)
        {
            if (!Items.Contains(item))
            {
                bool closeEnough = (RobotManager.ActiveRobot.transform.position - item.gameObject.transform.position).magnitude < 100f;
                if (!closeEnough)
                {
                    return;
                }
                Purify(item);
                Items.Add(item);
                item.Collected = true;
            }
        }

        private static void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            sender.RaiseImmediate<OnInventoryChanged>(e);
        }
    }

    #region Events
    public class OnInventoryChanged : IEventType<NotifyCollectionChangedEventArgs> { }
    #endregion
}