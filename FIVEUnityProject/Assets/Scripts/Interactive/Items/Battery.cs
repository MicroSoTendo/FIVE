using FIVE.Robot;
using UnityEngine;

namespace FIVE.Interactive.Items
{
    public class Battery : MonoBehaviour
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
            RobotManager.ActiveRobot.GetComponent<RobotComponents.Battery>().CurrentEnergy = Remaining;
        }
    }
}