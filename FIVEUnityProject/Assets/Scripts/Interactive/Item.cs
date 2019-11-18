using FIVE.CameraSystem;
using FIVE.EventSystem;
using FIVE.Robot;
using FIVE.UI.InGameDisplay;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace FIVE.Interactive
{
    internal class Item : MonoBehaviour
    {
        public delegate void ItemUsingAction(GameObject item);

        public static readonly Color[] HighlightColors =
        {
            Color.red, Color.magenta, Color.yellow, Color.green, Color.blue, Color.green, Color.yellow,
            Color.magenta, Color.red
        };

        private int currentColor = 0;
        private Coroutine flashingCoroutine;
        private bool isCollected;
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

        public float ScanningSpeed { get; set; } = 1f; //TODO: Get from CPU
        public bool IsRotating { get; set; } = false;
        public bool IsPickable { get; set; }

        public ItemUsingAction ItemAction { get; set; }
        public bool ActionExecuted { get; set; } = false;

        private void Awake()
        {
            itemRenderer = GetComponent<Renderer>();
            scanner = Instantiate(Resources.Load<GameObject>("EntityPrefabs/UI/Scanner"), transform)
                .GetComponent<Scanner>();
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
                this.RaiseEvent<OnItemDialogRequested, ItemDialogRequestedEventArgs>(
                    new ItemDialogRequestedEventArgs(gameObject));
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

        private void Update()
        {
            if (IsRotating)
            {
                if (Input.GetMouseButtonDown(1) && !ActionExecuted)
                {
                    ItemAction?.Invoke(gameObject);
                    ActionExecuted = true;
                    this.RaiseEvent<OnRemoveItemRequested, RemoveItemRequestedEventArgs>(
                        new RemoveItemRequestedEventArgs(gameObject));
                }
            }

            if (!isFlashing)
            {
                return;
            }
            bool closeEnough = (RobotManager.ActiveRobot.transform.position - transform.position).magnitude < 100f;
            if (!closeEnough)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                DropToInventory();
            }
        }

        public void DropToInventory()
        {
            isCollected = true;
            MeshCollider mc = GetComponent<MeshCollider>();
            if (mc != null)
            {
                mc.enabled = false;
                Destroy(mc);
            }

            GameObject o = gameObject;
            EventManager.RaiseImmediate<OnDropItemToInventory>(this, new DropedItemToInventoryEventArgs(o, null, o));
        }
    }
}