using FIVE.UI;
using FIVE.UI.InGameDisplay;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
        private Renderer itemRenderer;
        private ItemInfo itemInfo;
        private Coroutine flashingCoroutine;
        private bool isFlashing;
        private Material scannerMaterial;
        private Transform scannerTransform;
        private RectTransform scannerRectTransform;
        private GameObject scanner;

        void Awake()
        {
            itemRenderer = GetComponent<Renderer>();
            scanner = Instantiate(Resources.Load<GameObject>("EntityPrefabs/UI/Scanner"));
            scanner.transform.SetParent(transform);
            scanner = GetComponentInChildren<Canvas>().gameObject;
            scannerMaterial = GetComponentInChildren<Image>().material;
            scannerTransform = GetComponentInChildren<Image>().transform;
            scannerRectTransform = GetComponentInChildren<Image>().rectTransform;
            scanner.SetActive(false);
        }

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

        private readonly Color[] colors = {Color.red, Color.magenta, Color.yellow, Color.green, Color.blue, Color.green, Color.yellow, Color.magenta, Color.red};
        private int currentColor = 0;
        private int nextColor = 0;
        private float singleColorInterval = 0.5f;
        private float timer = 0;
        private IEnumerator Flashing()
        {
            while (isFlashing)
            {
                Color c = Color.Lerp(colors[currentColor], colors[nextColor], timer / singleColorInterval);
                itemRenderer.material.color = c;
                scannerMaterial.SetColor("_Color",c);
                timer += Time.deltaTime;
                if (timer > singleColorInterval)
                {
                    currentColor = (currentColor + 1) % colors.Length;
                    nextColor = (currentColor + 1) % colors.Length;
                    timer = 0.0f;
                }
                yield return null;
            }
        }

        public void OnMouseOver()
        {
            if (isFlashing == false)
            {
                scanner.SetActive(true);
                Vector2 size = GetScanFrameSize();
                scannerRectTransform.sizeDelta = size;
                scannerTransform.position = Camera.current.WorldToScreenPoint(transform.position) + new Vector3(0,size.y/2,0);
                isFlashing = true;
                flashingCoroutine = StartCoroutine(Flashing());
            }
        }

        private Vector2 GetScanFrameSize()
        {
            Bounds bounds = itemRenderer.bounds;
            Vector3 min = Camera.current.WorldToScreenPoint(bounds.min);
            Vector3 max = Camera.current.WorldToScreenPoint(bounds.max);
            float width = Mathf.Abs(max.x - min.x);
            float height = Mathf.Abs(max.y - min.y);
            float size = Math.Max(width, height);
            return new Vector2(size,size);
        }

        public void OnMouseExit()
        {
            isFlashing = false;
            scanner.SetActive(false);
            StopCoroutine(flashingCoroutine);
            itemRenderer.material.color = new Color32(255, 255, 255, 255);
        }

        private void ShowInfo()
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

    }
}
