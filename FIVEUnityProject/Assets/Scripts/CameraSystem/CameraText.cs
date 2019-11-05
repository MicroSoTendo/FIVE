using TMPro;
using UnityEngine;

public class CameraText : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TextMeshPro>().text = transform.parent.name;
    }

    public void SetEnabled(bool e = true)
    {
        GetComponent<TextMeshPro>().enabled = e;
        GetComponent<MeshRenderer>().enabled = e;
    }
}