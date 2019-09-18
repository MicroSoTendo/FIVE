
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
            PhotonNetwork.AutomaticallySyncScene = true;
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
        
        public void SetPlayerName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            PhotonNetwork.NickName = value;
            PlayerPrefs.SetString("PlayerName", value);
        }

        public override void OnConnected()
        {
            Debug.Log("Connected!");
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster() was called by PUN.");
            bool result = PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log(nameof(OnJoinedLobby));
            PhotonNetwork.JoinOrCreateRoom("Test Room", new RoomOptions() { MaxPlayers = 10 }, new TypedLobby());
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            lobbyInfoModel.RoomsList.Clear();
            roomList.ForEach(o => lobbyInfoModel.RoomsList.Add(o));
        }

        public void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        
        public override void OnJoinedRoom()
        {
            Debug.Log(nameof(OnJoinedRoom));
            var multiplayersGame = new MultiplayersGame(
                PhotonNetwork.CurrentRoom.PlayerCount == 1 ? //Check if I am the first player
                MultiplayersGame.State.Host: MultiplayersGame.State.Client);
        }


        public void CreateRoom(string roomName, RoomOptions roomOptions)
        {
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }

        void Update()
        {
            
        }
    }
}
