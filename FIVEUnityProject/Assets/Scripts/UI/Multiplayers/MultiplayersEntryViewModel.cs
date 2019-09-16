using System.Collections.Generic;
using System.ComponentModel;
using FIVE.EventSystem;
using FIVE.EventSystem.EventTypes;
using FIVE.Network;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.Multiplayers
{
    public class MultiplayersEntryViewModel : ViewModel<MultiplayersEntryView, MultiplayersEntryViewModel>
    {
        private readonly Transform contentTransform;
        private readonly List<GameObject> roomEntries = new List<GameObject>();
        public MultiplayersEntryViewModel()
        {
            contentTransform = View.ContentTransform;
            EventManager.Subscribe<OnPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>(OnLobbyInfoChanged);
        }

        private void OnLobbyInfoChanged(object sender, PropertyChangedEventArgs e)
        {
            var lobbyInfoModel = sender as LobbyInfoModel;
            for (int index = 0; index < roomEntries.Count; index++)
            {
                GameObject t = roomEntries[index];
                t.SetActive(false);
                Object.Destroy(t);
            }

            roomEntries.Clear();

            for (int index = 0; index < lobbyInfoModel?.RoomsList.Count; index++)
            {
                RoomInfo roomInfo = lobbyInfoModel.RoomsList[index];
                //if (!roomInfo.IsVisible) return;
                GameObject roomEntryPrefab = View.Resources["RoomEntry"];
                GameObject roomEntry = Object.Instantiate(roomEntryPrefab, contentTransform);
                roomEntry.name = roomInfo.Name;
                roomEntry.name = $"Room {index}";
                RectTransform rectTransform = roomEntry.GetComponent<RectTransform>();
                rectTransform.anchoredPosition3D = new Vector3(0, -50 -rectTransform.rect.height * index, 0);

                roomEntry.transform.GetChild(0).GetComponent<Text>().text = $"{roomInfo.Name}";
                roomEntry.transform.GetChild(1).GetComponent<Text>().text = $"Players: {roomInfo.PlayerCount}";

                roomEntry.SetActive(true);
                roomEntries.Add(roomEntry);
            }
        }
    }
}