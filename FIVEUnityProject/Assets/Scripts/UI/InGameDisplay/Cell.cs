using System.Collections;
using FIVE.Interactive;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FIVE.UI.InGameDisplay
{
    public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Transform contentTransform;
        private GameObject item;

        public GameObject Item
        {
            get => item;
            set
            {
                if (value != null)
                {
                    value.transform.SetParent(contentTransform);
                    item = value;
                }
                else
                {
                    item.transform.SetParent(null);
                    item = null;
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            contentTransform.GetChild(0).gameObject.GetComponent<Item>().IsRotating = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            contentTransform.GetChild(0).gameObject.GetComponent<Item>().IsRotating = false;
        }

        private void Awake()
        {
            contentTransform = gameObject.FindChildRecursive("Content").transform;
        }
    }
}