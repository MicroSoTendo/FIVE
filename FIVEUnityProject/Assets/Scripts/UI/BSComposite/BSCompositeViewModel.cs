using FIVE.Interactive;
using FIVE.Interactive.Blacksmith;
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

        private List<Button> inventoryButtons;
        private List<Button> inButtons;

        public override bool IsActive
        {
            get => base.IsActive;
            set
            {
                if (value)
                {
                    ResetLocalInventory();
                    ResetIn();
                }
                base.IsActive = value;
            }
        }

        private bool IsCompositeEmpty(int i)
        {
            return inButtons[i].transform.childCount == 0;
        }

        private bool IsInventoryEmpty(int i)
        {
            return inventoryButtons[i].transform.childCount == 0;
        }

        private GameObject ItemOut => Result.transform.childCount > 0 ? Result.transform.GetChild(0).gameObject : null;

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
            inButtons = new List<Button>();
            Composite = Get(nameof(Composite));

            foreach (Button button in Composite.GetComponentsInChildren<Button>())
            {
                inButtons.Add(button);
                Bind(button).To(() => OnCompositeButtonClicked(button));
            }
            BackButton = Get<Button>(nameof(BackButton));
            Bind(BackButton).To(OnBackButtonClick);
            Result = Get<Button>(nameof(Result));
            Bind(Result).To(OnResultButtonClick);
        }

        private void OnResultButtonClick()
        {
            if (ItemOut != null)
            {
                InventoryManager.Inventory.Add(ItemOut);

                // FIXME: remove items in inventory
                ResetIn();

                ResetLocalInventory();
            }
        }

        private void OnCompositeButtonClicked(Button button)
        {
            if (button.transform.childCount == 0)
            {
                return;
            }

            if (ItemOut != null)
            {
                RemoveChild0(Result);
            }

            GameObject item = button.transform.GetChild(0).gameObject;

            for (int i = 0; i < inventoryButtons.Count; i++)
            {
                if (IsInventoryEmpty(i))
                {
                    item.SetParent(inventoryButtons[i].transform);
                    Blacksmith.RemoveIn(item);

                    if (item.name.Contains("Battery"))
                    {
                        item.transform.localScale = new Vector3(7, 7, 7);
                        item.transform.localPosition = new Vector3(0, -37, 0);
                    }
                    if (item.name.Contains("Solar"))
                    {
                        item.transform.localPosition = new Vector3(-79, -1, -146);
                    }

                    break;
                }
            }
        }

        private void OnInventoryItemClicked(Button button)
        {
            if (button.transform.childCount == 0)
            {
                return;
            }

            GameObject item = button.transform.GetChild(0).gameObject;
            for (int i = 0; i < inButtons.Count; i++)
            {
                if (IsCompositeEmpty(i))
                {
                    item.SetParent(inButtons[i].transform);
                    if (item.name.Contains("Battery"))
                    {
                        item.transform.localScale = new Vector3(7, 7, 7);
                        item.transform.localPosition = new Vector3(0, -37, 0);
                    }
                    if (item.name.Contains("Solar"))
                    {
                        item.transform.localPosition = new Vector3(-70, -49, -146);
                    }
                    break;
                }
            }
            Blacksmith.AddIn(item);

            bool isFull = true;
            for (int i = 0; i < inButtons.Count; i++)
            {
                if (IsCompositeEmpty(i))
                {
                    isFull = false;
                    break;
                }
            }
            if (isFull)
            {
                GameObject resultItem = Object.Instantiate(Blacksmith.GenerateOut(), Result.transform);
                resultItem.transform.localPosition = new Vector3(-27.4f, -11.9f, -29.8f);
                resultItem.transform.localScale = new Vector3(20, 20, 20);
            }
        }

        private void ResetIn()
        {
            ResetButtons(inButtons);
            Blacksmith.ResetIn();
        }

        private void ResetLocalInventory()
        {
            ResetButtons(inventoryButtons);

            for (int i = 0; i < InventoryManager.Inventory.Count; i++)
            {
                GameObject go = InventoryManager.Inventory.Items[i];
                go.SetActive(true);
                GameObject gonew = Object.Instantiate(go, inventoryButtons[i].transform);

                if (go.name.Contains("Battery"))
                {
                    gonew.transform.localScale = new Vector3(7, 7, 7);
                    gonew.transform.localPosition = new Vector3(0, -37, 0);
                }
                if (go.name.Contains("Solar"))
                {
                    gonew.transform.localPosition = new Vector3(-79, -1, -146);
                }
            }
        }

        private void OnBackButtonClick()
        {
            ResetIn();
            IsActive = false;
        }

        private void RemoveChild0(Component o)
        {
            GameObject c = o.transform.GetChild(0).gameObject;
            c.SetActive(false);
            Object.Destroy(c);
            o.transform.DetachChildren();
        }

        private void ResetButtons(List<Button> l)
        {
            foreach (Button button in l)
            {
                if (button.transform.childCount > 0)
                {
                    RemoveChild0(button);
                }
            }
        }
    }
}