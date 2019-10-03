using FIVE.CameraSystem;
using FIVE.EventSystem;
using FIVE.Interactive;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.UI.InGameDisplay
{
    internal class InventoryViewModel : ViewModel<InventoryView, InventoryViewModel>
    {
        private readonly Dictionary<int, GameObject> cellDictionary = new Dictionary<int, GameObject>();

        public InventoryViewModel()
        {
            EventManager.Subscribe<OnInventoryChanged, InventoryChangedEventArgs>(OnInventoryChanged);
            Content = View.InventoryScrollView.FindChildRecursive("InventoryContent");
            ContentTransform = Content.GetComponent<RectTransform>();
            binder.Bind(v => v.ExitButton.onClick).To(vm => vm.ExitInventory);
            View.ViewCanvas.worldCamera = Camera.current;
            View.ViewCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            View.ViewCanvas.planeDistance = 0.5f;
        }

        private GameObject Content { get; }
        private RectTransform ContentTransform { get; }
        public Inventory Inventory { get; set; }

        public override void SetEnabled(bool value)
        {
            base.SetEnabled(value);
            View.ViewCanvas.worldCamera = CameraManager.CurrentActiveCamera;
            SetUpCells();
        }

        public override void ToggleEnabled()
        {
            base.ToggleEnabled();
            View.ViewCanvas.worldCamera = CameraManager.CurrentActiveCamera;
            SetUpCells();
        }

        private void ExitInventory(object sender, EventArgs e)
        {
            SetEnabled(false);
        }

        private void AddCell(string cellId, int index, GameObject item)
        {
            GameObject cell = View.AddUIElementFromResources<GameObject>("Cell", cellId, ContentTransform);
            cell.transform.SetParent(ContentTransform);
            Transform itemHolder = cell.FindChildRecursive("Content").transform;
            item.transform.SetParent(itemHolder);
            item.GetComponent<MeshRenderer>().receiveShadows = false;
            ItemInfo info = item.GetComponent<Item>().Info;
            item.transform.localScale = info.UIScale;
            item.transform.localEulerAngles = info.UIRotation;
            item.transform.localPosition = info.UIPosition;
            AddOrUpdate(index, cell);
        }

        private void AddOrUpdate(int index, GameObject go)
        {
            if (cellDictionary.ContainsKey(index))
            {
                cellDictionary[index] = go;
            }
            else
            {
                cellDictionary.Add(index, go);
            }
        }

        private void SetUpCells()
        {
            if (IsEnabled)
            {
                MainThreadDispatcher.ScheduleCoroutine(SetUpCellsCoroutine());
            }
        }

        private IEnumerator SetUpCellsCoroutine()
        {
            yield return new WaitForEndOfFrame();
            float cellWidth = 200; //TODO: get by call
            int totalColumns = (int)(ContentTransform.rect.width / cellWidth);
            foreach (KeyValuePair<int, GameObject> keyValuePair in cellDictionary)
            {
                int index = keyValuePair.Key;
                GameObject cell = keyValuePair.Value;
                int x = index % totalColumns;
                int y = index / totalColumns;
                cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(x * cellWidth, -y * cellWidth);
                yield return null;
            }
        }
        private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            switch (e.Action)
            {
                case InventoryChangedAction.Add:
                    AddCell($"Cell-{e.Index}", e.Index, e.Item);
                    break;
                case InventoryChangedAction.Remove:
                    MainThreadDispatcher.Destroy(cellDictionary[e.Index]);
                    cellDictionary.Remove(e.Index);
                    break;
                case InventoryChangedAction.RemoveAt:
                    break;
                case InventoryChangedAction.Insert:
                    break;
                case InventoryChangedAction.Replace:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SetUpCells();
        }
    }
}