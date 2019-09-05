using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraManager : MonoBehaviour
{
    public Camera CameraPrefab;

    public readonly Dictionary<string, Camera> Cameras = new Dictionary<string, Camera>();

    public Camera NewCamera(string id)
    {
        var cam = Instantiate(CameraPrefab);
        Cameras[id] = cam;
        return cam;
    }

    public Camera GetCamera(string id)
    {
        return Cameras[id];
    }

    private void Start()
    {
        var cam = NewCamera("deep_space");
        cam.transform.position = new Vector3(64f, 30f, 64f);
        cam.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
        cam.enabled = true;
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            foreach (var c in Cameras)
            {
                c.Value.enabled = false;
            }
            Cameras.ElementAt(Random.Range(0, Cameras.Count)).Value.enabled = true;
        }
    }
}