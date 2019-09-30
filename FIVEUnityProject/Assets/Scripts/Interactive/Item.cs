using FIVE.UI;
using FIVE.UI.InGameDisplay;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

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

        void Awake()
        {
            itemRenderer = GetComponent<Renderer>(); 
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
                itemRenderer.material.color = Color.Lerp(colors[currentColor], colors[nextColor], timer / singleColorInterval);
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
            Debug.Log(nameof(OnMouseOver));
            if (isFlashing == false)
            {
                isFlashing = true;
                flashingCoroutine = StartCoroutine(Flashing());
            }
        }

        public void OnMouseExit()
        {
            isFlashing = false;
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
