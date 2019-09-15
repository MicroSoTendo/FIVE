using UnityEngine;

public class AWSLEditor : MonoBehaviour
{
    private string code = "";

    private void Start()
    {
        enabled = false;
    }

    private void OnGUI()
    {
        Rect r = Screen.safeArea;
        float w = r.width, h = r.height;
        code = GUI.TextArea(new Rect(20, 40, w - 40, h - 80), code, int.MaxValue);
    }
}