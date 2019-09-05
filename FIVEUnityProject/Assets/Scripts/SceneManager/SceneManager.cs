using Assets.Scripts.SceneManager;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public GameObject Ground;

    private GameObject Camera;
    private readonly Dictionary<(int, int), AreaData> Areas = new Dictionary<(int, int), AreaData>();

    private void Update()
    {
        if (Camera == null)
        {
            Camera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        if (Camera != null)
        {
            if (Time.frameCount % 60 == 0)
            {
                var x = (int)Camera.transform.position.x / AreaData.size;
                var y = (int)Camera.transform.position.z / AreaData.size;
                for (var i = -1; i <= 1; i++)
                {
                    for (var j = -1; j <= 1; j++)
                    {
                        LoadArea(x + i, y + j);
                    }
                }
            }
        }
    }

    private void LoadArea(int x, int y)
    {
        if (Areas.ContainsKey((x, y)))
        {
            return;
        }

        var pos = new Vector3(x * AreaData.size, 0f, y * AreaData.size);

        var ad = new AreaData(pos);
        Areas[(x, y)] = ad;

        var o = Instantiate(Ground, pos, Quaternion.identity);
        ad.ConstructArea(o);
    }
}