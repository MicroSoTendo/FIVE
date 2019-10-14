using FIVE.CameraSystem;
using FIVE.EventSystem;
using FIVE.Interactive;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace FIVE.UI.InGameDisplay
{
    internal class InventoryViewModel : ViewModel
    {
        private readonly Dictionary<int, GameObject> cellDictionary = new Dictionary<int, GameObject>();
        private readonly RectTransform contentRectTransform;
        protected override RenderMode ViewModelRenderMode { get; } = RenderMode.ScreenSpaceCamera;
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/Inventory/InventoryScrollView";
        private GameObject CellPrefab { get; } = Resources.Load<GameObject>("EntityPrefabs/UI/Inventory/Cell");
        public GameObject InventoryContent { get; }
        public Button ExitButton { get; }
        public Inventory Inventory { get; set; }

        public override bool IsActive
        {
            get
            {
                return base.IsActive;
            }
            set
            {
                base.IsActive = value;
                if (value)
                {
                    this[RenderMode.ScreenSpaceCamera].worldCamera = CameraManager.CurrentActiveCamera;
                    SetUpCells();
                }
            }
        }

        public InventoryViewModel()
        {
            InventoryContent = Get(nameof(InventoryContent));
            ExitButton = Get<Button>(nameof(ExitButton));
            Bind(ExitButton).To(ExitInventory);

            EventManager.Subscribe<OnInventoryChanged, InventoryChangedEventArgs>(OnInventoryChanged);
            contentRectTransform = InventoryContent.GetComponent<RectTransform>();
            this[RenderMode.ScreenSpaceCamera].worldCamera = Camera.current;
            this[RenderMode.ScreenSpaceCamera].planeDistance = 0.5f;
        }

        public override void ToggleEnabled()
        {
            base.ToggleEnabled();
            this[RenderMode.ScreenSpaceCamera].worldCamera = CameraManager.CurrentActiveCamera;
            SetUpCells();
        }

        private void ExitInventory()
        {
            IsActive = false;
        }

        private void AddCell(string cellId, int index, GameObject item)
        {
            GameObject cell = Object.Instantiate(CellPrefab, contentRectTransform);
            cell.name = nameof(cell) + cellId;
            cell.transform.SetParent(contentRectTransform);
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
            if (IsActive)
            {
                MainThreadDispatcher.ScheduleCoroutine(SetUpCellsCoroutine());
            }
        }

        private IEnumerator SetUpCellsCoroutine()
        {
            yield return new WaitForEndOfFrame();
            float cellWidth = 200; //TODO: get by call
            int totalColumns = (int)(contentRectTransform.rect.width / cellWidth);
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