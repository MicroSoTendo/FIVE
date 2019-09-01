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
        public override void OnInspectorGUI()
        {
            var script = target as EventSystemWrapper;
            script.RunningMode = (EventSystem.RunningMode)EditorGUILayout.EnumPopup("Running Mode", script.RunningMode);
            if (script.RunningMode == EventSystem.RunningMode.Asynchronous)
            {
                script.AsynchronousTimeOut = EditorGUILayout.IntField("Asynchronous Time Out", script.AsynchronousTimeOut);
            }
        }
    }
}
