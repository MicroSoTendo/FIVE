using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.EventSystem
{
    public class EventSystemWrapper : MonoBehaviour
    {
        public EventSystem.RunningMode RunningMode;
        public int AsynchronousTimeOut;
        public event Action OnUpdate;

        private void Start()
        {
            EventSystem.Initialize(this, RunningMode);
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

    }

    [CustomEditor(typeof(EventSystemWrapper))]
    public class EventSystemWrapperEditor : Editor
    {
        public SerializedProperty RunningModeProperty;
        public SerializedProperty AsynchronousTimeOutProperty;

        public void OnEnable()
        {
            RunningModeProperty = serializedObject.FindProperty("RunningMode");
            AsynchronousTimeOutProperty = serializedObject.FindProperty("AsynchronousTimeOut");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(RunningModeProperty);
            var mode = (EventSystem.RunningMode) RunningModeProperty.enumValueIndex;
            if (mode == EventSystem.RunningMode.Asynchronous)
            {
                EditorGUILayout.PropertyField(AsynchronousTimeOutProperty, new GUIContent("Asynchronous Time Out"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
