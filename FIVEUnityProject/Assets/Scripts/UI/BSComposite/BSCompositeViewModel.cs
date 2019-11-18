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
        public Button OutButton { get; }
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

        private List<GameObject> ItemsIn
        {
            get
            {
                var items = new List<GameObject>();
                foreach (Button b in inButtons)
                {
                    GameObject item = GetChild0(b);
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }
                return items;
            }
        }

        private GameObject ItemOut => GetChild0(OutButton);

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
                Bind(button).To(() => OnInItemClicked(button));
            }
            BackButton = Get<Button>("BackButton");
            Bind(BackButton).To(OnBackButtonClick);
            OutButton = Get<Button>("Result");
            Bind(OutButton).To(OnOutItemClick);
        }

        private void OnInventoryItemClicked(Button button)
        {
            GameObject item = GetChild0(button);
            if (item != null)
            {
                foreach (Button inbutton in inButtons)
                {
                    if (!HasChild(inbutton))
                    {
                        item.SetParent(inbutton.gameObject);

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

                bool isFull = true;
                foreach (Button inbutton in inButtons)
                {
                    if (!HasChild(inbutton))
                    {
                        isFull = false;
                        break;
                    }
                }
                if (isFull)
                {
                    GameObject prefab = Blacksmith.GenerateOut(ItemsIn);
                    GameObject o = Object.Instantiate(prefab, OutButton.transform);
                    o.transform.localPosition = new Vector3(-27.4f, -11.9f, -29.8f);
                    o.transform.localScale = new Vector3(20, 20, 20);
                }
            }
        }

        private void OnInItemClicked(Button button)
        {
            GameObject item = GetChild0(button);
            if (item != null)
            {
                foreach (Button invbutton in inventoryButtons)
                {
                    if (!HasChild(invbutton))
                    {
                        item.SetParent(invbutton.gameObject);

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

                if (ItemOut != null)
                {
                    DestroyChild0(OutButton);
                }
            }
        }

        private void OnOutItemClick()
        {
            if (ItemOut != null)
            {
                ItemOut.GetComponent<Item>().DropToInventory();
                RemoveChild0(OutButton);

                foreach (Button inbutton in inButtons)
                {
                    GameObject item = GetChild0(inbutton);
                    foreach (GameObject invitem in InventoryManager.Inventory.Items)
                    {
                        if (invitem.name.Substring(0, 5) == item.name.Substring(0, 5))
                        {
                            InventoryManager.Inventory.Remove(invitem);
                            break;
                        }
                    }
                }

                ResetIn();
                ResetLocalInventory();
            }
        }

        private void ResetIn()
        {
            ResetButtons(inButtons);
        }

        private void ResetLocalInventory()
        {
            ResetButtons(inventoryButtons);

            int i = 0;
            foreach (GameObject go in InventoryManager.Inventory.Items)
            {
                go.SetActive(true);
                GameObject gonew = Object.Instantiate(go, inventoryButtons[i++].transform);

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
            IsActive = false;
        }

        private bool HasChild(Component o)
        {
            return o.transform.childCount > 0;
        }

        private GameObject GetChild0(Component o)
        {
            return HasChild(o) ? o.transform.GetChild(0).gameObject : null;
        }

        private void DestroyChild0(Component o)
        {
            GameObject c = o.transform.GetChild(0).gameObject;
            c.transform.parent = null;
            c.SetActive(false);
            Object.Destroy(c);
        }

        private void RemoveChild0(Component o)
        {
            o.transform.GetChild(0).parent = null;
        }

        private void ResetButtons(List<Button> l)
        {
            foreach (Button button in l)
            {
                if (button.transform.childCount > 0)
                {
                    DestroyChild0(button);
                }
            }
        }
    }
}