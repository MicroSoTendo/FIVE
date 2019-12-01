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
            item.ItemUseAction = UseAction;
        }

        private void UseAction()
        {
            RobotManager.ActiveRobot.GetComponent<RobotComponents.Battery>().CurrentEnergy = Remaining;
        }
    }
}