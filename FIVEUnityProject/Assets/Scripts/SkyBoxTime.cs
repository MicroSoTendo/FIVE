using System;
using UnityEngine;

public class SkyBoxTime : MonoBehaviour
{
    public float time;
    public TimeSpan currenttime;
    public Transform SunTransform;
    public Light Sun;
    public int days;

    public float intensity;
    public Color fogday = Color.gray;
    public Color fognight = Color.black;

    public int speed;

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

        SunTransform.rotation = Quaternion.Euler(new Vector3((time - 21600) / 86400 * 360, 0, 0));
        if (time > 43200)
        {
            intensity = 1 - (43200 - time) / 43200;
        }
        else
        {
            intensity = 1 - ((43200 - time) / 43200 * -1);
        }

        RenderSettings.fogColor = Color.Lerp(fognight, fogday, intensity * intensity);

        Sun.intensity = intensity;
    }
}