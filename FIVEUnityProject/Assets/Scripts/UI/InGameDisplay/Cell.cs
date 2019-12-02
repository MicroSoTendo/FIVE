using FIVE.Interactive;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FIVE.UI.InGameDisplay
{
    public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private int index;

        public Action Clicked = () => {};
        public Item Item { get; private set; }
        private void Awake()
        {
            gameObject.GetComponentInChildren<Button>().onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            Clicked?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Item == null)
            {
                return;
            }
            Item.IsRotating = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Item == null)
            {
                return;
            }
            Item.IsRotating = false;
        }

        public void SetItem(Item newItem)
        {
            Transform contentTransform = gameObject.FindChildRecursive("Content").transform;
            if (newItem.transform.parent != contentTransform)
            {
                Item = newItem;
                ItemInfo info = Item.Info;
                Transform itemTransform = Item.gameObject.transform;
                itemTransform.SetParent(contentTransform);
                Item.GetComponent<Renderer>().receiveShadows = false;
                itemTransform.localScale = info.UIScale;
                itemTransform.localEulerAngles = info.UIRotation;
                itemTransform.localPosition = info.UIPosition;
            }
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}