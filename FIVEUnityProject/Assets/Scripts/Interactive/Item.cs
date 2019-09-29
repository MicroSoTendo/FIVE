using FIVE.UI;
using FIVE.UI.InGameDisplay;
using System;
using UnityEngine;

namespace FIVE.Interactive
{
    public class Item : MonoBehaviour
    {
        [Serializable]
        public struct ItemInfo
        {
            public string Name;
            public string Description;
        }


        private ItemDialogViewModel cachedVM;
        private ItemInfo itemInfo;
        public bool IsPickable { get; set; }
        public ItemInfo Info
        {
            get => itemInfo;
            set
            {
                itemInfo = value;
                cachedVM?.SetItemInfo(value);
            }
        }


        public void ShowInfo()
        {
            if (cachedVM != null)
            {
                cachedVM.SetEnabled(true);
            }
            else
            {
                cachedVM = UIManager.AddViewModel<ItemDialogViewModel>(name: gameObject.name + gameObject.GetInstanceID());
                cachedVM.SetItemInfo(Info);
                cachedVM.SetEnabled(true);
            }
        }

        public void DismissInfo()
        {
            cachedVM?.SetEnabled(false);
        }

        public void OnCursorOver()
        {
            //TODO: Shinning boundary
            //TODO: Show info
        }

        private void Update()
        {
        }
    }
}
