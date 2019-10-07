using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace FIVE
{
    public static class ToolbarCallback
    {
        private static readonly Type ToolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static readonly Type GUIViewType = typeof(Editor).Assembly.GetType("UnityEditor.GUIView");
        private static readonly PropertyInfo VisualTreeProperty = GUIViewType.GetProperty("visualTree",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo IMGUIContainerOnGui = typeof(IMGUIContainer).GetField("m_OnGUIHandler",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        private static ScriptableObject currentToolbar;

        public static Action OnToolbarGUI;

        static ToolbarCallback()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            // Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
            if (currentToolbar != null)
            {
                return;
            }
            // Find toolbar
            Object[] toolbars = Resources.FindObjectsOfTypeAll(ToolbarType);
            currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
            if (currentToolbar == null)
            {
                return;
            }
            // Get IMGUIContainer in visual tree
            var visualTree = (VisualElement)VisualTreeProperty.GetValue(currentToolbar, null);
            var container = (IMGUIContainer)visualTree[0];
            // Re-attach handler
            var handler = (Action)IMGUIContainerOnGui.GetValue(container);
            handler -= OnGUI;
            handler += OnGUI;
            IMGUIContainerOnGui.SetValue(container, handler);
        }

        private static void OnGUI()
        {
            OnToolbarGUI?.Invoke();
        }
    }
}
