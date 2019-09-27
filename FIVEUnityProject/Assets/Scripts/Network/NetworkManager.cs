
using Assets.Scripts.PrefabPool;
using FIVE.EventSystem;
using FIVE.UI.StartupMenu;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Network
{
    public class NetworkManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        private LobbyInfoModel lobbyInfoModel;

        private void Awake()
        {
            PhotonNetwork.PrefabPool = PrefabPools.Instance;
        }

        private void Start()
        {
            EventManager.Subscribe<OnMultiPlayersButtonClicked>(Initialize);
            lobbyInfoModel = new LobbyInfoModel();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Initialize(object sender, EventArgs args)
        {
            bool connectingResult = PhotonNetwork.ConnectUsingSettings();
            Debug.Log(connectingResult ? "Initialize" : "fail to connect");
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
            this.RaiseEvent<OnConnected>();
        }

        public override void OnConnectedToMaster()
        {
            this.RaiseEvent<OnConnectedToMaster>();
        }

        public override void OnJoinedLobby()
        {
            this.RaiseEvent<OnJoinedLobby>();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            this.RaiseEvent<OnRoomListUpdate>(new OnRoomListUpdateEventArgs(roomList));
            //lobbyInfoModel.RoomsList.Clear();
            //roomList.ForEach(o => lobbyInfoModel.RoomsList.Add(o));
        }

        public void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }

        public void JoinOrCreateRoom(string roomName)
        {
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions(), new TypedLobby());
        }


        public override void OnJoinedRoom()
        {
            this.RaiseEvent<OnJoinedRoom>();
        }


        public void CreateRoom(string roomName, RoomOptions roomOptions)
        {
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

        }

        public void JoinLobby()
        {
            PhotonNetwork.JoinLobby();
        }
    }
}
