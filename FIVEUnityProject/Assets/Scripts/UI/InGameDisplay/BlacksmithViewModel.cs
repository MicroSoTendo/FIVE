using FIVE.CameraSystem;
using FIVE.EventSystem;
using FIVE.Interactive;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static FIVE.EventSystem.MainThreadDispatcher;
using Object = UnityEngine.Object;

namespace FIVE.UI.InGameDisplay
{
    public class BlacksmithViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/Blacksmith/Blacksmith";
        private GameObject CellPrefab { get; } = Resources.Load<GameObject>("EntityPrefabs/UI/Inventory/Cell");
        protected override RenderMode ViewModelRenderMode { get; } = RenderMode.ScreenSpaceCamera;
        public GameObject InventoryContent { get; }
        private RectTransform InventoryContentTransform { get; }
        public GameObject CompositeContent { get; }
        private RectTransform SynthesisContentTransform { get; }
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
                }
            }
        }

        private readonly Dictionary<Item, Item> item2CopyItems = new Dictionary<Item, Item>();
        private readonly Dictionary<Item, Item> copyItems2Items = new Dictionary<Item, Item>();
        private readonly ObservableCollection<Cell> inventoryCells =  new ObservableCollection<Cell>();
        private readonly HashSet<Item> inventoryItems =  new HashSet<Item>();
        private readonly HashSet<Cell> synthesisCells = new HashSet<Cell>();
        private readonly HashSet<Item> synthesisItems =  new HashSet<Item>();

        public BlacksmithViewModel()
        {
            BackButton = Get<Button>(nameof(BackButton));
            Bind(BackButton).To(OnBackButtonClick);
            BuildButton = Get<Button>(nameof(BuildButton));
            Bind(BuildButton).To(OnBuildButtonClick);
            InventoryContent = Get(nameof(InventoryContent));
            InventoryContentTransform = InventoryContent.GetComponent<RectTransform>();
            CompositeContent = Get(nameof(CompositeContent));
            SynthesisContentTransform = CompositeContent.GetComponent<RectTransform>();
            ResultPanel = Get(nameof(ResultPanel));
            EventManager.Subscribe<OnInventoryChanged, NotifyCollectionChangedEventArgs>(OnInventoryChanged);
        }

        private void OnInventoryChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScheduleCoroutine(UpdateItems());
        }

        private void OnBuildButtonClick()
        {
            var cell2Items = synthesisCells.ToDictionary(compositeCell => compositeCell, compositeCell => compositeCell.Item.gameObject);

            if (Blacksmith.TrySynthesize(cell2Items.Values, out GameObject newItemPrefab))
            {
                ObservableCollection<Item> inventory = InventoryManager.Inventory.Items;
                GameObject newItem = Object.Instantiate(newItemPrefab);
                Item item = newItem.GetComponent<Item>();
                inventory.Add(item);
                foreach (Cell compositeCell in synthesisCells)
                {
                    Item original = copyItems2Items[compositeCell.Item];
                    inventory.Remove(original);
                }
                synthesisCells.Clear();
                synthesisItems.Clear();
                UpdateSynthesisCells();
                UpdateInventoryCells();
            }
        }

        private void OnBackButtonClick()
        {
            IsActive = false;
            synthesisCells.Clear();
        }

        private void UpdateInventoryCells()
        {
            foreach (Cell inventoryCell in inventoryCells)
            {
                inventoryCell.SetUpPosition();
            }
        }

        private void UpdateSynthesisCells()
        {
            foreach (Cell synthesisCell in synthesisCells)
            {
                synthesisCell.SetUpPosition();
            }
        }

        private IEnumerator UpdateItems()
        {
            ObservableCollection<Item> items = InventoryManager.Inventory.Items;
            for (int i = 0, j = 0; i < items.Count; i++)
            {
                Item originalItem = items[i];
                if (!item2CopyItems.ContainsKey(originalItem))
                {
                    GameObject copy = Object.Instantiate(originalItem.gameObject);
                    item2CopyItems.Add(originalItem, copy.GetComponent<Item>());
                }
                if (item2CopyItems.TryGetValue(originalItem, out Item copyItem))
                {
                    //Do not show if it's in synthesis window
                    if (!synthesisItems.Contains(copyItem))
                    {
                        if (inventoryCells.Count > j)
                        {
                            Cell cell = inventoryCells[j];
                            cell.Index = j;
                            cell.SetItem(copyItem);
                        }
                        else
                        {
                            GameObject cellGo = Object.Instantiate(CellPrefab, InventoryContentTransform);
                            Cell newCell = cellGo.GetComponent<Cell>();
                            inventoryCells.Add(newCell);
                            newCell.Index = j;
                            newCell.Clicked += () => OnInventoryCellClicked(newCell);
                            newCell.SetItem(copyItem);
                        }
                        j++;
                    }
                }
                yield return new WaitForFixedUpdate();
            }
        }

        private void OnInventoryCellClicked(Cell cell)
        {
            cell.transform.SetParent(SynthesisContentTransform);
            inventoryCells.Remove(cell);
            inventoryItems.Remove(cell.Item);
            synthesisCells.Add(cell);
            synthesisItems.Add(cell.Item);
            UpdateSynthesisCells();
            UpdateInventoryCells();
            cell.Clicked = null;
            cell.Clicked = () => OnSynthesisCellClicked(cell);
        }

        private void OnSynthesisCellClicked(Cell cell)
        {
            cell.transform.SetParent(InventoryContentTransform);
            synthesisCells.Remove(cell);
            synthesisItems.Remove(cell.Item);
            inventoryCells.Add(cell);
            inventoryItems.Add(cell.Item);
            UpdateSynthesisCells();
            UpdateInventoryCells();
            cell.Clicked = null;
            cell.Clicked = () => OnInventoryCellClicked(cell);
        }
    }
}