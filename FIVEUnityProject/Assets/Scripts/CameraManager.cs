using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera CameraPrefab;

    private readonly Dictionary<string, Camera> Cameras = new Dictionary<string, Camera>();

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
}