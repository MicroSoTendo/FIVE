using System.Collections;
using System.Collections.Generic;
using FIVE.CameraSystem;
using FIVE.Robot;
using UnityEngine;
using static FIVE.Util;
namespace FIVE.Interactive
{
    public class NPC : MonoBehaviour
    {
        private Transform image;
        Vector3 originalScale;
        private bool onClick;

        private GameObject canvas;
        private bool isScanned = false;

        private void Awake()
        public GameObject Description;
        private void Start()
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
            foreach (Camera fpsCamera in CameraManager.GetFpsCameras)
            {
                Vector3 ndc = fpsCamera.WorldToViewportPoint(transform.position);
                if (ndc.x > 0 && ndc.x < 1 && ndc.y > 0 && ndc.y < 1 && ndc.z > 0)
                {
                    canvas.SetActive(true);
                    isScanned = true;
                }
            }
        }

        IEnumerator ChangeOverTime(float time)
        {
            
            Vector3 destinationScale = new Vector3(1.05f, 1.05f, 1.05f);

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
            if (!isScanned) return;
            onClick = true;
            StartCoroutine(ChangeOverTime(0.2f));
        }
        public void OnMouseExit()
        {
            onClick = false;
            
        }
        public void OnMouseDown()
        {
            Debug.Log("OnMouseDownClicked");
            GameObject go = Instantiate(Description) as GameObject;
            go.transform.SetParent(GameObject.Find("UIScreenSpace").transform);
            go.transform.localPosition = new Vector2(0, -192);
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
