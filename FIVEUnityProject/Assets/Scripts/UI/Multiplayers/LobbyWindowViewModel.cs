using FIVE.EventSystem;
using FIVE.Network;
using System.Collections;
using System.Collections.Generic;
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

        private readonly GameObject entryPrefab = Resources.Load<GameObject>("EntityPrefabs/UI/Multiplayers/RoomEntry");

        private GameObject selectedEntry;
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

        public override bool IsActive
        {
            get => base.IsActive;
            set
            {
                base.IsActive = value;
                if (value)
                {
                    MainThreadDispatcher.ScheduleCoroutine(UpdateRoomInfo());
                }
            }
        }

        private static void SetEntryInfo(GameObject entry, string roomName, int currentPlayers, int maxPlayers, bool hasPassword)
        {
            entry.FindChildRecursive("RoomName").GetComponent<TMP_Text>().text = roomName;
            entry.FindChildRecursive("PlayersCount").GetComponent<TMP_Text>().text =
                $"{currentPlayers}/{maxPlayers}";
            entry.FindChildRecursive("Locked").SetActive(hasPassword);
        }
        public IEnumerator UpdateRoomInfo()
        {
            while (IsActive)
            {
                float yOffset = 0f;
                //foreach (RoomInfo roomInfo in NetworkManager.Instance.RoomInfos)
                //{
                //    if (info2Entry.ContainsKey(roomInfo))
                //    {
                //        GameObject entry = info2Entry[roomInfo];
                //        SetEntryInfo(entry, roomInfo);
                //    }
                //    else
                //    {
                //        GameObject entry = Object.Instantiate(entryPrefab, ContentTransform);
                //        info2Entry.Add(roomInfo, entry);
                //        entry2Info.Add(entry, roomInfo);
                //        SetEntryInfo(entry, roomInfo);
                //        entry.transform.localPosition += new Vector3(0f, yOffset, 0f);
                //        entry.GetComponent<Button>().onClick.AddListener(() => OnEntryClick(entry));
                //        yOffset -= 100f;
                //    }
                //    yield return null;
                //}
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void OnEntryClick(GameObject entry)
        {
            entry.GetComponent<Button>().Select();
            selectedEntry = entry;
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
            //RoomInfo info = entry2Info[selectedEntry];
            //if (entry2Info[selectedEntry].HasPassword)
            //{
            //    PasswordPopUpViewModel passwordVM = UIManager.Get<PasswordPopUpViewModel>();
            //    passwordVM.IsActive = true;
            //    Bind(passwordVM.ConfirmButton).To(() =>
            //    {
            //        this.RaiseEvent<OnJoinRoomRequested>(new JoinRoomArgs(info.Guid, passwordVM.PasswordInputField.text));
            //        passwordVM.IsActive = false;
            //    });
            //}
            //else
            //{
            //    this.RaiseEvent<OnJoinRoomRequested>(new JoinRoomArgs(info.Guid, ""));
            //    IsActive = false;
            //}
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
            //UnBind(CreateButton).With(CreateButtonLobbyHandler);
            CreateButton.onClick.RemoveAllListeners();
            Title.text = "Create Room";
            Toggle(Bar, RoomScrollView, JoinButton, ReturnButton, CreateRoomPanel);
            Bind(CreateButton).To(CreateRoomHandler);
        }

        private void CreateRoomHandler()
        {
            string roomName = RoomNameInputField.text;
            int size = int.Parse(RoomSizeInputField.text);
            string password = PasswordInputField.text;
            this.RaiseEvent<OnCreateRoomRequested>(new CreateRoomArgs(roomName, size, password.Length != 0, password));
            IsActive = false;
        }
    }
}