using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace FIVE.Robot
{
    public class RobotManager : MonoBehaviour
    {
        public Dictionary<(int, int, int), List<int>> RobotIDs = new Dictionary<(int, int, int), List<int>>();

        public HashSet<GameObject> Robots = new HashSet<GameObject>();

        private readonly Dictionary<string, GameObject> robotPrefabs = new Dictionary<string, GameObject>();

        public static RobotManager Instance { get; private set; }

        private void Awake()
        {
            Assert.IsTrue(Instance == null); // Make sure singleton
            Instance = this;
            GameObject[] prefabs = Resources.LoadAll<GameObject>("EntityPrefabs/RobotPrefabs");
            foreach (GameObject prefab in prefabs)
            {
                robotPrefabs.Add(prefab.name, prefab);
            }
        }

        public static GameObject CreateRobot(string prefabName, Vector3 pos, Quaternion quat)
        {
            if (Instance.robotPrefabs.TryGetValue(prefabName, out GameObject prefab))
            {
                GameObject robot = Instantiate(prefab, pos, quat);
                Instance.Robots.Add(robot);

                (int x, int y, int z) k = Key(robot);
                if (!Instance.RobotIDs.ContainsKey(k))
                {
                    Instance.RobotIDs[k] = new List<int>();
                }

                Instance.RobotIDs[k].Add(robot.GetInstanceID());
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

        public List<int> FindNearbyRobots(GameObject robot)
        {
            (int x, int y, int z) k = Key(robot);
            var ret = new List<int>();
            ret.AddRange(RobotIDs[k]);

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        (int, int, int) _k = (k.x + dx, k.y + dy, k.z + dz);
                        if (RobotIDs.ContainsKey(_k))
                        {
                            ret.AddRange(RobotIDs[_k]);
                        }
                    }
                }
            }

            return ret;
        }
    }
}