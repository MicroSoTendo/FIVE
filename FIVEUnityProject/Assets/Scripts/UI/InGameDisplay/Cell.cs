using UnityEngine;

namespace FIVE.UI.InGameDisplay
{
    public class Cell : MonoBehaviour
    {
        private Transform contentTransform;

        private GameObject item;
        public GameObject Item
        {
            get
            {
                return item;
            }
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

        void Awake()
        {
            contentTransform = gameObject.GetChildGameObject("Content").transform;
        }
        void Update()
        {
            
        }
    }
}
