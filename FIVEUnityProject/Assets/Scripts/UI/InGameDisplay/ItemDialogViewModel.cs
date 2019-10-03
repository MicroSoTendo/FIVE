using System;
using System.Collections;
using System.Linq;
using FIVE.CameraSystem;
using FIVE.EventSystem;
using FIVE.Interactive;
using UnityEngine;

namespace FIVE.UI.InGameDisplay
{
    public class ItemDialogRequestedEventArgs : EventArgs
    {
        public Bounds ItemRenderBounds { get; }
        public ItemDialogRequestedEventArgs(Bounds bounds)
        {
            ItemRenderBounds = bounds;
        }
    }
    public abstract class OnItemDialogDismissRequested : IEventType { }
    public abstract class OnItemDialogRequested : IEventType<ItemDialogRequestedEventArgs> { }
    public class ItemDialogViewModel : ViewModel<ItemDialogView, ItemDialogViewModel>
    {
        public ItemDialogViewModel()
        {
            EventManager.Subscribe<OnItemDialogRequested, ItemDialogRequestedEventArgs>(OnItemDialogRequested);
            EventManager.Subscribe<OnItemDialogDismissRequested>(OnDismiss);
            base.SetEnabled(false);
        }

        private void OnDismiss(object sender, EventArgs e)
        {
            base.SetEnabled(false);
        }

        private void OnItemDialogRequested(object sender, ItemDialogRequestedEventArgs e)
        {
            MainThreadDispatcher.ScheduleCoroutine(ProcedureDisplay(e.ItemRenderBounds));
        }

        private ItemInfo info = ItemInfo.Empty;
        public void SetItemInfo(ItemInfo itemInfo)
        {
            this.info = itemInfo;
        }

        public bool IsProcedureDisplayFinished { get; private set; } = false;

        private IEnumerator ProcedureDisplay(Bounds itemBounds)
        {
            SetEnabled(true);
            yield return new WaitForEndOfFrame();
            SetDialogPosition(itemBounds);
            yield return null;
            //Show words
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

        private void SetDialogPosition(Bounds itemBounds)
        {
            Camera fpsCamera = CameraManager.GetFpsCameras.First();
            Vector3 itemNDCMin = fpsCamera.WorldToViewportPoint(itemBounds.min);
            Vector3 itemNDCMax = fpsCamera.WorldToViewportPoint(itemBounds.max);
            float left = Mathf.Min(itemNDCMin.x, itemNDCMax.x);
            float right = Mathf.Max(itemNDCMin.x, itemNDCMax.x);
            float bottom = Mathf.Min(itemNDCMin.y, itemNDCMax.y);
            float top = Mathf.Max(itemNDCMin.y, itemNDCMax.y);

            float x, y;

            if (left > 1f)
            {
                x = fpsCamera.pixelWidth - View.DialogPanel.GetComponent<RectTransform>().rect.width;
            }
            else if (right < 0f)
            {
                x = 0;
            }
            else
            {
                if (right < 0.5f)
                {
                    x = right * fpsCamera.pixelWidth;
                }
                else
                {
                    x = left * fpsCamera.pixelWidth - View.DialogPanel.GetComponent<RectTransform>().rect.width;
                }
            }

            if (bottom > 1f)
            {
                y = fpsCamera.pixelHeight - View.DialogPanel.GetComponent<RectTransform>().rect.height;
            }
            else if (top < 0f)
            {
                y = 0f;
            }
            else
            {
                if (top < 0.5f)
                {
                    y = top * fpsCamera.pixelHeight;
                }
                else
                {
                    y = bottom * fpsCamera.pixelHeight - View.DialogPanel.GetComponent<RectTransform>().rect.height;
                }
            }
            View.DialogPanel.GetComponent<Transform>().position = new Vector3(x, y, 0);
        }
    }
}
