using System.Collections;
using System.Linq;
using FIVE.CameraSystem;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.Interactive
{
    public class Scanner : MonoBehaviour
    {
        private Canvas canvas;

        private GameObject cursor;
        private RectTransform cursorRectTransform;
        private Transform cursorTransform;

        private Coroutine flashingCoroutine;
        private Camera fpsCamera;

        private bool isScanning;
        private Text percentageText;
        private GameObject scannerFrame;
        private Material scannerMaterial;
        private RectTransform scannerRectTransform;
        private Transform scannerTransform;
        private Transform scanningTransform;
        private float timer;
        public float ScanningProgress { get; set; } = 0;
        public float ScanningSpeed { get; set; } = 3f;
        public bool IsScanningFinished { get; private set; } = false;
        public Color TintColor { get; set; } = Color.white;

        void Awake()
        {
            canvas = GetComponent<Canvas>();
            scannerFrame = gameObject.FindChild("Frame");
            Image scannerImage = scannerFrame.GetComponent<Image>();
            scannerMaterial = scannerImage.material;
            scannerTransform = scannerImage.transform;
            scannerRectTransform = scannerImage.rectTransform;
            percentageText = gameObject.FindChildRecursive("Percentage").GetComponent<Text>();

            cursor = gameObject.FindChild("Cursor");
            Image cursorImage = cursor.GetComponent<Image>();
            cursorTransform = cursorImage.transform;
            cursorRectTransform = cursorImage.rectTransform;

            gameObject.SetActive(false);
        }

        public void StartScanning(GameObject item)
        {
            gameObject.SetActive(true);
            flashingCoroutine = StartCoroutine(Scanning(item));
        }

        public void StopScanning()
        {
            StopCoroutine(flashingCoroutine);
            isScanning = false;
            gameObject.SetActive(false);
        }

        private static Color Invert(Color rgbColor)
        {
            Color.RGBToHSV(rgbColor, out float h, out float s, out float v);
            return Color.HSVToRGB((h + 0.5f) % 1, s, v);
        }

        private void UpdatePosition(GameObject item)
        {
            fpsCamera = CameraManager.GetPovCameras.First();
            Vector3 position = fpsCamera.WorldToScreenPoint(item.transform.position) +
                               new Vector3(0, scannerRectTransform.sizeDelta.y / 2, 0);
            scannerTransform.position = position;
            cursorTransform.position = position;
        }


        private void UpdateSize(GameObject item)
        {
            Bounds bounds = item.GetComponent<Renderer>().bounds;
            Vector3 min = fpsCamera.WorldToViewportPoint(bounds.min);
            Vector3 max = fpsCamera.WorldToViewportPoint(bounds.max);
            float width = Mathf.Abs(max.x - min.x) * fpsCamera.pixelWidth * 1.1f;
            float height = Mathf.Abs(max.y - min.y) * fpsCamera.pixelHeight * 1.1f;
            Vector2 size = Vector2.one * Mathf.Max(width, height);
            scannerRectTransform.sizeDelta = size;
            cursorRectTransform.sizeDelta = size;
        }

        private IEnumerator Scanning(GameObject item)
        {
            isScanning = true;
            while (isScanning)
            {
                UpdatePosition(item);
                UpdateSize(item);
                scannerMaterial.SetColor("_Color", TintColor);
                scannerMaterial.SetFloat("_Intensity", 1f);
                scannerMaterial.SetFloat("_ElapsedTime", Time.realtimeSinceStartup * 4f);
                timer += Time.deltaTime;
                percentageText.color = Invert(TintColor);
                if (ScanningProgress < 100f)
                {
                    ScanningProgress += Time.deltaTime * 100 / 3f * ScanningSpeed;
                    if (ScanningProgress >= 100f)
                    {
                        ScanningProgress = 100f;
                        isScanning = false;
                        timer = 0f;
                        scannerMaterial.SetFloat("_ElapsedTime", 0);
                        scannerMaterial.SetFloat("_Intensity", 0);
                        scannerMaterial.SetColor("_Color", Color.white);
                        IsScanningFinished = true;
                    }

                    percentageText.text = $"{ScanningProgress:F2}%";
                }

                yield return null;
            }
        }
    }
}