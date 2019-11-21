using FIVE.Robot;
using FIVE.RobotComponents;
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

        private void Awake()
        {
            item = GetComponent<Item>();
            item.ItemAction = ItemAction;
        }

        private void ItemAction(GameObject o)
        {
            if (SkyBoxTime.isDayTime())
            {
                SolarSingleton.PowerCharge = 1.0f;
            }
        }
    }
}