using FIVE.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.Multiplayers
{
    public class MultiplayersEntryViewModel : ViewModel<MultiplayersEntryView, MultiplayersEntryViewModel>
    {
        private readonly Transform contentTransform;
        private readonly Dictionary<object, GameObject> roomEntries = new Dictionary<object, GameObject>();
        public MultiplayersEntryViewModel()
        {
            contentTransform = View.ContentTransform;
            // EventManager.Subscribe<OnObservableChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>(OnLobbyInfoChanged);
            binder.Bind(View => View.CreateButton.onClick).To(ViewModel => ViewModel.OnCreateButtonClicked);
        }

        private void OnLobbyInfoChanged(object sender, PropertyChangedEventArgs e)
        {
            LobbyInfoModel lobbyInfoModel = sender as LobbyInfoModel;
            foreach (object roomInfo in lobbyInfoModel.RoomsList)
            {
                if (roomEntries.ContainsKey(roomInfo))
                {
                }
                else
                {
                    Button roomEntry = View.AddUIElementFromResources<Button>("RoomEntry", $"Room {roomInfo.ToString()}", contentTransform);
                    roomEntry.onClick.AddListener(OnRoomEntryClicked);
                    roomEntry.gameObject.SetActive(true);
                    roomEntries.Add(roomInfo, roomEntry.gameObject);
                }
            }
            int i = 0;
            foreach (KeyValuePair<object, GameObject> keyValue in roomEntries)
            {
                RectTransform rectTransform = keyValue.Value.GetComponent<RectTransform>();
                rectTransform.anchoredPosition3D = new Vector3(0, -50 - rectTransform.rect.height * i, 0);
                i++;
            }
        }

        private void OnCreateButtonClicked(object sender, EventArgs e)
        {
            SetEnabled(false);
            UIManager.AddViewModel<CreateRoomViewModel>();
        }

        private void OnRoomEntryClicked()
        {

        }
    }
}