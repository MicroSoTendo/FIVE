using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FIVE
{
    [InitializeOnLoad]
    public static class ToolbarExtender
    {
        private static readonly int ToolCount;

        public static readonly List<Action> DoToolbarGUI = new List<Action>();

        static ToolbarExtender()
        {
            Type toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
            const string fieldName = "k_ToolCount";
            FieldInfo toolIcons = toolbarType.GetField(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            ToolCount = toolIcons != null ? ((int)toolIcons.GetValue(null)) : 7;
            ToolbarCallback.OnToolbarGUI -= OnGUI;
            ToolbarCallback.OnToolbarGUI += OnGUI;
        }

        private static void OnGUI()
        {
            float screenWidth = EditorGUIUtility.currentViewWidth;
            float playButtonsPosition = (screenWidth - 100) / 2;
            var rect = new Rect(0, 0, screenWidth, Screen.height);
            rect.xMin += 10 + 32 * ToolCount; // Tool buttons
            rect.xMin += 20; // Spacing between tools and pivot
            rect.xMin += 64 * 2; // Pivot buttons
            rect.xMax = playButtonsPosition - 150;
            rect.y = 5;
            rect.height = 24;
            GUILayout.BeginArea(rect);
            GUILayout.BeginHorizontal();
            foreach (Action handler in DoToolbarGUI)
            {
                handler();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
