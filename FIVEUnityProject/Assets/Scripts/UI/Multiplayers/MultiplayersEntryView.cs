using UnityEngine;
using UnityEngine.UI;
namespace FIVE.UI.Multiplayers
{
    public class MultiplayersEntryView : View<MultiplayersEntryView, MultiplayersEntryViewModel>
    {
        public GameObject ScrollWindow { get; }
        public Button JoinButton { get; }
        public Button CreateButton { get; }
        public Transform ContentTransform { get; }

        public MultiplayersEntryView()
        {
            ScrollWindow = AddUIElement<GameObject>(nameof(ScrollWindow));
            JoinButton = GetUIElement<Button>("LeftButton");
            CreateButton = GetUIElement<Button>("RightButton");
            //Better way?
            ContentTransform = ScrollWindow.transform.GetChild(3).transform.GetChild(0).transform.GetChild(0);
        }
    }
}