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

        public int Index
        {
            get => index;
            set
            {
                index = value;
                gameObject.name = $"Cell-{index}";
            }
        }

        public event Action Clicked;
        private Item item;
        private void Awake()
        {
            gameObject.GetComponentInChildren<Button>().onClick.AddListener(OnClicked);
        }

        private IEnumerator SetUpPositionRoutine()
        {
            yield return new WaitForEndOfFrame();
            RectTransform rectTransform = gameObject.GetComponentInChildren<RectTransform>();
            float cellWidth = rectTransform.rect.width;
            int totalColumns = (int)(gameObject.transform.parent.GetComponent<RectTransform>().rect.width / cellWidth);
            int x = Index % totalColumns;
            int y = Index / totalColumns;
            rectTransform.anchoredPosition = new Vector2(x * cellWidth, -y * cellWidth);
        }
        public void SetUpPosition()
        {
            StartCoroutine(SetUpPositionRoutine());
        }


        private void OnClicked()
        {
            Clicked?.Invoke();
            Destroy(gameObject);
            if (item == null)
            {
                return;
            }
            item.Use();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (item == null)
            {
                return;
            }
            item.IsRotating = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (item == null)
            {
                return;
            }

            item.IsRotating = false;
        }

        public void SetItem(Item newItem)
        {
            Transform contentTransform = gameObject.FindChildRecursive("Content").transform;
            if (newItem.transform.parent != contentTransform)
            {
                item = newItem;
                ItemInfo info = item.Info;
                Transform itemTransform = item.gameObject.transform;
                itemTransform.SetParent(contentTransform);
                item.GetComponent<Renderer>().receiveShadows = false;
                itemTransform.localScale = info.UIScale;
                itemTransform.localEulerAngles = info.UIRotation;
                itemTransform.localPosition = info.UIPosition;
            }
        }

        public void DestoryItem()
        {
            Transform contentTransform = gameObject.FindChildRecursive("Content").transform;
            contentTransform.DetachChildren();
            Destroy(item);
            item = null;
            gameObject.SetActive(false);
        }
    }
}