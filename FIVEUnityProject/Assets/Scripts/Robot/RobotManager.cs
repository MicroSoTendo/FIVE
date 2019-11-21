using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Robot
{
    public class RobotManager : MonoBehaviour
    {
        private readonly Dictionary<string, GameObject> robotPrefabs = new Dictionary<string, GameObject>();
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
                robot.GetComponent<Movable>().enabled = false;
                int _id = robot.GetComponent<RobotSphere>().ID;

                instance.robots.Add(robot);
                instance.id2Robot.Add(_id, robot);

                return robot;
            }

            return null;
        }

        public static void RemoveRobot(GameObject robot)
        {
            int _id = robot.GetComponent<RobotSphere>().ID;
            instance.robots.Remove(robot);
            instance.id2Robot.Remove(_id);
        }
    }
}