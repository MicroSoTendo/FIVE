using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Interactive.Items
{
    public class SolarPanel : MonoBehaviour
    {
        private float Capacity = 100;
        private Item item;
        private float Remaining = 100;
        void Awake()
        {
            item = GetComponent<Item>();
            item.ItemAction = ItemAction;
        }

        private void ItemAction(GameObject owner, GameObject o)
        {
            if(SkyBoxTime.isDayTime())
            {
                //current Energy downs slower
                owner.GetComponent<RobotComponents.Battery>().CurrentEnergy+= 0.7f;
            }

        }
    }
}
