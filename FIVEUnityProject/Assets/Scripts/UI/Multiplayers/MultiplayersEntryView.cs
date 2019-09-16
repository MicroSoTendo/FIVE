using UnityEngine;
using UnityEngine.UI;
namespace FIVE.UI.Multiplayers
{
    public class MultiplayersEntryView : View<MultiplayersEntryView, MultiplayersEntryViewModel>
    {
        public GameObject ScrollWindow { get; }
        public Button LeftButton { get; }
        public Button RightButton { get; }
        public Transform ContentTransform { get; }
    
        public MultiplayersEntryView()
        {
            ScrollWindow = AddUIElement<GameObject>(nameof(ScrollWindow));
            LeftButton = GetUIElement<Button>(nameof(LeftButton));
            RightButton = GetUIElement<Button>(nameof(RightButton));
            ContentTransform = ScrollWindow.transform.GetChild(3).transform.GetChild(0).transform.GetChild(0);
            LoadResources();
        }
    }
}