using System;
using FIVE.EventSystem;
using FIVE.Network;
using FIVE.UI;
using FIVE.UI.Multiplayers;
using Mirror;
using UnityEngine;


namespace FIVE.GameModes
{
    public class Multiplayers : MonoBehaviour
    {
        private ListServer listServer;
        private NetworkManager networkManager;

        private void Awake()
        {
            networkManager = FindObjectOfType<NetworkManager>();
            listServer = FindObjectOfType<ListServer>();
            UIManager.Create<LobbyWindowViewModel>().IsActive = true;
        }

        private void Start()
        {
            EventManager.Subscribe<OnCreateRoomRequested, CreateRoomRequestedEventArgs>(CreateRoomHandler);
        }

        private void CreateRoomHandler(object sender, CreateRoomRequestedEventArgs e)
        {
            networkManager.StartHost();
        }
    }
}