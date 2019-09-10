namespace FIVE.UI
{
    public class UIElement<T> where T : UnityEngine.EventSystems.UIBehaviour
    {
        T UIBehavior { get; set; }
    }
}