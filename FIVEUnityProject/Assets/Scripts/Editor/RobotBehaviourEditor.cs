using FIVE.Robot;
using FIVE.RobotComponents;
using UnityEditor;
using UnityEngine;

namespace FIVE
{
    [CustomEditor(typeof(RobotBehaviour), editorForChildClasses:true)]
    internal class RobotBehaviourEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            //Rect rect = EditorGUILayout.GetControlRect();
            //SerializedProperty property = serializedObject.FindProperty("Components");
            //foreach (SerializedProperty child in property)
            //{

            //    EditorGUI.ObjectField(rect, GUIContent.none, null, typeof(RobotComponent), true);
            //}
            base.OnInspectorGUI();
        }

    }
}
