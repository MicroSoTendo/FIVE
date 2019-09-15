
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FIVE.EventSystem;
using FIVE.UI;
using FIVE.UI.StartupMenu;
using Photon.Pun;
using UnityEngine;
using System.Collections.Specialized;
using System.ComponentModel;
using Photon.Realtime;

namespace FIVE.Network
{
    public class LobbyInfoModel : Observable
    {
        public ObservableCollection<RoomInfo> RoomsList { get; }

        public LobbyInfoModel()
        {
            RoomsList = new ObservableCollection<RoomInfo>();
            RoomsList.CollectionChanged += OnRoomsListChanged;
        }

        private void OnRoomsListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChange(new PropertyChangedEventArgs(nameof(RoomsList)));
        }

    }

    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        private LobbyInfoModel lobbyInfoModel;
        void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
            EventManager.Subscribe<OnMultiPlayersButtonClicked>(Initialize);
        }

        void Initialize(object sender, EventArgs args)
        {
            bool result = PhotonNetwork.JoinLobby();
            if (result)
            {
                Debug.Log($"{nameof(PhotonNetwork.JoinLobby)} Success");
            }
            else
            {
                Debug.Log($"{nameof(PhotonNetwork.JoinLobby)} Failed");
            }

        }
        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster() was called by PUN.");
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            lobbyInfoModel.RoomsList.Clear();
            roomList.ForEach(o => lobbyInfoModel.RoomsList.Add(o));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
