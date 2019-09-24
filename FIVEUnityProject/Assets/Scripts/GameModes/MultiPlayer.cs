using System;
using FIVE.EventSystem;
using FIVE.Network;
using FIVE.Robot;
using UnityEngine;

namespace FIVE.GameStates
{
    public class MultiPlayer : GameMode
    {
        private NetworkManager networkManager;
        void Awake()
        {
            networkManager = FindObjectOfType<NetworkManager>();
        }

        void Start()
        {
            EventManager.Subscribe<OnConnectedToMaster>(OnConnectedToMaster);
            EventManager.Subscribe<OnJoinedLobby>(OnJoinedLobby);
            EventManager.Subscribe<OnJoinedRoom>(OnJoinedRoom);
        }

        private void OnJoinedRoom(object sender, EventArgs e)
        {
            GameObject robot = RobotManager.CreateRobot();
            robot.SetActive(false);
            NetworkProxy.ProxyIt(robot, SyncModule.Transform);
            robot.name = "P1";
        }

        private void OnJoinedLobby(object sender, EventArgs e)
        {
            networkManager.JoinOrCreateRoom("Test Room");
        }

        private void OnConnectedToMaster(object sender, EventArgs e)
        {
            networkManager.JoinLobby();
        }
    }
}
