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
                    ScheduleCoroutine(UpdateItems());
                }
            }
        }

        private readonly List<(GameObject cell, Transform content)> inventoryCells = new List<(GameObject cell, Transform content)>();
        private readonly List<GameObject> copyItems = new List<GameObject>();
        private readonly Dictionary<GameObject, GameObject> copyItemsMap = new Dictionary<GameObject, GameObject>();
        private readonly HashSet<GameObject> compositeCells = new HashSet<GameObject>();

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

        private IEnumerator SetCompositeCells()
        {
            yield return new WaitForEndOfFrame();
            int i = 0;
            float cellWidth = 180; //TODO: get by call
            foreach (GameObject cell in compositeCells)
            {
                int y = i;
                cell.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(0, -y * cellWidth);
                i++;
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator SetInventoryCells()
        {
            yield return new WaitForEndOfFrame();
            int i = 0;
            float cellWidth = 180; //TODO: get by call
            int totalColumns = (int)(InventoryContentTransform.rect.width / cellWidth);
            foreach ((GameObject cell, Transform content) in inventoryCells)
            {
                if (!compositeCells.Contains(cell))
                {
                    int x = i % totalColumns;
                    int y = i / totalColumns;
                    cell.GetComponent<RectTransform>().anchoredPosition =
                        new Vector2(x * cellWidth, -y * cellWidth);
                    i++;
                }
                yield return new WaitForFixedUpdate();
            }
        }

        private void OnInventoryChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScheduleCoroutine(UpdateItems());
        }

        private void OnBuildButtonClick()
        {
            var list = compositeCells.Select(compositeItem => compositeItem.FindChildRecursive("Content").transform.GetChild(0).gameObject).ToList();

            if (Blacksmith.TryGenerateItem(list, out GameObject newGoPrefab))
            {
                GameObject newItem = Object.Instantiate(newGoPrefab);
                foreach (GameObject item in list)
                {
                    GameObject origin = copyItemsMap[item];
                    InventoryManager.Inventory.Items.Remove(origin);
                }
                compositeCells.Clear();
                this.RaiseImmediate<OnDropItemToInventory>(new DropedItemToInventoryEventArgs(null, newItem));
            }
        }

        private void OnBackButtonClick()
        {
            IsActive = false;
            compositeCells.Clear();
        }

        private IEnumerator AddCompositeItems(GameObject cell)
        {
            yield return new WaitForEndOfFrame();
            cell.SetParent(CompositeContentTransform);
            float cellSize = 180; //TODO: get by call
            cell.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(0, inventoryCells.Count * cellSize);
            compositeCells.Add(cell);
            ScheduleCoroutine(SetInventoryCells());
            ScheduleCoroutine(SetCompositeCells());
        }

        private IEnumerator UpdateItems()
        {
            for (int i = 0; i < InventoryManager.Inventory.Items.Count; i++)
            {
                GameObject original = InventoryManager.Inventory.Items[i];
                if (i < copyItems.Count)
                {
                    GameObject copy = copyItems[i];
                    if (copy.GetComponent<Item>().Info.Name != original.GetComponent<Item>().Info.Name)
                    {
                        if (copyItemsMap.ContainsKey(copy))
                        {
                            copyItemsMap.Remove(copy);
                        }
                        copyItems[i] = Object.Instantiate(original);
                        copyItemsMap.Add(copy, original);
                    }
                }
                else
                {
                    GameObject copy = Object.Instantiate(original);
                    copyItems.Add(copy);
                    copyItemsMap.Add(copy, original);
                }
                yield return new WaitForFixedUpdate();
            }
            //Ensure size
            while (copyItems.Count > inventoryCells.Count)
            {
                GameObject cell = Object.Instantiate(CellPrefab, InventoryContentTransform);
                int i = inventoryCells.Count;
                cell.name = $"Cell-{i}";
                Transform contenTransform = cell.FindChildRecursive("Content").transform;
                inventoryCells.Add((cell, contenTransform));

                Button button = cell.GetComponentInChildren<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnInventoryItemClicked(cell));
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
                ScheduleCoroutine(SetInventoryCells());
            }
        }


        private void OnInventoryItemClicked(GameObject cell)
        {
            ScheduleCoroutine(AddCompositeItems(cell));
        }

    }
}