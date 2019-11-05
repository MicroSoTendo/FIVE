using UnityEngine;

public class CameraControlled : MonoBehaviour
{
    private void Start()
    {
        GetComponentInChildren<CameraText>().SetEnabled(false);
    }

    private void OnPreRender()
    {
        GetComponentInChildren<CameraText>().SetEnabled();
    }

    private void OnPostRender()
    {
        GetComponentInChildren<CameraText>().SetEnabled();
    }
}