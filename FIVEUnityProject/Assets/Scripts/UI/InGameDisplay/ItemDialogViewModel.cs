using FIVE.Interactive;

namespace FIVE.UI.InGameDisplay
{
    public class ItemDialogViewModel : ViewModel<ItemDialogView, ItemDialogViewModel>
    {
        public void SetItemInfo(Item.ItemInfo info)
        {
            View.ItemName.text = info.Name;
            View.Description.text = info.Description;
        }
    }
}
