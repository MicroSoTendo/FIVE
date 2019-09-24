
using System;
using System.Collections.Generic;
using Assets.Scripts.PrefabPool;
using FIVE.EventSystem;
using FIVE.UI.StartupMenu;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

namespace FIVE.Network
{
    public class NetworkManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        private LobbyInfoModel lobbyInfoModel;

        void Awake()
        {
            PhotonNetwork.PrefabPool = PrefabPools.Instance;
        }

        void Start()
        {
            EventManager.Subscribe<OnMultiPlayersButtonClicked>(Initialize);
            lobbyInfoModel = new LobbyInfoModel();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        void Initialize(object sender, EventArgs args)
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
            Debug.Log("OnConnected");
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster");
            bool result = PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log(nameof(OnJoinedLobby));
            PhotonNetwork.JoinOrCreateRoom("Test Room", new RoomOptions(), new TypedLobby());
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
            GameObject multiPlayer = new GameObject("Multiplayer");
            multiPlayer.AddComponent<MultiplayersManager>();
        }


        public void CreateRoom(string roomName, RoomOptions roomOptions)
        {
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }

        void Update()
        {
            
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            
        }
    }
}
