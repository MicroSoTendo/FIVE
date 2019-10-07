using UnityEditor;
using UnityEngine;

namespace FIVE
{
    [InitializeOnLoad]
    public class CompilerOptionsEditor
    {
        private const string ButtonText = "Hack Unity Reload\u2620";
        private const string PrefsKey = "IsReloadLocked";
        private static GUIContent content;
        private static GUIStyle style;
        private static Texture2D image_h;
        private static Texture2D image_n;
        private static Texture2D image_p;
        private static bool initialized = false;
        static CompilerOptionsEditor()
        {
            EditorPrefs.SetBool(PrefsKey, false);
            ToolbarExtender.DoToolbarGUI.Add(OnToolbarGUI);
            image_h = Resources.Load<Texture2D>("Textures/UI/Btn_OK_h");
            image_n = Resources.Load<Texture2D>("Textures/UI/Btn_OK_n");
            image_p = Resources.Load<Texture2D>("Textures/UI/Btn_OK_p");
        }

        private static void Initialize()
        {
            content = new GUIContent(ButtonText, "Toggle Unity's auto reload.");
            style = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                font = Font.CreateDynamicFontFromOSFont("Segoe UI Emoji", 12),
                fixedWidth = 175,
                fixedHeight = 24,
                normal = { background = image_n },
                hover = { background = image_h },
                active = { background = image_p },
                onNormal = { background = image_n },
                focused = { background = image_p },
                onHover = { background = image_h },
                onActive = { background = image_n },
                onFocused = { background = image_n },
                border = new RectOffset(0, 0, 0, 0),
            };
            style.normal.textColor = Color.white;
            style.hover.textColor = Color.white;
            style.active.textColor = Color.magenta;
            initialized = true;
        }
        private static void OnToolbarGUI()
        {
            if (!initialized) Initialize();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(content, style))
            {
                if (EditorPrefs.GetBool(PrefsKey))
                {
                    EditorApplication.UnlockReloadAssemblies();
                    EditorPrefs.SetBool(PrefsKey, false);
                    Debug.Log("Unlocked compile");
                }
                else
                {
                    EditorApplication.LockReloadAssemblies();
                    EditorPrefs.SetBool(PrefsKey, true);
                    Debug.Log("Locked compile");
                }
            }
        }
    }
}
