using FIVE.CameraSystem;
using FIVE.EventSystem;
using FIVE.UI.InGameDisplay;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
namespace FIVE.Interactive
{
    public class Item : MonoBehaviour
    {
        public delegate void ItemEvent(Item item);

        public static readonly Color[] HighlightColors =
        {
            Color.red, Color.magenta, Color.yellow, Color.green, Color.blue, Color.green, Color.yellow,
            Color.magenta, Color.red
        };

        private int currentColor = 0;
        private Coroutine flashingCoroutine;
        private bool isFlashing;

        [SerializeField] private ItemInfo itemInfo;
        private Renderer itemRenderer;
        private int nextColor = 0;
        private Scanner scanner;
        private float singleColorInterval = 0.5f;
        private float timer = 0;

        public ItemInfo Info
        {
            get => itemInfo;
            set => itemInfo = value;
        }

        public bool IsRotating { get; set; } = false;
        public bool Collected { get; set; } = false;
        public Action ItemUseAction { get; set; }

        public static event ItemEvent LeftClicked;
        public static event ItemEvent ItemUsed;

        private void Awake()
        {
            itemRenderer = GetComponent<Renderer>();
            scanner = Instantiate(Resources.Load<GameObject>("EntityPrefabs/UI/Scanner"), transform)
                .GetComponent<Scanner>();
        }

        public void Use()
        {
            ItemUseAction?.Invoke();
        }

        private IEnumerator FlashingSelf()
        {
            while (!scanner.IsScanningFinished)
            {
                var tintColor = Color.Lerp(HighlightColors[currentColor], HighlightColors[nextColor],
                    timer / singleColorInterval);
                itemRenderer.material.color = tintColor;
                scanner.TintColor = tintColor;
                timer += Time.deltaTime;
                if (timer > singleColorInterval)
                {
                    currentColor = (currentColor + 1) % HighlightColors.Length;
                    nextColor = (currentColor + 1) % HighlightColors.Length;
                    timer = 0.0f;
                }

                yield return null;
            }
        }

        private void OnMouseOver()
        {
            if (!CameraManager.GetPovCameras.Any(c => c.enabled))
            {
                return;
            }

            if (Collected)
            {
                return;
            }

            if (isFlashing == false)
            {
                scanner.StartScanning(gameObject);
                isFlashing = true;
                flashingCoroutine = StartCoroutine(FlashingSelf());
            }

            if (scanner.IsScanningFinished)
            {
                this.RaiseEvent<OnItemDialogRequested, ItemDialogRequestedEventArgs>(
                    new ItemDialogRequestedEventArgs(gameObject));
            }

            if (Input.GetMouseButtonDown(0))
            {
                LeftClicked?.Invoke(this);
            }
        }

        private void OnMouseExit()
        {
            if (!isFlashing)
            {
                return;
            }

            isFlashing = false;
            scanner.StopScanning();
            itemRenderer.material.color = new Color32(255, 255, 255, 255);
            if (flashingCoroutine != null)
            {
                StopCoroutine(flashingCoroutine);
            }

            this.RaiseEvent<OnItemDialogDismissRequested>();
        }

        private void LateUpdate()
        {
            if (IsRotating)
            {
                transform.localEulerAngles += new Vector3(0, Time.deltaTime * 90f, 0);
            }
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}