using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Robot
{
    public class RobotManager : MonoBehaviour
    {
        public HashSet<GameObject> Robots;

        [SerializeField] private GameObject RobotPrefab = null;

        public static RobotManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            Robots = new HashSet<GameObject>();
        }

        public static GameObject CreateRobot(Vector3 pos)
        {
            GameObject robot = Instantiate(Instance.RobotPrefab, pos, Quaternion.identity);
            Instance.Robots.Add(robot);
            return robot;
        }
    }
}