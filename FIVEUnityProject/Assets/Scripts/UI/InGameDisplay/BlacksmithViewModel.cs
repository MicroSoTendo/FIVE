using FIVE.Interactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using FIVE.CameraSystem;
using FIVE.EventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using static FIVE.EventSystem.MainThreadDispatcher;

namespace FIVE.UI.InGameDisplay
{
    ///*
    public class BlacksmithViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/Blacksmith/Blacksmith";
        private GameObject CellPrefab { get; } = Resources.Load<GameObject>("EntityPrefabs/UI/Inventory/Cell");
        protected override RenderMode ViewModelRenderMode { get; } = RenderMode.ScreenSpaceCamera;
        public GameObject InventoryContent { get; }
        private RectTransform InventoryContentTransform { get; }
        public GameObject CompositeContent { get; }
        private RectTransform CompositeContentTransform { get; }
        public GameObject ResultPanel { get; }
        public Button BuildButton { get; }
        public Button BackButton { get; }

        public override bool IsActive
        {
            get => base.IsActive;
            set
            {
                base.IsActive = value;
                if (value)
                {
                    this[RenderMode.ScreenSpaceCamera].worldCamera = CameraManager.CurrentActiveCamera;
                    ScheduleCoroutine(SetUpInventoryItems());
                }
            }
        }

        private readonly List<(GameObject cell, Transform content)> inventoryCells = new List<(GameObject cell, Transform content)>();
        private readonly List<GameObject> copyItems = new List<GameObject>();
        private readonly HashSet<GameObject> compositeItems = new HashSet<GameObject>();

        public BlacksmithViewModel()
        {
            BackButton = Get<Button>(nameof(BackButton));
            Bind(BackButton).To(OnBackButtonClick);
            BuildButton = Get<Button>(nameof(BuildButton));
            Bind(BuildButton).To(OnBuildButtonClick);
            InventoryContent = Get(nameof(InventoryContent));
            InventoryContentTransform = InventoryContent.GetComponent<RectTransform>();
            CompositeContent = Get(nameof(CompositeContent));
            CompositeContentTransform = CompositeContent.GetComponent<RectTransform>();
            ResultPanel = Get(nameof(ResultPanel));
            EventManager.Subscribe<OnInventoryChanged, NotifyCollectionChangedEventArgs>(OnInventoryChanged);
        }

        private IEnumerator MakeCopy()
        {
            ObservableCollection<GameObject> items = InventoryManager.Inventory.Items;
            for (int i = 0; i < copyItems.Count; i++)
            {
                if (i >= items.Count)
                {
                    Destroy(copyItems[i]);
                }
                else
                {
                    GameObject copy = copyItems[i];
                    GameObject original = items[i];
                    if (copy.GetComponent<Item>().Info.Name != original.GetComponent<Item>().Info.Name)
                    {
                        copyItems[i] = Object.Instantiate(original, InventoryContentTransform);
                    }
                }
                yield return new WaitForFixedUpdate();
            }
            if (IsActive)
            {
                ScheduleCoroutine(SetUpInventoryItems());
            }
        }

        private void OnInventoryChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScheduleCoroutine(MakeCopy());
        }

        private void OnBuildButtonClick()
        {

        }

        private void OnBackButtonClick()
        {
            IsActive = false;
        }

        private IEnumerator SetUpCompositeItems(GameObject cell)
        {
            yield return new WaitForEndOfFrame();
            cell.SetParent(CompositeContentTransform);
            float cellSize = 180; //TODO: get by call
            cell.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(0,  inventoryCells.Count * cellSize);
            compositeItems.Add(cell);
        }
        private IEnumerator SetUpInventoryItems()
        {
            //Ensure size
            yield return new WaitForEndOfFrame();
            float cellSize = 180; //TODO: get by call
            int totalColumns = (int)(InventoryContentTransform.rect.width / cellSize);
            while (copyItems.Count > inventoryCells.Count)
            {
                GameObject cell = Object.Instantiate(CellPrefab, InventoryContentTransform);
                Button button = cell.GetComponentInChildren<Button>();
                int i = inventoryCells.Count;
                cell.name = $"Cell-{i}";
                int x = i % totalColumns;
                int y = i / totalColumns;
                cell.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(x * cellSize, -y * cellSize);
                Transform contenTransform = cell.FindChildRecursive("Content").transform;
                inventoryCells.Add((cell, contenTransform));
                button.onClick.AddListener(()=>OnInventoryItemClicked(cell));
                yield return new WaitForFixedUpdate();
            }
            //
            for (int i = 0; i < inventoryCells.Count; i++)
            {
                (GameObject cell, Transform content) = inventoryCells[i];
                if (i >= copyItems.Count)
                {
                    cell.SetActive(false);
                }
                else
                {
                    GameObject item = copyItems[i];
                    item.SetParent(content);
                    item.GetComponent<MeshRenderer>().receiveShadows = false;
                    ItemInfo info = item.GetComponent<Item>().Info;
                    item.transform.localScale = info.UIScale;
                    item.transform.localEulerAngles = info.UIRotation;
                    item.transform.localPosition = info.UIPosition;
                    cell.SetActive(true);
                }
                yield return new WaitForFixedUpdate();
            }
        }


        private void OnInventoryItemClicked(GameObject cell)
        {
            ScheduleCoroutine(SetUpCompositeItems(cell));
        }

    }
}