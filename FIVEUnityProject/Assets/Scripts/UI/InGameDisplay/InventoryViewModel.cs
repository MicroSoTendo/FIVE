using FIVE.CameraSystem;
using FIVE.EventSystem;
using FIVE.Interactive;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using static FIVE.EventSystem.MainThreadDispatcher;
using Object = UnityEngine.Object;

namespace FIVE.UI.InGameDisplay
{
    internal class InventoryViewModel : ViewModel
    {
        private readonly ObservableCollection<Cell> cells = new ObservableCollection<Cell>();
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/Inventory/InventoryScrollView";
        protected override RenderMode ViewModelRenderMode { get; } = RenderMode.ScreenSpaceCamera;
        private GameObject CellPrefab { get; } = Resources.Load<GameObject>("EntityPrefabs/UI/Inventory/Cell");
        private RectTransform ContentRectTransform { get; }
        public GameObject InventoryContent { get; }
        public Button ExitButton { get; }

        public override bool IsActive
        {
            get => base.IsActive;
            set
            {
                base.IsActive = value;
                if (value)
                {
                    this[RenderMode.ScreenSpaceCamera].worldCamera = CameraManager.CurrentActiveCamera;
                    foreach (Cell cell in cells)
                    {
                        cell.SetUpPosition();
                    }
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

        private IEnumerator UpdateItems()
        {
            ObservableCollection<Item> items = InventoryManager.Inventory.Items;
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                if (cells.Count > i)
                {
                    Cell cell = cells[i];
                    cell.Index = i;
                    cell.SetItem(item);
                }
                else
                {
                    GameObject cellGo = Object.Instantiate(CellPrefab, ContentRectTransform);
                    Cell newCell = cellGo.GetComponent<Cell>();
                    newCell.Index = i;
                    newCell.Clicked += () => cells.Remove(newCell);
                    newCell.SetItem(items[i]);
                    cells.Add(newCell);
                }

                yield return new WaitForFixedUpdate();
            }

            if (IsActive)
            {
                foreach (Cell cell in cells)
                {
                    cell.SetUpPosition();
                }
            }
        }

        private void OnInventoryChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScheduleCoroutine(UpdateItems());
        }
    }
}