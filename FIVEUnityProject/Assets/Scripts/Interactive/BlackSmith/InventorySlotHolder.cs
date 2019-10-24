using FIVE.Interactive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.UI.BlackSmith
{
    public class InventorySlotHolder : MonoBehaviour
    {
        private int allSlots;
        private int[] enabledSlots;
        private int[] enabledComps;
        private GameObject[] slot;
        public GameObject slotHolder;
        private Inventory itemInventory;
        private List<GameObject> compList; 
        private List<GameObject> invList;
        void Start()
        {
            itemInventory = InventoryManager.FindInventory();
            compList = new List<GameObject>(3);
            enabledComps = new int[3];
            allSlots = 9;
            enabledSlots = new int[allSlots];
            slot = new GameObject[allSlots];
            for (int i = 0; i < allSlots; i++)
            {
                enabledSlots[i] = 0;
                slot[i] = slotHolder.transform.GetChild(i).gameObject;
            }
            for(int i = 0; i < 3; i++)
            {
                enabledComps[i] = 0;
            }
        }

        // Update is called once per frame
        void Update()
        {
            itemInventory = InventoryManager.FindInventory();
            invList = itemInventory.Items;
            if (invList != null)
            {
                for (int i = 0; i < invList.Count; i++)
                {
                    if (enabledSlots[i] == 0)
                    {
                        //list[i].GetComponent<Item>().Info.UIPosition = new Vector3();
                        Instantiate(invList[i], slot[i].transform);
                        enabledSlots[i] = 1;
                    }

                }
            }
        }
        public void SetParentInvToComp(int index)
        {
            for(int i = 0; i < 3; i++)
            {
                if(enabledComps[i] == 0)
                {
                    if(invList[index] != null)
                    {
                        invList[index].SetParent(compList[i]);
                        itemInventory.RemoveAt(index);
                        enabledComps[i] = 1;
                        enabledSlots[index] = 0;
                    }
                    

                }
            }
        }
    }
}
