using System.Collections;
using FIVE.Interactive;
using UnityEngine;

namespace FIVE.UI.InGameDisplay
{
    public class ItemDialogViewModel : ViewModel<ItemDialogView, ItemDialogViewModel>
    {
        private ItemInfo info = ItemInfo.Empty;
        public void SetItemInfo(ItemInfo itemInfo)
        {
            this.info = itemInfo;
        }

        public void SetPosition(Vector3 vector)
        {
            View.DialogPanel.GetComponent<Transform>().position = vector;
        }

        public bool IsProcedureDisplayFinished { get; private set; } = false;
        public IEnumerator ProcedureDisplay()
        {
            string itemType = "Item Type: " + info.Name;
            string description = "Description: " + info.Description;

            View.ItemNameInputField.text = "";
            View.DescriptionInputField.text = "";
            for (int i = 0; i <= itemType.Length; i++)
            {
                View.ItemNameInputField.text = itemType.Substring(0, i);
                yield return null;
            }
            for (int i = 0; i <= description.Length; i++)
            {
                View.DescriptionInputField.text = description.Substring(0, i);
                yield return null;
            }

            IsProcedureDisplayFinished = true;
        }
    }
}
