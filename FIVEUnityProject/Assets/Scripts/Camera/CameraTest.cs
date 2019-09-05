using UnityEngine;
using System.Collections;

public class CameraTest : MonoBehaviour
{
    private Color col;
    private void Start()
    {
        col = this.GetComponent<Renderer>().material.color;
    }
    void OnMouseOver()
    {
        this.GetComponent<Renderer>().material.color = Color.red;
        CameraPosition.Instance.position = this.transform;
    }

    void OnMouseExit()
    {
        this.GetComponent<Renderer>().material.color = col;
        CameraPosition.Instance.position = null;
    }
}
