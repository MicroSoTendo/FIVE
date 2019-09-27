using FIVE.Network;
using Photon.Realtime;
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
        private readonly Dictionary<RoomInfo, GameObject> roomEntries = new Dictionary<RoomInfo, GameObject>();
        public MultiplayersEntryViewModel()
        {
            contentTransform = View.ContentTransform;
            // EventManager.Subscribe<OnObservableChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>(OnLobbyInfoChanged);
            binder.Bind(View => View.CreateButton.onClick).To(ViewModel => ViewModel.OnCreateButtonClicked);
        }

        private void OnLobbyInfoChanged(object sender, PropertyChangedEventArgs e)
        {
            LobbyInfoModel lobbyInfoModel = sender as LobbyInfoModel;
            foreach (RoomInfo roomInfo in lobbyInfoModel.RoomsList)
            {
                if (roomEntries.ContainsKey(roomInfo))
                {
                    if (roomInfo.RemovedFromList || roomInfo.PlayerCount == 0)
                    {
                        View.RemoveUIElement(roomEntries[roomInfo].name);
                        roomEntries.Remove(roomInfo);
                    }
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
            foreach (KeyValuePair<RoomInfo, GameObject> keyValue in roomEntries)
            {
                RectTransform rectTransform = keyValue.Value.GetComponent<RectTransform>();
                rectTransform.anchoredPosition3D = new Vector3(0, -50 - rectTransform.rect.height * i, 0);
                keyValue.Value.transform.GetChild(0).GetComponent<Text>().text = $"{keyValue.Key.Name}";
                keyValue.Value.transform.GetChild(1).GetComponent<Text>().text = $"Players: {keyValue.Key.PlayerCount}/8";
                i++;
            }
        }

        private void OnCreateButtonClicked(object sender, EventArgs e)
        {
            SetActive(false);
            UIManager.AddViewModel<CreateRoomViewModel>();
        }

        private void OnRoomEntryClicked()
        {

        }
    }
}