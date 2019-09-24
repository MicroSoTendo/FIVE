using FIVE.EventSystem;
using FIVE.UI.StartupMenu;
using UnityEngine;

namespace FIVE.Robot
{
    public class RobotManager : MonoBehaviour
    {
        [SerializeField] private GameObject RobotPrefab = null;
        private static RobotManager instance;
        private void Awake()
        {
            instance = this;
        }

        public static GameObject CreateRobot()
        {
            return Instantiate(instance.RobotPrefab, new Vector3(0f, 20f, 0f), Quaternion.identity);
        }

        public static GameObject GetPrefab()
        {
            return instance.RobotPrefab;
        }
    }
}