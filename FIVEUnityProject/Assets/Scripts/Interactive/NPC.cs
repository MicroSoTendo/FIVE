using FIVE.CameraSystem;
using FIVE.Robot;
using FIVE.UI;
using FIVE.UI.BSComposite;
using FIVE.UI.NPC;
using System.Collections;
using UnityEngine;
using static FIVE.EventSystem.Util;

namespace FIVE.Interactive
{
    public class NPC : MonoBehaviour
    {
        private GameObject canvas;

        public GameObject Description;
        private Transform image;
        private bool isScanned = false;
        private bool onClick;
        private Vector3 originalScale;

        private void Awake()
        {
            onClick = false;
            image = gameObject.transform.Find("Canvas").Find("Image");
            originalScale = image.localScale;
            Subscribe<OnGlobalScanFinished>(OnScanFinished);
            canvas = gameObject.transform.Find("Canvas").gameObject;
            canvas.SetActive(false);
        }

        private void OnScanFinished()
        {
            foreach (Camera fpsCamera in CameraManager.GetPovCameras)
            {
                Vector3 ndc = fpsCamera.WorldToViewportPoint(transform.position);
                if (ndc.x > 0 && ndc.x < 1 && ndc.y > 0 && ndc.y < 1 && ndc.z > 0)
                {
                    canvas.SetActive(true);
                    isScanned = true;
                }
            }
        }

        private IEnumerator ChangeOverTime(float time)
        {
            var destinationScale = new Vector3(1.05f, 1.05f, 1.05f);

            float currentTime = 0.0f;

            do
            {
                image.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
                currentTime += Time.deltaTime;
                yield return null;
            } while (currentTime <= time);
        }


        public void OnMouseOver()
        {
            if (!isScanned)
            {
                return;
            }

            onClick = true;
            StartCoroutine(ChangeOverTime(0.2f));
        }

        public void OnMouseExit()
        {
            onClick = false;
        }

        public void OnMouseDown()
        {
            if (gameObject.name.Contains("Blue"))
            {
                UIManager.Get<NPCDialogueViewModel>().IsActive = true;
            }
            if (gameObject.name.Contains("Orange"))
            {
                UIManager.Get<BSCompositeViewModel>().IsActive = true;
            }
            
        }

        public void Update()
        {
            if (!onClick)
            {
                image.localScale = originalScale;
            }
        }
    }
}