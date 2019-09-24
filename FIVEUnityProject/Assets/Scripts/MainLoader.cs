using FIVE.EventSystem;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FIVE
{
    public class MainLoader : MonoBehaviour
    {
        [HideInInspector]
        public List<GameObject> InfrastructuresOnAwake;
        [HideInInspector]
        public List<GameObject> InfrastructuresOnStart;

        private void Awake()
        {
            this.RaiseEvent<OnMainLoaderAwake>(EventArgs.Empty);
            InfrastructuresOnAwake.ForEach(v =>
            {
                Debug.Log(v.name + " Loaded");
                DontDestroyOnLoad(Instantiate(v));
            });
        }

        private IEnumerator Start()
        {
            this.RaiseEvent<OnMainLoaderStart>(EventArgs.Empty);
            foreach (GameObject prefab in InfrastructuresOnStart)
            {
                DontDestroyOnLoad(Instantiate(prefab));
                yield return null;
            }
            Destroy(this);
            this.RaiseEvent<OnMainLoaderDestroyed>(EventArgs.Empty);
        }
    }

    [CustomEditor(typeof(MainLoader))]
    public class MainLoaderEditor : Editor
    {
        private ReorderableList onAwakeLoadingList;
        private ReorderableList onStartLoadingList;

        private int onAwakePickerWindowId;
        private int onStartPickerWindowId;

        private MainLoader MainLoader => (MainLoader)target;

        private void OnEnable()
        {
            onAwakeLoadingList = new ReorderableList(MainLoader.InfrastructuresOnAwake, typeof(GameObject), true, true, true, true);
            onStartLoadingList = new ReorderableList(MainLoader.InfrastructuresOnStart, typeof(GameObject), true, true, true, true);
            onAwakeLoadingList.drawHeaderCallback += rect => { GUI.Label(rect, "Loading on Awake"); };
            onStartLoadingList.drawHeaderCallback += rect => { GUI.Label(rect, "Loading on Start"); };

            onAwakeLoadingList.onAddCallback += OnAdd;
            onStartLoadingList.onAddCallback += OnAdd;


            onAwakeLoadingList.onRemoveCallback += RemoveItem;
            onStartLoadingList.onRemoveCallback += RemoveItem;
        }

        private void OnDisable()
        {
            onAwakeLoadingList.drawHeaderCallback = null;
            onAwakeLoadingList.onAddCallback = null;
            onAwakeLoadingList.onRemoveCallback = null;

            onStartLoadingList.drawHeaderCallback = null;
            onStartLoadingList.onAddCallback = null;
            onStartLoadingList.onRemoveCallback = null;
        }

        private void OnAdd(ReorderableList list)
        {
            if (list == onAwakeLoadingList)
            {
                onAwakePickerWindowId = GUIUtility.GetControlID(FocusType.Passive) + 100;
                EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, null, onAwakePickerWindowId);
            }

            if (list == onStartLoadingList)
            {
                onStartPickerWindowId = GUIUtility.GetControlID(FocusType.Passive) + 101;
                EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, null, onStartPickerWindowId);
            }

            EditorUtility.SetDirty(target);
        }

        private void RemoveItem(ReorderableList list)
        {
            if(list == onAwakeLoadingList)
            {
                MainLoader.InfrastructuresOnAwake.RemoveAt(list.index);
            }

            if(list == onStartLoadingList)
            {
                MainLoader.InfrastructuresOnStart.RemoveAt(list.index);
            }

            EditorUtility.SetDirty(target);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            onAwakeLoadingList.DoLayoutList();
            GUILayout.Space(10);
            onStartLoadingList.DoLayoutList();
            GUILayout.Space(10);

            if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == onAwakePickerWindowId)
            {
                var selectedObject = (GameObject)EditorGUIUtility.GetObjectPickerObject();
                onAwakePickerWindowId = -1;
                MainLoader.InfrastructuresOnAwake.Add(selectedObject);
            }

            if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == onStartPickerWindowId)
            {
                var selectedObject = (GameObject)EditorGUIUtility.GetObjectPickerObject();
                onStartPickerWindowId = -1;
                MainLoader.InfrastructuresOnStart.Add(selectedObject);
            }
        }
    }
}