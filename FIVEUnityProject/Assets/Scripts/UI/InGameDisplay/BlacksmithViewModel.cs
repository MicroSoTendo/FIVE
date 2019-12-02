using FIVE.CameraSystem;
using FIVE.EventSystem;
using FIVE.Interactive;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
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
        public GameObject SynthesisContent { get; }
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
                else
                {
                    synthesisItems.Clear();
                    synthesisCells.Clear();
                }
            }
        }

        private readonly Dictionary<Item, Item> item2CopyItems = new Dictionary<Item, Item>();
        private readonly Dictionary<Item, Item> copyItems2Items = new Dictionary<Item, Item>();

        private readonly ObservableCollection<Cell> inventoryCells = new ObservableCollection<Cell>();
        private readonly HashSet<Item> inventoryItems = new HashSet<Item>();
        private readonly HashSet<Cell> synthesisCells = new HashSet<Cell>();
        private readonly HashSet<Item> synthesisItems = new HashSet<Item>();

        public BlacksmithViewModel()
        {
            BackButton = Get<Button>(nameof(BackButton));
            BuildButton = Get<Button>(nameof(BuildButton));
            InventoryContent = Get(nameof(InventoryContent));
            SynthesisContent = Get(nameof(SynthesisContent));
            ResultPanel = Get(nameof(ResultPanel));

            Bind(BackButton).To(OnBackButtonClick);
            Bind(BuildButton).To(OnBuildButtonClick);
            InventoryContentTransform = InventoryContent.GetComponent<RectTransform>();
            SynthesisContentTransform = SynthesisContent.GetComponent<RectTransform>();
            EventManager.Subscribe<OnInventoryChanged, NotifyCollectionChangedEventArgs>(OnInventoryChanged);
        }

        private void OnInventoryChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScheduleCoroutine(UpdateItems());
        }

        private void OnBuildButtonClick()
        {
            var cell2Items = synthesisCells.ToDictionary(synthesisCell => synthesisCell, synthesisCell => synthesisCell.Item.gameObject);

            if (Blacksmith.TrySynthesize(cell2Items.Values, out GameObject newItemPrefab))
            {
                ObservableCollection<Item> inventory = InventoryManager.Inventory.Items;
                //Add new item to inventory
                GameObject newItem = Object.Instantiate(newItemPrefab);
                Item item = newItem.GetComponent<Item>();
                inventory.Add(item);
                //Destory source objects
                foreach (Cell synthesisCell in synthesisCells)
                {
                    Item original = copyItems2Items[synthesisCell.Item];
                    inventory.Remove(original);
                    original.Destroy();
                    synthesisCell.Destroy();
                }
                synthesisCells.Clear();
                synthesisItems.Clear();
            }
        }

        private void OnBackButtonClick()
        {
            IsActive = false;
            synthesisCells.Clear();
        }

        private IEnumerator UpdateItems()
        {
            ObservableCollection<Item> items = InventoryManager.Inventory.Items;
            int j = 0;
            foreach (Item originalItem in items)
            {
                //Make copy first if it hasn't
                Item copyItem;
                if (!item2CopyItems.ContainsKey(originalItem))
                {
                    copyItem = Object.Instantiate(originalItem.gameObject).GetComponent<Item>();
                    item2CopyItems.Add(originalItem, copyItem);
                    copyItems2Items.Add(copyItem, originalItem);
                }
                else
                {
                    copyItem = item2CopyItems[originalItem];
                }
                //Do not show if it's in synthesis window
                if (!synthesisItems.Contains(copyItem))
                {
                    Cell cell;
                    if (inventoryCells.Count > j)
                    {
                        cell = inventoryCells[j];
                    }
                    else
                    {
                        cell = Object.Instantiate(CellPrefab, InventoryContentTransform).GetComponent<Cell>();
                        inventoryCells.Add(cell);
                        inventoryItems.Add(copyItem);
                    }
                    cell.gameObject.SetActive(true);
                    cell.SetItem(copyItem);
                    cell.Clicked = () => OnInventoryCellClicked(cell);
                    j++;
                }
                yield return new WaitForFixedUpdate();
            }
            //Hide redundnat cells
            for (int i = j; i < inventoryCells.Count; i++)
            {
                inventoryCells[i].gameObject.SetActive(false);
            }
        }

        private void OnInventoryCellClicked(Cell cell)
        {
            cell.transform.SetParent(SynthesisContentTransform);
            inventoryCells.Remove(cell);
            inventoryItems.Remove(cell.Item);
            synthesisCells.Add(cell);
            synthesisItems.Add(cell.Item);
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
            cell.Clicked = null;
            cell.Clicked = () => OnInventoryCellClicked(cell);
        }
    }
}