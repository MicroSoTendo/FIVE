using UnityEngine;

namespace FIVE.UI.InGameDisplay
{
    public class Cell : MonoBehaviour
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

        private void Awake()
        {
            contentTransform = gameObject.GetChildGameObject("Content").transform;
        }

        private void Update()
        {

        }
    }
}
