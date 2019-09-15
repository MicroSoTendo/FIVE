namespace FIVE.UI
{
    public class UIElement<T> where T : UnityEngine.EventSystems.UIBehaviour
    {
        private T UIBehavior { get; set; }
    }
}