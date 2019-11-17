using FIVE.Robot;
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
                //current Energy downs slower
                RobotManager.ActiveRobot.GetComponent<RobotComponents.Battery>().CurrentEnergy += 0.7f;
            }
        }
    }
}