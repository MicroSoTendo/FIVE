using System;
using UnityEngine;

namespace FIVE
{
    public class SkyBoxTime : MonoBehaviour
    {
        [SerializeField] private GameObject lightPrefab;
        public float time;
        public TimeSpan currenttime;
        private Transform sunTransform;
        private Light sun;
        public int days;

        public float intensity;
        public Color fogday = Color.gray;
        public Color fognight = Color.black;

        public int speed;

        private void Awake()
        {
            GameObject lightGameObject = Instantiate(lightPrefab);
            sunTransform = lightGameObject.transform;
            sun = lightGameObject.GetComponent<Light>();
        }

        private void Update()
        {
            ChangeTime();
        }

        public void ChangeTime()
        {
            time += Time.deltaTime * speed;
            if (time > 86400)
            {
                days += 1;
                time = 0;
            }

            currenttime = TimeSpan.FromSeconds(time);

            sunTransform.rotation = Quaternion.Euler(new Vector3((time - 21600) / 86400 * 360, 0, 0));
            if (time > 43200)
            {
                intensity = 1 - (43200 - time) / 43200;
            }
            else
            {
                intensity = 1 - ((43200 - time) / 43200 * -1);
            }

            RenderSettings.fogColor = Color.Lerp(fognight, fogday, intensity * intensity);

            sun.intensity = intensity;
        }
    }
}