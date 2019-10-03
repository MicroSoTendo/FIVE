using FIVE.Robot;
using FIVE.UI;
using FIVE.UI.InGameDisplay;
using System;
using System.Collections;
using System.Linq;
using FIVE.CameraSystem;
using UnityEngine;

namespace FIVE.Interactive
{
    public class Item : MonoBehaviour
    {
        public static readonly Color[] HighlightColors =
        {
            Color.red, Color.magenta, Color.yellow, Color.green, Color.blue, Color.green, Color.yellow,
            Color.magenta, Color.red
        };

        [SerializeField] private ItemInfo itemInfo;
        private Coroutine flashingCoroutine;
        private Renderer itemRenderer;
        private int currentColor = 0;
        private int nextColor = 0;
        private float timer = 0;
        private bool isCollected;
        private bool isFlashing;
        private Scanner scanner;
        private float singleColorInterval = 0.5f;

        public ItemInfo Info
        {
            get => itemInfo;
            set
            {
                itemInfo = value;
            }
        }

        public float ScanningSpeed { get; set; } = 1f; //TODO: Get from CPU
        public bool IsRotating { get; set; } = false;
        public bool IsPickable { get; set; }

        private void Awake()
        {
            itemRenderer = GetComponent<Renderer>();
            scanner = Instantiate(Resources.Load<GameObject>("EntityPrefabs/UI/Scanner"), transform).GetComponent<Scanner>();
        }

        private IEnumerator FlashingSelf()
        {
            while (!scanner.IsScanningFinished)
            {
                Color tintColor = Color.Lerp(HighlightColors[currentColor], HighlightColors[nextColor], timer / singleColorInterval);
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

        public void OnMouseOver()
        {
            if (!CameraManager.GetFpsCameras.First()?.enabled ?? true)
            {
                return;
            }
            if (isCollected)
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
                this.RaiseEvent<OnItemDialogRequested, ItemDialogRequestedEventArgs>(new ItemDialogRequestedEventArgs(GetComponent<Renderer>().bounds));
            }
        }

        public void OnMouseExit()
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

        private void Update()
        {
            if (!isFlashing)
            {
                return;
            }

            bool closeEnough = (FindObjectOfType<RobotSphere>().transform.position - transform.position).magnitude < 5f;
            if (!closeEnough)
            {
                return;
            }
            if (Input.GetMouseButtonDown(1))
            {
                isCollected = true;
                MeshCollider mc = GetComponent<MeshCollider>();
                if (mc != null)
                {
                    mc.enabled = false;
                    Destroy(mc);
                }
                this.RaiseEvent<OnDropItemToInventory, DropedItemToInventoryEventArgs>(
                    new DropedItemToInventoryEventArgs(gameObject, null, gameObject));
            }
        }
    }
}