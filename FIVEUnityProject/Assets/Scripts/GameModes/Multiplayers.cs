using System;
using FIVE.CameraSystem;
using FIVE.EventSystem;
using FIVE.Network;
using FIVE.UI;
using FIVE.UI.Multiplayers;
using Mirror;
using UnityEngine;


namespace FIVE.GameModes
{
    [RequireComponent(typeof(ListServer))]
    [RequireComponent(typeof(NetworkManager))]
    public class Multiplayers : MonoBehaviour
    {
        private ListServer listServer;
        private NetworkManager networkManager;

        private void Awake()
        {
            networkManager = GetComponent<NetworkManager>();
            listServer = GetComponent<ListServer>();
            UIManager.Create<LobbyWindowViewModel>().IsActive = true;
        }

        private void Start()
        {
            EventManager.Subscribe<OnCreateRoomRequested, CreateRoomRequestedEventArgs>(CreateRoomHandler);
        }


        private void CreateRoomHandler(object sender, CreateRoomRequestedEventArgs e)
        {
            listServer.gameServerTitle = e.RoomInfo.Name;
            networkManager.maxConnections = e.RoomInfo.Size;
            networkManager.StartHost();
        }
    }
}