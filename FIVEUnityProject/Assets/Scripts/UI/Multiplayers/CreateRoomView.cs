using UnityEngine;
using UnityEngine.UI;
namespace FIVE.UI.Multiplayers
{
    public class CreateRoomView : View<CreateRoomView, CreateRoomViewModel>
    {
        public Text RoomName { get; }
        public InputField RoomNameInput { get; }

        public Button CreateButton {get;}
        public CreateRoomView()
        {
            RoomName = AddUIElement<Text>("RoomName");
            RoomName.color = Color.white;
            RoomNameInput = AddUIElement<InputField>("RoomNameInput");
            RoomNameInput.caretColor = Color.white;
            RoomName.alignment = TextAnchor.MiddleLeft;
            CreateButton = AddUIElement<Button>("CreateButton");
        }
    }
}