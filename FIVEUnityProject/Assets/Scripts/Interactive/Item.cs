using FIVE.UI;
using FIVE.UI.InGameDisplay;
using System;
using System.Collections;
using FIVE.EventSystem;
using FIVE.Robot;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.Interactive
{
    public class Item : MonoBehaviour
    {
        private readonly Color[] colors =
        {
            Color.red, Color.magenta, Color.yellow, Color.green, Color.blue, Color.green, Color.yellow,
            Color.magenta, Color.red
        };

        [SerializeField] private ItemInfo itemInfo;
        private ItemDialogViewModel cachedVM;
        private int currentColor = 0;
        private bool finishedScanning;
        private Coroutine flashingCoroutine;
        private bool isCollected;
        private bool isFlashing;
        private Renderer itemRenderer;
        private int nextColor = 0;
        private Text percentageText;
        private GameObject scanner;
        private Material scannerMaterial;
        private RectTransform scannerRectTransform;
        private Transform scannerTransform;
        private float scanningProgress;
        private float singleColorInterval = 0.5f;
        private float timer = 0;

        public ItemInfo Info
        {
            get => itemInfo;
            set
            {
                itemInfo = value;
                cachedVM?.SetItemInfo(value);
            }
        }

        public float ScanningSpeed { get; set; } = 1f; //TODO: Get from CPU
        public bool IsRotating { get; set; } = false;

        public bool IsPickable { get; set; }

        private void Awake()
        {
            itemRenderer = GetComponent<Renderer>();
            scanner = Instantiate(Resources.Load<GameObject>("EntityPrefabs/UI/Scanner"));
            scanner.transform.SetParent(transform);
            scanner = GetComponentInChildren<Canvas>().gameObject;
            scannerMaterial = GetComponentInChildren<Image>().material;
            scannerTransform = GetComponentInChildren<Image>().transform;
            scannerRectTransform = GetComponentInChildren<Image>().rectTransform;
            percentageText = GetComponentInChildren<Text>();
            scanner.SetActive(false);
            scanningProgress = 0;
            finishedScanning = false;
        }

        private static Color Invert(Color rgbColor)
        {
            Color.RGBToHSV(rgbColor, out float h, out float s, out float v);
            return Color.HSVToRGB((h + 0.5f) % 1, s, v);
        }

        private IEnumerator Flashing()
        {
            while (isFlashing && !finishedScanning)
            {
                Color tintColor = Color.Lerp(colors[currentColor], colors[nextColor], timer / singleColorInterval);
                itemRenderer.material.color = tintColor;
                scannerMaterial.SetColor("_Color", tintColor);
                scannerMaterial.SetFloat("_Intensity", 1f);
                scannerMaterial.SetFloat("_ElapsedTime", Time.realtimeSinceStartup * 4f);
                timer += Time.deltaTime;
                if (timer > singleColorInterval)
                {
                    currentColor = (currentColor + 1) % colors.Length;
                    nextColor = (currentColor + 1) % colors.Length;
                    timer = 0.0f;
                }

                percentageText.color = Invert(tintColor);
                if (scanningProgress < 100f)
                {
                    scanningProgress += Time.deltaTime * 100 / 3f * ScanningSpeed;
                    if (scanningProgress >= 100f)
                    {
                        scanningProgress = 100f;
                        ShowInfo();
                    }

                    percentageText.text = $"{scanningProgress:F2}%";
                }
                else
                {
                    timer = 0f;
                    scannerMaterial.SetFloat("_ElapsedTime", timer);
                    scannerMaterial.SetFloat("_Intensity", 0);
                    finishedScanning = true;
                    scannerMaterial.SetColor("_Color", Color.white);
                }

                yield return null;
            }
        }

        public void OnMouseOver()
        {
            //TODO: Refactor it later
            if (!Camera.current?.name.Contains("fps") ?? true)
            {
                return;
            }

            if (isCollected)
            {
                return;
            }

            if (isFlashing == false)
            {
                scanner.SetActive(true);
                SetScannerSize();
                SetScannerPosition();
                isFlashing = true;
                flashingCoroutine = StartCoroutine(Flashing());
            }

            if (finishedScanning)
            {
                ShowInfo();
            }
        }

        private void SetScannerPosition()
        {
            scannerTransform.position = Camera.current.WorldToScreenPoint(transform.position) +
                                        new Vector3(0, scannerRectTransform.sizeDelta.y / 2, 0);
        }

        private void SetScannerSize()
        {
            Bounds bounds = itemRenderer.bounds;
            Vector3 min = Camera.current.WorldToScreenPoint(bounds.min);
            Vector3 max = Camera.current.WorldToScreenPoint(bounds.max);
            float width = Mathf.Abs(max.x - min.x);
            float height = Mathf.Abs(max.y - min.y);
            float size = Math.Max(width, height);
            scannerRectTransform.sizeDelta = new Vector2(size, size);
        }

        private void SetVMPosition()
        {
            //TODO: Better way to find position?
            float x = scannerTransform.position.x + scannerRectTransform.sizeDelta.x;
            float y = scannerRectTransform.position.y + scannerRectTransform.sizeDelta.y;
            if (scannerTransform.position.x + scannerRectTransform.sizeDelta.x + 300 > Screen.width)
            {
                x = scannerTransform.position.x - 300;
            }

            if (scannerRectTransform.position.y + scannerRectTransform.sizeDelta.y + 300 > Screen.height)
            {
                y = scannerTransform.position.y - 300;
            }

            cachedVM.SetPosition(new Vector3(x, y, 0));
        }

        public void OnMouseExit()
        {
            if (!isFlashing)
            {
                return;
            }

            isFlashing = false;
            scanner.SetActive(false);
            itemRenderer.material.color = new Color32(255, 255, 255, 255);
            DismissInfo();
            if (!finishedScanning)
            {
                StopCoroutine(flashingCoroutine);
            }
        }

        private void ShowInfo()
        {
            if (isCollected)
            {
                return;
            }

            if (cachedVM != null)
            {
                SetVMPosition();
                cachedVM.SetEnabled(true);
                if (!cachedVM.IsProcedureDisplayFinished)
                {
                    StartCoroutine(cachedVM.ProcedureDisplay());
                }
            }
            else
            {
                cachedVM = UIManager.AddViewModel<ItemDialogViewModel>(gameObject.name + gameObject.GetInstanceID());
                SetVMPosition();
                cachedVM.SetItemInfo(itemInfo);
                cachedVM.SetEnabled(true);
                StartCoroutine(cachedVM.ProcedureDisplay());
            }
        }

        public void DismissInfo()
        {
            if (!isCollected)
            {
                cachedVM?.SetEnabled(false);
            }
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

            //TODO: Refactor this later
            bool closeEnough = (FindObjectOfType<RobotSphere>().transform.position - transform.position).magnitude <
                               100f;
            if (!closeEnough)
            {
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                DismissInfo();
                //TODO: Change to event based
                UIManager.RemoveViewModel(gameObject.name + gameObject.GetInstanceID());
                //TODO: external setting collecting state
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