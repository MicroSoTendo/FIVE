using TMPro;
using UnityEngine;

public class CameraControlled : MonoBehaviour
{
    private TextMeshPro text;

    private void Start()
    {
        text = GetComponentInChildren<TextMeshPro>();
        text.text = name;
        text.ForceMeshUpdate();
        SetText(false);
    }

    private void OnPreRender()
    {
        SetText();
    }

    private void OnPostRender()
    {
        SetText(false);
    }

    private void OnDisable()
    {
        SetText(false);
    }

    private void SetText(bool e = true)
    {
        if (text != null)
            text.enabled = e;
    }
}