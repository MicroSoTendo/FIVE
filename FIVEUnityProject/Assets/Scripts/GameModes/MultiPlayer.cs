using FIVE.Network;
using FIVE.Robot;
using UnityEngine;

namespace FIVE.GameStates
{
    public class MultiPlayer : GameMode
    {
        void Start()
        {
            GameObject robot = RobotManager.CreateRobot();
            NetworkProxy.TryCreateProxy(robot, out NetworkProxy proxy);
        }

    }
}
