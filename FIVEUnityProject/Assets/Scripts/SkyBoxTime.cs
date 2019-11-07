using System;
using UnityEngine;

namespace FIVE
{
    public class SkyBoxTime : MonoBehaviour
    {
        public TimeSpan currenttime;
        public int days;
        public Color fogday = Color.gray;
        public Color fognight = Color.black;

        public float intensity;
        [SerializeField] private GameObject lightPrefab;

        public int speed;
        private Light sun;
        private Transform sunTransform;
        public static float time;

        private void Awake()
        {
            GameObject lightGameObject = Instantiate(lightPrefab);
            sunTransform = lightGameObject.transform;
            sun = lightGameObject.GetComponent<Light>();
            time = 44000;
        }

        private void Update()
        {
            ChangeTime();
        }
        public static bool isDayTime()
        {
            if(time < 43200)
            {
                return false;
            }
            return true;
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
                intensity = 1 - (43200 - time) / 43200 * -1;
            }

            RenderSettings.fogColor = Color.Lerp(fognight, fogday, intensity * intensity);

            sun.intensity = intensity;
        }
    }
}