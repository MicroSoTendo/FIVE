using FIVE.CameraSystem;
using FIVE.EventSystem;
using FIVE.Interactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using static FIVE.EventSystem.MainThreadDispatcher;
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

        private readonly object poolLock = new object();
        private readonly List<(GameObject, Transform)> cellPool = new List<(GameObject, Transform)>();

        public override bool IsActive
        {
            get => base.IsActive;
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
            contentRectTransform = InventoryContent.GetComponent<RectTransform>();
            this[RenderMode.ScreenSpaceCamera].worldCamera = Camera.current;
            this[RenderMode.ScreenSpaceCamera].planeDistance = 0.5f;
            EventManager.Subscribe<OnInventoryChanged, NotifyCollectionChangedEventArgs>(OnInventoryChanged);
            ScheduleCoroutine(UpdateCellPool());
        }

        public IEnumerator UpdateCellPool()
        {
            while (true)
            {
                while (cellDictionary.Count >= cellPool.Count)
                {
                    GameObject cell0 = Object.Instantiate(CellPrefab, contentRectTransform);
                    GameObject cell1 = Object.Instantiate(CellPrefab, contentRectTransform);
                    cell0.name = $"Cell-{cellPool.Count}";
                    cell1.name = $"Cell-{cellPool.Count + 1}";
                    cell0.SetActive(false);
                    cell1.SetActive(false);
                    Transform itemHolder0 = cell0.FindChildRecursive("Content").transform;
                    Transform itemHolder1 = cell1.FindChildRecursive("Content").transform;
                    cellPool.Add((cell0, itemHolder0));
                    cellPool.Add((cell1, itemHolder1));
                }
                yield return null;
            }
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

        private void DoAddCell(int index, GameObject item)
        {
            IEnumerator Routine()
            {
                while (index > cellPool.Count)
                {
                    yield return null;
                }

                (GameObject cell, Transform itemHolder) = cellPool[index];
                item.transform.SetParent(itemHolder);
                item.GetComponent<MeshRenderer>().receiveShadows = false;
                ItemInfo info = item.GetComponent<Item>().Info;
                item.transform.localScale = info.UIScale;
                item.transform.localEulerAngles = info.UIRotation;
                item.transform.localPosition = info.UIPosition;
                cell.SetActive(true);
                AddOrUpdate(index, cell);
            }
            ScheduleCoroutine(Routine());
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
                ScheduleCoroutine(SetUpCellsCoroutine());
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
                cell.GetComponent<RectTransform>().anchoredPosition = 
                    new Vector2(x * cellWidth, -y * cellWidth);
                yield return null;
            }
        }

        private void OnInventoryChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    DoAddCell(e.NewStartingIndex, e.NewItems[0] as GameObject);
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    GameObject go = cellDictionary[e.OldStartingIndex];
                    cellDictionary.Remove(e.OldStartingIndex);
                    Destroy(go);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SetUpCells();
        }
    }
}