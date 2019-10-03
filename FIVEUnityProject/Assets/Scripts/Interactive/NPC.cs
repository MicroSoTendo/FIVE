using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Interactive
{
    public class NPC : MonoBehaviour
    {
        private Transform image;
        public GameObject Description;
        Vector3 originalScale;
        private bool onClick;
        private void Start()
        {
            onClick = false;
            Description.SetActive(false);
            image = gameObject.transform.Find("Canvas").Find("Image");
            originalScale = image.localScale;
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
            onClick = true; onClick = true;
            StartCoroutine(ChangeOverTime(0.2f));
        }
        public void  OnMouseExit()
        {
            onClick = false;
            
        }
        public void OnMouseDown()
        {
            Description.SetActive(true);
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
