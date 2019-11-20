using UnityEngine;

namespace FIVE.RobotComponents
{
    public class RobotComponent : MonoBehaviour
    {
        [Range(0f, 10f)] public float PowerConsumption = 0;
    }
}