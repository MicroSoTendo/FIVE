using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace FIVE.Robot
{
    public class RobotManager : MonoBehaviour
    {
        private readonly Dictionary<(int x, int y, int z), GameObject> robots = new Dictionary<(int, int, int), GameObject>();
        private readonly Dictionary<string, GameObject> robotPrefabs = new Dictionary<string, GameObject>();
        private static RobotManager instance;
        public static Dictionary<(int x, int y, int z), GameObject> Robots => instance.robots;
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
        private static (int x, int y, int z) Key(GameObject robot)
        {
            int x = (int)(robot.transform.position.x * 100);
            int y = (int)(robot.transform.position.y * 100);
            int z = (int)(robot.transform.position.z * 100);
            return (x, y, z);
        }
    }
}