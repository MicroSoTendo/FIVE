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

        private (byte R, byte G, byte B) color;
        private ItemDialogViewModel cachedVM;
        private Renderer renderer;
        private ItemInfo itemInfo;

        void Awake()
        {
            renderer = GetComponent<Renderer>();
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

        private IEnumerator Flashing()
        {
            while (true)
            {
                color.R = (byte)Random.Range(0, 255);
                color.G = (byte)Random.Range(0, 255);
                color.B = (byte)Random.Range(0, 255);

                renderer.material.color = new Color32(color.R, color.G, color.B, 255);
                yield return new WaitForSeconds(0.05f);
            }
        }

        public void OnMouseOver()
        {
            StartCoroutine(Flashing());
        }

        public void OnMouseExit()
        {
            StopCoroutine(Flashing());
            renderer.material.color = new Color32(255, 255, 255, 255);
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

        private void Update()
        {
        }
    }
}
