using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Robot
{
    public class RobotManager : MonoBehaviour
    {
        public List<GameObject> Robots;

        [SerializeField] private GameObject RobotPrefab = null;
        private static RobotManager instance;

        private void Awake()
        {
            instance = this;
            Robots = new List<GameObject>();
        }

        public static GameObject CreateRobot()
        {
            GameObject robot = Instantiate(instance.RobotPrefab, new Vector3(0f, 20f, 0f), Quaternion.identity);
            instance.Robots.Add(robot);
            return robot;
        }

        public static GameObject GetPrefab()
        {
            return instance.RobotPrefab;
        }
    }
}