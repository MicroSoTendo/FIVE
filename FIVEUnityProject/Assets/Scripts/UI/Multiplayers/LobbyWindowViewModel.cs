using FIVE.Network;
using TMPro;
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
        public Button ReturnButton { get; }
        public Button RevealButton { get; }
        public GameObject RoomScrollView { get; }
        public GameObject CreateRoomPanel { get; }
        public TMP_InputField RoomNameInputField { get; }
        public TMP_InputField RoomSizeInputField { get; }
        public TMP_InputField PasswordInputField { get; }
        public GameObject Bar { get; }
        public TMP_Text Title { get; }

        public LobbyWindowViewModel()
        {
            ZIndex = 1;
            RoomScrollViewContent = Get(nameof(RoomScrollViewContent));
            ContentTransform = RoomScrollViewContent.transform;
            CreateButton = Get<Button>(nameof(CreateButton));
            JoinButton = Get<Button>(nameof(JoinButton));
            ReturnButton = Get<Button>(nameof(ReturnButton));
            RevealButton = Get<Button>(nameof(RevealButton));
            Title = Get<TMP_Text>(nameof(Title));
            RoomNameInputField = Get<TMP_InputField>(nameof(RoomNameInputField));
            RoomSizeInputField = Get<TMP_InputField>(nameof(RoomSizeInputField));
            PasswordInputField = Get<TMP_InputField>(nameof(PasswordInputField));
            PasswordInputField.inputType = TMP_InputField.InputType.Password;

            RoomScrollView = Get(nameof(RoomScrollView));
            CreateRoomPanel = Get(nameof(CreateRoomPanel));
            Bar = Get(nameof(Bar));
            Bind(CreateButton).To(CreateButtonLobbyHandler);
            Bind(JoinButton).To(OnJoinButtonClicked);
            Bind(ReturnButton).To(OnReturnButtonClicked);
            Bind(RevealButton).To(OnRevealButonClicked);
        }

        private void OnRevealButonClicked()
        {
            PasswordInputField.inputType ^= TMP_InputField.InputType.Password;
            PasswordInputField.ForceLabelUpdate();
        }

        private void OnReturnButtonClicked()
        {
            Title.text = "Lobby";
            Toggle(Bar, RoomScrollView, JoinButton, ReturnButton, CreateRoomPanel);
        }

        private void OnJoinButtonClicked()
        {
            //JoinButton.RaiseEvent<OnJoinButtonPressed>();
        }

        private void Toggle(params object[] objects)
        {
            foreach (object o in objects)
            {
                if (o is GameObject go)
                {
                    go.SetActive(!go.activeSelf);
                }
                else if (o is Component co)
                {
                    co.gameObject.SetActive(!co.gameObject.activeSelf);
                }
            }
        }

        private void CreateButtonLobbyHandler()
        {
            Title.text = "Create Room";
            Toggle(Bar, RoomScrollView, JoinButton, ReturnButton, CreateRoomPanel);
            UnBind(CreateButton).With(CreateButtonLobbyHandler);
            Bind(CreateButton).To(CreateButtonCreateRoomHandler);
        }

        private void CreateButtonCreateRoomHandler()
        {
            string roomName = RoomNameInputField.text;
            int size = int.Parse(RoomSizeInputField.text);
            string password = PasswordInputField.text;
            this.RaiseEvent<OnCreateRoomRequested>(new CreateRoomRequestedEventArgs(new RoomInfo(roomName, size, password)));
            Root.SetActive(false);
        }
    }
}