// ----------------------------------------------------------------------------
// <copyright file="PhotonAnimatorViewEditor.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   This is a custom editor for the AnimatorView component.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEngine;


    [CustomEditor(typeof(PhotonAnimatorView))]
    public class PhotonAnimatorViewEditor : Editor
    {
        private Animator m_Animator;
        private PhotonAnimatorView m_Target;
        private AnimatorController m_Controller;

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            if (m_Animator == null)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("GameObject doesn't have an Animator component to synchronize");
                GUILayout.EndVertical();
                return;
            }

            DrawWeightInspector();

            if (GetLayerCount() == 0)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("Animator doesn't have any layers setup to synchronize");
                GUILayout.EndVertical();
            }

            DrawParameterInspector();

            if (GetParameterCount() == 0)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("Animator doesn't have any parameters setup to synchronize");
                GUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();

            //GUILayout.Label( "m_SynchronizeLayers " + serializedObject.FindProperty( "m_SynchronizeLayers" ).arraySize );
            //GUILayout.Label( "m_SynchronizeParameters " + serializedObject.FindProperty( "m_SynchronizeParameters" ).arraySize );
        }


        private int GetLayerCount()
        {
            return (m_Controller == null) ? 0 : m_Controller.layers.Length;
        }

        private int GetParameterCount()
        {
            return (m_Controller == null) ? 0 : m_Controller.parameters.Length;
        }

        private AnimatorControllerParameter GetAnimatorControllerParameter(int i)
        {
            return m_Controller.parameters[i];
        }


        private RuntimeAnimatorController GetEffectiveController(Animator animator)
        {
            RuntimeAnimatorController controller = animator.runtimeAnimatorController;

            AnimatorOverrideController overrideController = controller as AnimatorOverrideController;
            while (overrideController != null)
            {
                controller = overrideController.runtimeAnimatorController;
                overrideController = controller as AnimatorOverrideController;
            }

            return controller;
        }

        private void OnEnable()
        {
            m_Target = (PhotonAnimatorView)target;
            m_Animator = m_Target.GetComponent<Animator>();

            m_Controller = GetEffectiveController(m_Animator) as AnimatorController;

            CheckIfStoredParametersExist();
        }

        private void DrawWeightInspector()
        {
            SerializedProperty foldoutProperty = serializedObject.FindProperty("ShowLayerWeightsInspector");
            foldoutProperty.boolValue = PhotonGUI.ContainerHeaderFoldout("Synchronize Layer Weights", foldoutProperty.boolValue);

            if (foldoutProperty.boolValue == false)
            {
                return;
            }

            float lineHeight = 20;
            Rect containerRect = PhotonGUI.ContainerBody(GetLayerCount() * lineHeight);

            for (int i = 0; i < GetLayerCount(); ++i)
            {
                if (m_Target.DoesLayerSynchronizeTypeExist(i) == false)
                {
                    m_Target.SetLayerSynchronized(i, PhotonAnimatorView.SynchronizeType.Disabled);
                }

                PhotonAnimatorView.SynchronizeType syncType = m_Target.GetLayerSynchronizeType(i);

                Rect elementRect = new Rect(containerRect.xMin, containerRect.yMin + i * lineHeight, containerRect.width, lineHeight);

                Rect labelRect = new Rect(elementRect.xMin + 5, elementRect.yMin + 2, EditorGUIUtility.labelWidth - 5, elementRect.height);
                GUI.Label(labelRect, "Layer " + i);

                Rect popupRect = new Rect(elementRect.xMin + EditorGUIUtility.labelWidth, elementRect.yMin + 2, elementRect.width - EditorGUIUtility.labelWidth - 5, EditorGUIUtility.singleLineHeight);
                syncType = (PhotonAnimatorView.SynchronizeType)EditorGUI.EnumPopup(popupRect, syncType);

                if (i < GetLayerCount() - 1)
                {
                    Rect splitterRect = new Rect(elementRect.xMin + 2, elementRect.yMax, elementRect.width - 4, 1);
                    PhotonGUI.DrawSplitter(splitterRect);
                }

                if (syncType != m_Target.GetLayerSynchronizeType(i))
                {
                    Undo.RecordObject(target, "Modify Synchronize Layer Weights");
                    m_Target.SetLayerSynchronized(i, syncType);
                }
            }
        }

        private bool DoesParameterExist(string name)
        {
            for (int i = 0; i < GetParameterCount(); ++i)
            {
                if (GetAnimatorControllerParameter(i).name == name)
                {
                    return true;
                }
            }

            return false;
        }

        private void CheckIfStoredParametersExist()
        {
            List<PhotonAnimatorView.SynchronizedParameter> syncedParams = m_Target.GetSynchronizedParameters();
            List<string> paramsToRemove = new List<string>();

            for (int i = 0; i < syncedParams.Count; ++i)
            {
                string parameterName = syncedParams[i].Name;
                if (DoesParameterExist(parameterName) == false)
                {
                    Debug.LogWarning("Parameter '" + m_Target.GetSynchronizedParameters()[i].Name + "' doesn't exist anymore. Removing it from the list of synchronized parameters");
                    paramsToRemove.Add(parameterName);
                }
            }

            if (paramsToRemove.Count > 0)
            {
                foreach (string param in paramsToRemove)
                {
                    m_Target.GetSynchronizedParameters().RemoveAll(item => item.Name == param);
                }
            }
        }


        private void DrawParameterInspector()
        {
            // flag to expose a note in Interface if one or more trigger(s) are synchronized
            bool isUsingTriggers = false;

            SerializedProperty foldoutProperty = serializedObject.FindProperty("ShowParameterInspector");
            foldoutProperty.boolValue = PhotonGUI.ContainerHeaderFoldout("Synchronize Parameters", foldoutProperty.boolValue);

            if (foldoutProperty.boolValue == false)
            {
                return;
            }

            float lineHeight = 20;
            Rect containerRect = PhotonGUI.ContainerBody(GetParameterCount() * lineHeight);

            for (int i = 0; i < GetParameterCount(); i++)
            {
                AnimatorControllerParameter parameter = null;
                parameter = GetAnimatorControllerParameter(i);

                string defaultValue = "";

                if (parameter.type == AnimatorControllerParameterType.Bool)
                {
                    if (Application.isPlaying && m_Animator.gameObject.activeInHierarchy)
                    {
                        defaultValue += m_Animator.GetBool(parameter.name);
                    }
                    else
                    {
                        defaultValue += parameter.defaultBool.ToString();
                    }
                }
                else if (parameter.type == AnimatorControllerParameterType.Float)
                {
                    if (Application.isPlaying && m_Animator.gameObject.activeInHierarchy)
                    {
                        defaultValue += m_Animator.GetFloat(parameter.name).ToString("0.00");
                    }
                    else
                    {
                        defaultValue += parameter.defaultFloat.ToString();
                    }
                }
                else if (parameter.type == AnimatorControllerParameterType.Int)
                {
                    if (Application.isPlaying && m_Animator.gameObject.activeInHierarchy)
                    {
                        defaultValue += m_Animator.GetInteger(parameter.name);
                    }
                    else
                    {
                        defaultValue += parameter.defaultInt.ToString();
                    }
                }
                else if (parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    if (Application.isPlaying && m_Animator.gameObject.activeInHierarchy)
                    {
                        defaultValue += m_Animator.GetBool(parameter.name);
                    }
                    else
                    {
                        defaultValue += parameter.defaultBool.ToString();
                    }
                }

                if (m_Target.DoesParameterSynchronizeTypeExist(parameter.name) == false)
                {
                    m_Target.SetParameterSynchronized(parameter.name, (PhotonAnimatorView.ParameterType)parameter.type, PhotonAnimatorView.SynchronizeType.Disabled);
                }

                PhotonAnimatorView.SynchronizeType value = m_Target.GetParameterSynchronizeType(parameter.name);

                // check if using trigger and actually synchronizing it
                if (value != PhotonAnimatorView.SynchronizeType.Disabled && parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    isUsingTriggers = true;
                }

                Rect elementRect = new Rect(containerRect.xMin, containerRect.yMin + i * lineHeight, containerRect.width, lineHeight);

                Rect labelRect = new Rect(elementRect.xMin + 5, elementRect.yMin + 2, EditorGUIUtility.labelWidth - 5, elementRect.height);
                GUI.Label(labelRect, parameter.name + " (" + defaultValue + ")");

                Rect popupRect = new Rect(elementRect.xMin + EditorGUIUtility.labelWidth, elementRect.yMin + 2, elementRect.width - EditorGUIUtility.labelWidth - 5, EditorGUIUtility.singleLineHeight);
                value = (PhotonAnimatorView.SynchronizeType)EditorGUI.EnumPopup(popupRect, value);

                if (i < GetParameterCount() - 1)
                {
                    Rect splitterRect = new Rect(elementRect.xMin + 2, elementRect.yMax, elementRect.width - 4, 1);
                    PhotonGUI.DrawSplitter(splitterRect);
                }

                if (value != m_Target.GetParameterSynchronizeType(parameter.name))
                {
                    Undo.RecordObject(target, "Modify Synchronize Parameter " + parameter.name);
                    m_Target.SetParameterSynchronized(parameter.name, (PhotonAnimatorView.ParameterType)parameter.type, value);
                }
            }

            // display note when synchronized triggers are detected.
            if (isUsingTriggers)
            {
                GUILayout.BeginHorizontal(GUI.skin.box);
                GUILayout.Label("When using triggers, make sure this component is last in the stack");
                GUILayout.EndHorizontal();
            }
        }
    }
}