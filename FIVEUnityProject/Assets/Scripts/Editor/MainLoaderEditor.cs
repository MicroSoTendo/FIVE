using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FIVE
{
    [CustomEditor(typeof(MainLoader))]
    public class MainLoaderEditor : Editor
    {
        private ReorderableList onAwakeLoadingList;
        private ReorderableList onStartLoadingList;

        private int pickerWindowId;

        private MainLoader MainLoader => (MainLoader)target;
        private List<GameObject> pickerList;

        private void InitList(string propertyName, out ReorderableList list, string headerText, List<GameObject> prefabList)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            list = new ReorderableList(serializedObject, property, true, true, true, true);
            list.drawHeaderCallback += rect => { GUI.Label(rect, headerText); };
            list.onAddCallback += OnAdd(prefabList);
            list.drawElementCallback += DrawElementCallback(property);
            list.onRemoveCallback += RemoveItem(prefabList);
            list.elementHeightCallback += ElementHeightCallback(property);
        }

        private ReorderableList.ElementHeightCallbackDelegate ElementHeightCallback(SerializedProperty property)
        {
            return index => 24f;
        }

        private void OnEnable()
        {
            pickerWindowId = GUIUtility.GetControlID(FocusType.Passive) + 100;
            InitList("InfrastructuresOnAwake", out onAwakeLoadingList, "Loading on Awake", MainLoader.InfrastructuresOnAwake);
            InitList("InfrastructuresOnStart", out onStartLoadingList, "Loading on Start", MainLoader.InfrastructuresOnStart);
        }

        private static ReorderableList.ElementCallbackDelegate DrawElementCallback(SerializedProperty property)
        {
            return (rect, index, isActive, isfocused) =>
            {
                SerializedProperty prefab = property.GetArrayElementAtIndex(index);
                GameObject go = (GameObject)prefab.objectReferenceValue;
                GUIContent content = new GUIContent(go.name);
                Rect r = new Rect(rect) {height = 24f};
                GameObject newGameObject =
                    (GameObject)EditorGUI.ObjectField(r, content, go, typeof(GameObject), false);
            

                if (newGameObject != go)
                {
                    prefab.objectReferenceValue = newGameObject;
                }
            };
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

        private ReorderableList.AddCallbackDelegate OnAdd(List<GameObject> toBeAdded)
        {
            return list =>
            {
                pickerList = toBeAdded;
                pickerWindowId = GUIUtility.GetControlID(FocusType.Passive) + 100;
                EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, null, pickerWindowId);
                EditorUtility.SetDirty(target);
            };
        }

        private ReorderableList.RemoveCallbackDelegate RemoveItem(IList prefabList)
        {
            return list =>
            {
                prefabList.RemoveAt(list.index);
                EditorUtility.SetDirty(target);
            };
        }

        public override void OnInspectorGUI()
        {
            onAwakeLoadingList.DoLayoutList();
            GUILayout.Space(10);
            onStartLoadingList.DoLayoutList();
            GUILayout.Space(10);

            if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == pickerWindowId)
            {
                var selectedObject = (GameObject)EditorGUIUtility.GetObjectPickerObject();
                pickerWindowId = -1;
                pickerList.Add(selectedObject);
            }
        }
    }
}
