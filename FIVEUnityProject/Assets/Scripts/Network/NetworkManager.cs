using FIVE.EventSystem;
using FIVE.UI.StartupMenu;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Network
{
    public class NetworkManager
    {
        private LobbyInfoModel lobbyInfoModel;

        private void Awake()
        {
        }

        private void Start()
        {
            lobbyInfoModel = new LobbyInfoModel();
        }

        private void Initialize(object sender, EventArgs args)
        {
        }

        public void SetPlayerName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            PlayerPrefs.SetString("PlayerName", value);
        }

        public  void OnConnected()
        {
            this.RaiseEvent<OnConnected>();
        }

        public  void OnConnectedToMaster()
        {
            this.RaiseEvent<OnConnectedToMaster>();
        }

        public  void OnJoinedLobby()
        {
            this.RaiseEvent<OnJoinedLobby>();
        }

        public  void OnRoomListUpdate(List<object> roomList)
        {
            this.RaiseEvent<OnRoomListUpdate>(new OnRoomListUpdateEventArgs(roomList));
            //lobbyInfoModel.RoomsList.Clear();
            //roomList.ForEach(o => lobbyInfoModel.RoomsList.Add(o));
        }

        public void JoinRoom(string roomName)
        {
        }

        public void JoinOrCreateRoom(string roomName)
        {
        }


        public void OnJoinedRoom()
        {
            this.RaiseEvent<OnJoinedRoom>();
        }


        public void CreateRoom(string roomName)
        {
        }


    }
}
