using System;
using System.Collections;
using System.Linq;
using FIVE.CameraSystem;
using FIVE.EventSystem;
using FIVE.Interactive;
using TMPro;
using UnityEngine;

namespace FIVE.UI.InGameDisplay
{
    public class ItemDialogRequestedEventArgs : EventArgs
    {
        public GameObject Item { get; }
        public ItemDialogRequestedEventArgs(GameObject item)
        {
            this.Item = item;
        }
    }
    public abstract class OnItemDialogDismissRequested : IEventType { }
    public abstract class OnItemDialogRequested : IEventType<ItemDialogRequestedEventArgs> { }
    public class ItemDialogViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/ItemDialog";
        public TMP_InputField ItemNameInputField { get; }
        public TMP_InputField DescriptionInputField { get; }
        public ItemDialogViewModel()
        {
            ItemNameInputField = Get<TMP_InputField>(nameof(ItemNameInputField));
            DescriptionInputField = Get<TMP_InputField>(nameof(DescriptionInputField));
            EventManager.Subscribe<OnItemDialogRequested, ItemDialogRequestedEventArgs>(OnItemDialogRequested);
            EventManager.Subscribe<OnItemDialogDismissRequested>(OnDismiss);
            base.IsActive = false;
        }

        private void OnDismiss(object sender, EventArgs e)
        {
            base.IsActive = false;
            isUpdate = false;
        }

        private bool isUpdate = false;
        private void OnItemDialogRequested(object sender, ItemDialogRequestedEventArgs e)
        {
            base.IsActive = false;
            isUpdate = true;
            MainThreadDispatcher.ScheduleCoroutine(UpdatePosition(e.Item));
            MainThreadDispatcher.ScheduleCoroutine(ProcedureDisplayText.Routine(ItemNameInputField.textComponent, Callback));
        }

        private void Callback()
        {
            MainThreadDispatcher.ScheduleCoroutine(ProcedureDisplayText.Routine(DescriptionInputField.textComponent, null));
        }

        private float GetCoordWithBound(float min, float max, float range, float offset)
        {
            float f;
            if (min > 1f)
            {
                f = range - offset;
            }
            else if (max < 0f)
            {
                f = 0;
            }
            else
            {
                if (max < 0.5f)
                {
                    f = max * range;
                }
                else
                {
                    f = min * range - offset;
                }
            }
            return f;
        }

        private IEnumerator UpdatePosition(GameObject item)
        {
            Camera fpsCamera = CameraManager.GetFpsCameras.First();
            Renderer itemRenderer = item.GetComponent<Renderer>();
            while (isUpdate)
            {
                Bounds itemBounds = itemRenderer.bounds;
                Vector3 itemNDCMin = fpsCamera.WorldToViewportPoint(itemBounds.min);
                Vector3 itemNDCMax = fpsCamera.WorldToViewportPoint(itemBounds.max);
                float left = Mathf.Min(itemNDCMin.x, itemNDCMax.x);
                float right = Mathf.Max(itemNDCMin.x, itemNDCMax.x);
                float bottom = Mathf.Min(itemNDCMin.y, itemNDCMax.y);
                float top = Mathf.Max(itemNDCMin.y, itemNDCMax.y);
                float x = GetCoordWithBound(left,right,fpsCamera.pixelWidth, DescriptionInputField.GetComponent<RectTransform>().rect.width);
                float y = GetCoordWithBound(bottom,top,fpsCamera.pixelHeight, 0);
                Root.transform.position = new Vector3(x, y, 0);
                yield return null;
            }
        }

        public bool IsProcedureDisplayFinished { get; private set; } = false;

        private IEnumerator ProcedureDisplay(GameObject item)
        {
            //Show words
            ItemInfo info = item.GetComponent<Item>().Info;
            string itemType = "Item Type: " + info.Name;
            string description = "Description: " + info.Description;
            ItemNameInputField.text = "";
            DescriptionInputField.text = "";
            for (int i = 0; i <= itemType.Length; i++)
            {
                ItemNameInputField.text = itemType.Substring(0, i);
                yield return null;
            }
            for (int i = 0; i <= description.Length; i++)
            {
                DescriptionInputField.text = description.Substring(0, i);
                yield return null;
            }

            IsProcedureDisplayFinished = true;
        }

    }
}
