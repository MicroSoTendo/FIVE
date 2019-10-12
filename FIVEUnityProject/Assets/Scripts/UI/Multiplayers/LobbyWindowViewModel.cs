using FIVE.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.Multiplayers
{
    public class LobbyWindowViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/Multiplayers/LobbyWindow";
        public GameObject RoomScrollViewContent { get; }
        public Transform ContentTransform { get; }
        public Button JoinButton { get; }
        public Button CreateButton { get; }
        public LobbyWindowViewModel()
        {
            RoomScrollViewContent = Get(nameof(RoomScrollViewContent));
            ContentTransform = RoomScrollViewContent.transform;
            CreateButton = Get<Button>(nameof(CreateButton));
            JoinButton = Get<Button>(nameof(JoinButton));
            Bind(CreateButton).To(OnCreateButtonClicked);
            Bind(JoinButton).To(OnJoinButtonClicked);
        }

        private void OnJoinButtonClicked()
        {

        }

        private void OnCreateButtonClicked()
        {
            UIManager.GetOrCreate<CreateRoomViewModel>().SetActive(true);
        }
    }
}