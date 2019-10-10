using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Robot
{
    public class RobotManager : RobotBehaviour
    {
        public Dictionary<(int, int), GameObject> Robots = new Dictionary<(int, int), GameObject>();

        [SerializeField] private GameObject RobotPrefab = null;

        public static RobotManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public static GameObject CreateRobot(Vector3 pos)
        {
            GameObject robot = Instantiate(Instance.RobotPrefab, pos, Quaternion.identity);
            Instance.Robots.Add(Key(robot), robot);
            return robot;
        }

        private static (int, int) Key(GameObject robot)
        {
            int x = (int)robot.transform.position.x;
            int z = (int)robot.transform.position.z;
            return (x >> 1, z >> 1);
        }
    }
}