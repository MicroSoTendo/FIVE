using FIVE.CameraSystem;
using FIVE.EventSystem;
using FIVE.Interactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using static FIVE.EventSystem.MainThreadDispatcher;

namespace FIVE.UI.InGameDisplay
{
    internal class InventoryViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/Inventory/InventoryScrollView";
        protected override RenderMode ViewModelRenderMode { get; } = RenderMode.ScreenSpaceCamera;
        private GameObject CellPrefab { get; } = Resources.Load<GameObject>("EntityPrefabs/UI/Inventory/Cell");
        private RectTransform ContentRectTransform { get; }
        public GameObject InventoryContent { get; }
        public Button ExitButton { get; }
        private readonly List<(GameObject cell, Transform content)> cells = new List<(GameObject cell, Transform content)>();
        public override bool IsActive
        {
            get => base.IsActive;
            set
            {
                base.IsActive = value;
                if (value)
                {
                    this[RenderMode.ScreenSpaceCamera].worldCamera = CameraManager.CurrentActiveCamera;
                    ScheduleCoroutine(SetUpCells());
                }
            }
        }

        public InventoryViewModel()
        {
            InventoryContent = Get(nameof(InventoryContent));
            ExitButton = Get<Button>(nameof(ExitButton));
            Bind(ExitButton).To(ExitInventory);
            ContentRectTransform = InventoryContent.GetComponent<RectTransform>();
            this[RenderMode.ScreenSpaceCamera].worldCamera = Camera.current;
            this[RenderMode.ScreenSpaceCamera].planeDistance = 0.5f;
            EventManager.Subscribe<OnInventoryChanged, NotifyCollectionChangedEventArgs>(OnInventoryChanged);
        }

        public override void ToggleEnabled()
        {
            base.ToggleEnabled();
            this[RenderMode.ScreenSpaceCamera].worldCamera = CameraManager.CurrentActiveCamera;
        }

        private void ExitInventory()
        {
            IsActive = false;
        }

        private IEnumerator SetUpCells()
        {
            yield return new WaitForEndOfFrame();
            int i = 0;
            float cellWidth = 180; //TODO: get by call
            int totalColumns = (int)(ContentRectTransform.rect.width / cellWidth);
            foreach ((GameObject cell, Transform content) in cells)
            {
                int x = i % totalColumns;
                int y = i / totalColumns;
                cell.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(x * cellWidth, -y * cellWidth);
                i++;
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator UpdateItems()
        {
            ObservableCollection<GameObject> items = InventoryManager.Inventory.Items;
            //Ensure size
            while (items.Count > cells.Count)
            {
                GameObject cell = Object.Instantiate(CellPrefab, ContentRectTransform);
                int i = cells.Count;
                cell.name = $"Cell-{i}";
                Transform contenTransform = cell.FindChildRecursive("Content").transform;
                cells.Add((cell, contenTransform));
                yield return new WaitForFixedUpdate();
            }
            //
            for (int i = 0; i < cells.Count; i++)
            {
                (GameObject cell, Transform content) = cells[i];
                if (i >= items.Count)
                {
                    cell.SetActive(false);
                }
                else
                {
                    GameObject item = items[i];
                    if (item.transform.parent == null ||
                        !item.transform.parent.gameObject.name.Contains("Content"))
                    {
                        item.transform.SetParent(content);
                        item.GetComponent<MeshRenderer>().receiveShadows = false;
                        ItemInfo info = item.GetComponent<Item>().Info;
                        item.transform.localScale = info.UIScale;
                        item.transform.localEulerAngles = info.UIRotation;
                        item.transform.localPosition = info.UIPosition;
                    }
                    else if (content.transform.parent != content)
                    {
                        item.transform.SetParent(content);
                    }
                }
                yield return new WaitForFixedUpdate();
            }
            if (IsActive)
            {
                ScheduleCoroutine(SetUpCells());
            }
        }

        private void OnInventoryChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScheduleCoroutine(UpdateItems());
        }
    }
}