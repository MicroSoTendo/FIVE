using FIVE.EventSystem;
using FIVE.Interactive;
using System;
using System.Collections.Specialized;
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
            binder.Bind(v => v.ExitButton.onClick).To(vm => vm.ExitInventory);
        }

        private void ExitInventory(object sender, EventArgs e)
        {
            SetEnabled(false);
        }

        private void AddCell(string id, out GameObject cell, out RectTransform rectTransform, out Transform contentTransform)
        {
            cell = View.AddUIElementFromResources<GameObject>("Cell", id, ContentTransform);
            cell.transform.SetParent(ContentTransform);
            rectTransform = cell.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            contentTransform = cell.GetChildGameObject("Content").transform;
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
                        //TODO:fix this long stuff
                        int totalColumns = (int)(ContentTransform.gameObject.transform.parent.transform.parent.GetComponent<RectTransform>().rect.width
                         - verticalBarWidth) / cellWidth;
                        int startingIndex = e.NewStartingIndex;
                    
                        int x = startingIndex % totalColumns;
                        int y = startingIndex / totalColumns;
                        AddCell($"Cell-{i + startingIndex}", out GameObject cell, out RectTransform rectTransform, out Transform itemHolder);
                        rectTransform.anchoredPosition = new Vector2(34 + x * cellWidth, -34 - y * cellWidth);
                        GameObject go = (GameObject)e.NewItems[i];
                        go.transform.SetParent(itemHolder);
                        go.transform.localPosition = new Vector3(0,0,0);
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
