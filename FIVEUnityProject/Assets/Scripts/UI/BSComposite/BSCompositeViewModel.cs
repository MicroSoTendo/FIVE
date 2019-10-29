using FIVE.Interactive;
using FIVE.Interactive.Blacksmith;
using FIVE.Robot;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.BSComposite
{
    ///*
    public class BSCompositeViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/Shop/BSComposite";
        protected override RenderMode ViewModelRenderMode { get; } = RenderMode.ScreenSpaceCamera;
        public GameObject Inventory { get; }
        public GameObject Composite { get; }
        public Button BackButton { get; }
        private int[] emptyComposites;
        private List<Button> inventoryButtons;
        private List<Button> compositeButtons;
        public override bool IsActive
        {
            get => base.IsActive;
            set
            {
                base.IsActive = value;
                if (value)
                {
                    UpdateInventory();
                    ResetComposite();
                }
            }
        }

        public BSCompositeViewModel() : base()
        {
            inventoryButtons = new List<Button>();
            //Add inventory item buttons
            Inventory = Get(nameof(Inventory));
            foreach (Button button in Inventory.GetComponentsInChildren<Button>())
            {
                inventoryButtons.Add(button);
                Bind(button).To(() => OnInventoryItemClicked(button));
            }
            compositeButtons = new List<Button>();
            Composite = Get(nameof(Composite));
            emptyComposites = new int[3] { 0, 0, 0 };
            foreach (Button button in Composite.GetComponentsInChildren<Button>())
            {
                compositeButtons.Add(button);
                Bind(button).To(() => OnCompositeButtonClicked(button));
            }
            BackButton = Get<Button>(nameof(BackButton));
            Bind(BackButton).To(OnBackButtonClick);
        }

        private void OnCompositeButtonClicked(Button button)
        {
            if (button.transform.childCount == 0)
            {
                return;
            }
        }

        private void OnInventoryItemClicked(Button button)
        {
            if (button.transform.childCount == 0)
            {
                return;
            }

            GameObject item = button.transform.GetChild(0).gameObject;
            Blacksmith.AddForComposite(RobotManager.ActiveRobot, item);
            int count = 0; ;
            while (count < compositeButtons.Count)
            {
                if (emptyComposites[count] == 0)
                {
                    item.SetParent(compositeButtons[count].transform);
                    item.transform.localPosition = new Vector3(0, -37, 0);
                    item.transform.localScale = new Vector3(7, 7, 7);
                    break;
                }
            }
            Blacksmith.AddForComposite(RobotManager.ActiveRobot, item);
        }

        private void ResetComposite()
        {
            foreach (Button button in compositeButtons)
            {
                if (button.transform.childCount > 0)
                {
                    GameObject go = button.transform.GetChild(0).gameObject;
                    go.SetActive(false);
                    GameObject.Destroy(go);
                }
            }
        }
        private void UpdateInventory()
        {
            foreach (Button button in inventoryButtons)
            {
                if (button.transform.childCount > 0)
                {
                    GameObject go = button.transform.GetChild(0).gameObject;
                    go.SetActive(false);
                    GameObject.Destroy(go);
                }
            }
            Inventory inventory = InventoryManager.GetInventory(RobotManager.ActiveRobot);
            if (inventory != null)
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    GameObject go = inventory.Items[i];
                    var set = GameObject.Instantiate(go, inventoryButtons[i].transform);
                    set.transform.localScale = new Vector3(7, 7, 7);
                    set.transform.localPosition = new Vector3(0, -37, 0);
                }
            }
        }

        private void OnBackButtonClick()
        {
            ResetComposite();
            IsActive = false;
            Blacksmith.ResetList(RobotManager.ActiveRobot);
        }
    }
}