
using System;
using System.Collections.Generic;
using FIVE.EventSystem;
using FIVE.UI.StartupMenu;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

namespace FIVE.Network
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        private LobbyInfoModel lobbyInfoModel;
        void Start()
        {
            EventManager.Subscribe<OnMultiPlayersButtonClicked>(Initialize);
            lobbyInfoModel = new LobbyInfoModel();
        }

        void Initialize(object sender, EventArgs args)
        {
            Debug.Log($"Initialize");
            
            var connectingResult = PhotonNetwork.ConnectUsingSettings();
            if (connectingResult)
            {
                Debug.Log("connecting");
            }
            else
            {
                Debug.Log("fail to connect");
            }
        }

        public override void OnConnected()
        {
            Debug.Log("Connected!");
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster() was called by PUN.");
            bool result = PhotonNetwork.JoinLobby();
            // PhotonNetwork.JoinRandomRoom();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            lobbyInfoModel.RoomsList.Clear();
            roomList.ForEach(o => lobbyInfoModel.RoomsList.Add(o));
        }

        void Update()
        {

        }
    }
}
