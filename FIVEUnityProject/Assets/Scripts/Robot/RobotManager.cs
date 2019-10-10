using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace FIVE.Robot
{
    public class RobotManager : MonoBehaviour
    {
        private readonly Dictionary<(int, int), GameObject> robots = new Dictionary<(int, int), GameObject>();
        private readonly Dictionary<string, GameObject> robotPrefabs = new Dictionary<string, GameObject>();
        private static RobotManager instance;
        public static Dictionary<(int, int), GameObject> Robots => instance.robots;
        private void Awake()
        {
            Assert.IsTrue(instance == null); //Make sure singleton
            instance = this;
            GameObject[] prefabs = Resources.LoadAll<GameObject>("EntityPrefabs/RobotPrefabs");
            foreach (GameObject prefab in prefabs)
            {
                robotPrefabs.Add(prefab.name, prefab);
            }
        }

        public static GameObject CreateRobot(string prefabName, Vector3 pos, Quaternion quat)
        {
            if (instance.robotPrefabs.TryGetValue(prefabName, out GameObject prefab))
            {
                GameObject robot = Instantiate(prefab, pos, quat);
                instance.robots.Add(Key(robot), robot);
                return robot;
            }
            return null;
        }

        //Possible issue: 1 unity unit is large enough to have multiple robots
        //May cause ArgumentException for existed key.
        private static (int, int) Key(GameObject robot)
        {
            int x = (int)robot.transform.position.x;
            int z = (int)robot.transform.position.z;
            return (x >> 1, z >> 1);
        }
    }
}