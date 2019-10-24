using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace FIVE.Robot
{
    public class RobotManager : MonoBehaviour
    {
        private readonly Dictionary<string, GameObject> robotPrefabs = new Dictionary<string, GameObject>();

        public HashSet<GameObject> Robots = new HashSet<GameObject>();
        public Dictionary<int, GameObject> ID2Robot = new Dictionary<int, GameObject>();

        public Dictionary<(int, int, int), List<int>> HashMap = new Dictionary<(int, int, int), List<int>>();

        public static RobotManager Instance { get; private set; }

        private GameObject activeRobot;

        public GameObject ActiveRobot
        {
            get => activeRobot;
            set
            {
                if (activeRobot)
                    activeRobot.GetComponent<Movable>().enabled = false;
                activeRobot = value;
                activeRobot.GetComponent<Movable>().enabled = true;
            }
        }

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
                robot.GetComponent<Movable>().enabled = false;

                Instance.Robots.Add(robot);
                Instance.ID2Robot.Add(robot.GetInstanceID(), robot);

                (int x, int y, int z) k = Key(robot);
                if (!Instance.HashMap.ContainsKey(k))
                {
                    Instance.HashMap[k] = new List<int>();
                }
                Instance.HashMap[k].Add(robot.GetInstanceID());

                return robot;
            }

            return null;
        }

        public static void RemoveRobot(GameObject robot)
        {
            Instance.Robots.Remove(robot);
            Instance.ID2Robot.Remove(robot.GetInstanceID());
            (int x, int y, int z) k = Key(robot);
            Instance.HashMap[k].Add(robot.GetInstanceID());
        }

        // Possible issue: 1 unity unit is large enough to have multiple robots
        // May cause ArgumentException for existed key.
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
            ret.AddRange(HashMap[k]);

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        (int, int, int) _k = (k.x + dx, k.y + dy, k.z + dz);
                        if (HashMap.ContainsKey(_k))
                        {
                            ret.AddRange(HashMap[_k]);
                        }
                    }
                }
            }

            return ret;
        }
    }
}