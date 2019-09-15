using UnityEngine;

public class AWSLEditor : MonoBehaviour
{
    public string code = "";

    public Texture2D EditorBackground;
    public Color EditorTextColor;

    private GUIStyle style;

    private void Start()
    {
        enabled = false;
        style = new GUIStyle
        {
            font = Font.CreateDynamicFontFromOSFont("Courier New", 16),
            fontSize = 16,
            wordWrap = true,
        };
        style.normal.textColor = EditorTextColor;
        style.normal.background = EditorBackground;
    }

    private void OnGUI()
    {
        Rect r = Screen.safeArea;
        float w = r.width, h = r.height;
        code = GUI.TextArea(new Rect(20, 40, w - 40, h - 80), code, int.MaxValue, style);
    }
}