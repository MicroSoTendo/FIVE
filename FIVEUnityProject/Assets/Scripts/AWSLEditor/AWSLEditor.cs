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
        if (!enabled)
        {
            return;
        }

        Rect r = Screen.safeArea;
        float w = r.width, h = r.height;
        code = GUI.TextArea(new Rect(20, 40, w - 100, h - 120), code, int.MaxValue, style);
        if (GUI.Button(new Rect(20, h - 80, w - 100, 60), "Done!"))
        {
            enabled = false;
        }
    }
}