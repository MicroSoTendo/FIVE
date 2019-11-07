using FIVE.Interactive;
using FIVE.Interactive.Blacksmith;
using FIVE.Robot;
using System;
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
        public Button Result { get; }
        public Button BackButton { get; }
        private int[] emptyComposites;
        private int[] emptyInventory;
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
            emptyInventory = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            foreach (Button button in Composite.GetComponentsInChildren<Button>())
            {
                compositeButtons.Add(button);
                Bind(button).To(() => OnCompositeButtonClicked(button));
            }
            BackButton = Get<Button>(nameof(BackButton));
            Bind(BackButton).To(OnBackButtonClick);
            Result = Get<Button>(nameof(Result));
            Bind(Result).To(() =>OnResultButtonClick(Result));
        }

        private void OnResultButtonClick(Button button)
        {
            Inventory inventory = InventoryManager.GetInventory(RobotManager.ActiveRobot);
            
            if (button.transform.childCount != 0)
            {
                GameObject resultItem = button.transform.GetChild(0).gameObject;

                inventory.Add(resultItem);
                foreach (Button a in compositeButtons)
                {
                    Blacksmith.RemoveFromComposite(RobotManager.ActiveRobot, a.transform.GetChild(0).gameObject);
                    a.transform.GetChild(0).gameObject.SetActive(false);
                    GameObject.Destroy(a.transform.GetChild(0).gameObject);

                }
                //hard coding
                for(int i = 0; i< 3; i++)
                {   
                        inventory.RemoveAt(0); 
                }
                for(int i = 0; i < emptyInventory.Length; i++)
                {
                    if(emptyInventory[i] == 0)
                    {
                        resultItem.transform.SetParent(inventoryButtons[i].transform);
                        resultItem.SetActive(true);
                        break;
                    }
                    
                }
                
                Blacksmith.RemoveResultItem(RobotManager.ActiveRobot);
                ResetComposite();
                UpdateInventory();
            }
        }

        private void OnCompositeButtonClicked(Button button)
        {
            if(Result.transform.childCount != 0)
            {
                Blacksmith.RemoveResultItem(RobotManager.ActiveRobot);
                GameObject a = Result.transform.GetChild(0).gameObject;
                a.SetActive(false);
                GameObject.Destroy(a);
            }
            if (button.transform.childCount == 0)
            {
                return;
            }
            for(int i = 0; i < emptyComposites.Length; i++)
            {
                if(compositeButtons[i] == button)
                {
                    emptyComposites[i] = 0;
                }
            }
            GameObject item = button.transform.GetChild(0).gameObject;
            int count = 0;
            while(count < inventoryButtons.Count)
            {
                if(emptyInventory[count] == 0)
                {
                    item.SetParent(inventoryButtons[count].transform);
                    Blacksmith.RemoveFromComposite(RobotManager.ActiveRobot, item);

                    if (item.name.Contains("Battery"))
                    {
                        item.transform.localScale = new Vector3(7, 7, 7);
                        item.transform.localPosition = new Vector3(0, -37, 0);
                    }
                    if (item.name.Contains("Solar"))
                    {
                        item.transform.localPosition = new Vector3(-79, -1, -146);
                    }
                    emptyInventory[count] = 1;
                    break;
                }
                count++;
            }

        }

        private void OnInventoryItemClicked(Button button)
        {
            if (button.transform.childCount == 0)
            {
                return;
            }
            for (int i = 0; i < emptyInventory.Length; i++)
            {
                if (inventoryButtons[i] == button)
                {
                    emptyInventory[i] = 0;
                }
            }
            GameObject item = button.transform.GetChild(0).gameObject;
            Blacksmith.AddForComposite(RobotManager.ActiveRobot, item);
            int count = 0; ;
            while (count < compositeButtons.Count)
            {
                if (emptyComposites[count] == 0)
                {
                    item.SetParent(compositeButtons[count].transform);
                    if (item.name.Contains("Battery"))
                    {
                        item.transform.localScale = new Vector3(7, 7, 7);
                        item.transform.localPosition = new Vector3(0, -37, 0);
                    }
                    if (item.name.Contains("Solar"))
                    {
                        item.transform.localPosition = new Vector3(-70, -49, -146);
                    }
                    emptyComposites[count] = 1;
                    break;
                }
                count++;
            }
            bool isFull = true;
            foreach(int a in emptyComposites)
            {
                if(a == 0)
                {
                    isFull = false;
                }
            }
            if (isFull)
            {
                GameObject resultItem = GameObject.Instantiate(Blacksmith.GenerateResultItems(), Result.transform);
                resultItem.transform.localPosition = new Vector3(-27.4f, -11.9f, -29.8f);
                resultItem.transform.localScale = new Vector3(20, 20,20);
            }
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
            emptyComposites = new int[3] { 0, 0, 0 };
            
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
                    go.SetActive(true);
                    var set = GameObject.Instantiate(go, inventoryButtons[i].transform);

                    if (go.name.Contains("Battery"))
                    {
                        set.transform.localScale = new Vector3(7, 7, 7);
                        set.transform.localPosition = new Vector3(0, -37, 0);
                    }
                    if (go.name.Contains("Solar"))
                    {
                        set.transform.localPosition = new Vector3(-79, -1, -146);
                    }
                    
                }
            }
            emptyInventory = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }

        private void OnBackButtonClick()
        {
            ResetComposite();
            IsActive = false;
            Blacksmith.ResetList(RobotManager.ActiveRobot);
        }
    }
}