using Mirror;
using UnityEngine;

namespace FIVE.Robot
{
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(NetworkTransformChild))]
    public class RobotBehaviour : NetworkBehaviour
    {
    }
}
