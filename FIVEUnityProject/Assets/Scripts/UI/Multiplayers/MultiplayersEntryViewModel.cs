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
        private RectTransform listTransform;
        private List<GameObject> roomEntries = new List<GameObject>();
        public MultiplayersEntryViewModel()
        {
            listTransform = view.LobbyListScrollRect.content;
            EventManager.Subscribe<OnPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>(OnLobbyInfoChanged);
        }

        private void OnLobbyInfoChanged(object sender, PropertyChangedEventArgs e)
        {
            Debug.Log("OnLobbyInfoChanged");
            LobbyInfoModel lobbyInfoModel = sender as LobbyInfoModel;
            for (int index = 0; index < roomEntries.Count; index++)
            {
                roomEntries[index].SetActive(false);
                GameObject.Destroy(roomEntries[index]);
            }
            roomEntries.Clear();

            for (int index = 0; index < lobbyInfoModel.RoomsList.Count; index++)
            {
                RoomInfo roomInfo = lobbyInfoModel.RoomsList[index];
                var gameObject = new GameObject();
                var button = gameObject.AddComponent<Button>();
                var rectTransform = button.GetComponent<RectTransform>();
                rectTransform.SetParent(listTransform);
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.sizeDelta = new Vector2(40, 100);
                rectTransform.offsetMin = new Vector2(10, 10);
                rectTransform.offsetMax = new Vector2(10, 10);
                rectTransform.anchoredPosition = new Vector3(0, -100 - index * 100, 0);
                gameObject.GetComponentInChildren<Text>().text = $"{roomInfo.Name}        Players:{roomInfo.PlayerCount}";
            }
        }
    }
}