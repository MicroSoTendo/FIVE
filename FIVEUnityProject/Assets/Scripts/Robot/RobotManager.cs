using FIVE.UI;
using FIVE.UI.CodeEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace FIVE.Robot
{
    public class RobotManager : MonoBehaviour
    {
        private readonly Dictionary<string, GameObject> robotPrefabs = new Dictionary<string, GameObject>();
        private readonly Dictionary<(int, int, int), List<int>> hashMap = new Dictionary<(int, int, int), List<int>>();
        private readonly Dictionary<int, GameObject> id2Robot = new Dictionary<int, GameObject>();
        private readonly HashSet<GameObject> robots = new HashSet<GameObject>();
        private GameObject activeRobot;
        private static RobotManager instance;

        public static HashSet<GameObject> Robots => instance.robots;
        public static Dictionary<int, GameObject> ID2Robot => instance.id2Robot;

        private int id = 0;
        public static int NextID => ++instance.id;

        public static GameObject ActiveRobot
        {
            get => instance.activeRobot;
            set
            {
                if (instance.activeRobot)
                {
                    instance.activeRobot.GetComponent<Movable>().enabled = false;
                }

                instance.activeRobot = value;
                instance.activeRobot.GetComponent<Movable>().enabled = true;
            }
        }

        private void Awake()
        {
            Assert.IsTrue(instance == null); // Make sure singleton
            instance = this;
            GameObject[] prefabs = Resources.LoadAll<GameObject>("EntityPrefabs/RobotPrefabs");
            foreach (GameObject prefab in prefabs)
            {
                robotPrefabs.Add(prefab.name, prefab);
            }
            StartCoroutine(ToggleEditorCoroutine());
        }

        private IEnumerator ToggleEditorCoroutine()
        {
            while (true)
            {
                if (!UIManager.Get<CodeEditorViewModel>()?.IsFocused ?? true)
                {
                    if (Input.GetKey(KeyCode.E))
                    {
                        this.RaiseEventFixed<OnToggleEditorRequested>(new LauncherEditorArgs(), 300);
                    }
                }
                yield return null;
            }
        }

        public static GameObject CreateRobot(string prefabName, Vector3 pos, Quaternion quat)
        {
            if (instance.robotPrefabs.TryGetValue(prefabName, out GameObject prefab))
            {
                GameObject robot = Instantiate(prefab, pos, quat);
                robot.GetComponent<Movable>().enabled = false;
                int _id = robot.GetComponent<RobotSphere>().ID;

                instance.robots.Add(robot);
                instance.id2Robot.Add(_id, robot);

                (int x, int y, int z) k = Key(robot);
                if (!instance.hashMap.ContainsKey(k))
                {
                    instance.hashMap[k] = new List<int>();
                }

                instance.hashMap[k].Add(_id);

                return robot;
            }

            return null;
        }

        public static void RemoveRobot(GameObject robot)
        {
            int _id = robot.GetComponent<RobotSphere>().ID;
            instance.robots.Remove(robot);
            instance.id2Robot.Remove(_id);
            (int x, int y, int z) k = Key(robot);
            instance.hashMap[k].Add(_id);
        }

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
            ret.AddRange(hashMap[k]);

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        (int, int, int) _k = (k.x + dx, k.y + dy, k.z + dz);
                        if (hashMap.ContainsKey(_k))
                        {
                            ret.AddRange(hashMap[_k]);
                        }
                    }
                }
            }

            return ret;
        }
    }
}