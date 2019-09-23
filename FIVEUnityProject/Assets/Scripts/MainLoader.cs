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
        private ReorderableList OnAwakeLoadingList;
        private ReorderableList OnStartLoadingList;

        private int OnAwakePickerWindowID;
        private int OnStartPickerWindowID;

        private MainLoader MainLoader => (MainLoader)target;

        private void OnEnable()
        {
            OnAwakeLoadingList = new ReorderableList(MainLoader.InfrastructuresOnAwake, typeof(GameObject), true, true, true, true);
            OnStartLoadingList = new ReorderableList(MainLoader.InfrastructuresOnStart, typeof(GameObject), true, true, true, true);
            OnAwakeLoadingList.drawHeaderCallback += rect => { GUI.Label(rect, "Loading on Awake"); };
            OnStartLoadingList.drawHeaderCallback += rect => { GUI.Label(rect, "Loading on Start"); };

            OnAwakeLoadingList.onAddCallback += OnAdd;
            OnStartLoadingList.onAddCallback += OnAdd;


            OnAwakeLoadingList.onRemoveCallback += RemoveItem;
            OnStartLoadingList.onRemoveCallback += RemoveItem;
        }

        private void OnDisable()
        {
            OnAwakeLoadingList.drawHeaderCallback = null;
            OnAwakeLoadingList.onAddCallback = null;
            OnAwakeLoadingList.onRemoveCallback = null;

            OnStartLoadingList.drawHeaderCallback = null;
            OnStartLoadingList.onAddCallback = null;
            OnStartLoadingList.onRemoveCallback = null;
        }

        private void OnAdd(ReorderableList list)
        {
            if (list == OnAwakeLoadingList)
            {
                OnAwakePickerWindowID = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;
                EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, null, OnAwakePickerWindowID);
            }

            if (list == OnStartLoadingList)
            {
                OnStartPickerWindowID = EditorGUIUtility.GetControlID(FocusType.Passive) + 101;
                EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, null, OnStartPickerWindowID);
            }

            EditorUtility.SetDirty(target);
        }

        private void RemoveItem(ReorderableList list)
        {
            if(list == OnAwakeLoadingList)
            {
                MainLoader.InfrastructuresOnAwake.RemoveAt(list.index);
            }

            if(list == OnStartLoadingList)
            {
                MainLoader.InfrastructuresOnStart.RemoveAt(list.index);
            }

            EditorUtility.SetDirty(target);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            OnAwakeLoadingList.DoLayoutList();
            GUILayout.Space(10);
            OnStartLoadingList.DoLayoutList();
            GUILayout.Space(10);

            if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == OnAwakePickerWindowID)
            {
                GameObject selectedObject = (GameObject)EditorGUIUtility.GetObjectPickerObject();
                OnAwakePickerWindowID = -1;
                MainLoader.InfrastructuresOnAwake.Add(selectedObject);
            }

            if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == OnStartPickerWindowID)
            {
                GameObject selectedObject = (GameObject)EditorGUIUtility.GetObjectPickerObject();
                OnStartPickerWindowID = -1;
                MainLoader.InfrastructuresOnStart.Add(selectedObject);
            }
        }
    }
}