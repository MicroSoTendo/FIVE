using System;
using System.Collections.Specialized;
using FIVE.EventSystem;
using FIVE.Interactive;
using UnityEngine;

namespace FIVE.UI.InGameDisplay
{
    internal class InventoryViewModel : ViewModel<InventoryView, InventoryViewModel>
    {
        private GameObject Content { get; }
        private RectTransform ContentTransform { get; }

        public Inventory Inventory { get; set; }
        public InventoryViewModel()
        {
            EventManager.Subscribe<OnInventoryChanged, NotifyCollectionChangedEventArgs>(OnInventoryChanged);
            Content = View.InventoryScrollView.GetChildGameObject("InventoryContent");
            ContentTransform = Content.GetComponent<RectTransform>();
            binder.Bind(v=>v.ExitButton.onClick).To(vm=>vm.ExitInventory);
        }

        private void ExitInventory(object sender, EventArgs e)
        {
            SetActive(false);
        }

        private void AddCell(string id, out GameObject cell, out RectTransform rectTransform)
        {
            cell = View.AddUIElementFromResources<GameObject>("Cell", id, ContentTransform);
            rectTransform = cell.GetComponent<RectTransform>();
        }

        private void OnInventoryChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        int verticalBarWidth = 20; //TODO: get by call
                        int cellWidth = 62; //TODO: get by call
                        int totalColumns = (int)(ContentTransform.sizeDelta.x - verticalBarWidth) / cellWidth;
                        int startingIndex = e.NewStartingIndex;
                        int x = startingIndex % totalColumns;
                        int y = startingIndex / totalColumns;
                        AddCell($"Cell-{i + startingIndex}", out GameObject cell, out RectTransform rectTransform);
                        rectTransform.anchorMin = new Vector2(0,1);
                        rectTransform.anchorMax = new Vector2(0, 1);
                        rectTransform.pivot = new Vector2(0.5f, 0.5f);
                        rectTransform.anchoredPosition = new Vector2(34 + x * cellWidth, -34 - y * cellWidth);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

    }
}
