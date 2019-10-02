using FIVE.EventSystem;
using FIVE.Interactive;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using FIVE.CameraSystem;
using UnityEngine;

namespace FIVE.UI.InGameDisplay
{
    internal class InventoryViewModel : ViewModel<InventoryView, InventoryViewModel>
    {
        private GameObject Content { get; }
        private RectTransform ContentTransform { get; }
        private int total;
        public Inventory Inventory { get; set; }
        public InventoryViewModel()
        {
            EventManager.Subscribe<OnInventoryChanged, NotifyCollectionChangedEventArgs>(OnInventoryChanged);
            Content = View.InventoryScrollView.GetChildGameObject("InventoryContent");
            ContentTransform = Content.GetComponent<RectTransform>();
            binder.Bind(v => v.ExitButton.onClick).To(vm => vm.ExitInventory);
            View.ViewCanvas.worldCamera = Camera.current;
            View.ViewCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            View.ViewCanvas.planeDistance = 0.5f;
        }

        public override void SetEnabled(bool value)
        {
            base.SetEnabled(value);
            string s = value ? "Enabled" : "Disabled";
            Debug.Log($"Inventory is {s}");
            View.ViewCanvas.worldCamera = CameraManager.CurrentActiveCamera;
        }

        public override void ToggleEnabled()
        {
            base.ToggleEnabled();
            View.ViewCanvas.worldCamera = CameraManager.CurrentActiveCamera;
        }

        private void ExitInventory(object sender, EventArgs e)
        {
            SetEnabled(false);
        }

        private GameObject AddCell(string id, out RectTransform rectTransform, out Transform itemHolder)
        {
            GameObject cell = View.AddUIElementFromResources<GameObject>("Cell", id, ContentTransform);
            cell.transform.SetParent(ContentTransform);
            rectTransform = cell.GetComponent<RectTransform>();
            itemHolder = cell.GetChildGameObject("Content").transform;
            return cell;
        }

        private List<GameObject> cellList;

        private void OnInventoryChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {

                        int startingIndex = e.NewStartingIndex;
                        GameObject newCell = AddCell($"Cell-{i + startingIndex}", out RectTransform rectTransform, out Transform itemHolder);

                        float cellWidth = rectTransform.sizeDelta.x; //TODO: get by call
                        int totalColumns = (int)(ContentTransform.rect.width / cellWidth);
                        int x = startingIndex % totalColumns;
                        int y = startingIndex / totalColumns;
                        rectTransform.anchoredPosition = new Vector2(x * cellWidth, - y * cellWidth);

                        GameObject go = (GameObject)e.NewItems[i];
                        go.GetComponent<MeshRenderer>().receiveShadows = false;
                        go.transform.SetParent(itemHolder);

                        //TODO:Parametrize it into Item.cs
                        go.transform.localScale = new Vector3(7.7f, 7.7f, 7.7f);
                        go.transform.localEulerAngles = new Vector3(-90,0,0);
                        go.transform.localPosition = new Vector3(7.5f,-55,-25);

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
