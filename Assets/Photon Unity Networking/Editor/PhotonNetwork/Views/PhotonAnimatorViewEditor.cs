// ----------------------------------------------------------------------------
// <copyright file="PhotonAnimatorViewEditor.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
//   This is a custom editor for the AnimatorView component.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 || UNITY_5_4_OR_NEWER
#define UNITY_MIN_5_3
#endif


using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
using UnityEditorInternal;
#elif UNITY_5 || UNITY_5_0 || UNITY_5_3_OR_NEWER 
using UnityEditor.Animations;
#endif

[CustomEditor(typeof (PhotonAnimatorView))]
public class PhotonAnimatorViewEditor : Editor
{
    private Animator m_Animator;
    private PhotonAnimatorView m_Target;

	#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_0 || UNITY_5_3_OR_NEWER
    private AnimatorController m_Controller;
#endif

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        if (this.m_Animator == null)
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
		#if UNITY_5 || UNITY_5_0 || UNITY_5_3_OR_NEWER
		return (this.m_Controller == null) ? 0 : this.m_Controller.layers.Length;
		#else
		return (this.m_Controller == null) ? 0 : this.m_Controller.layerCount;
		#endif
    }


	#if UNITY_5 || UNITY_5_0 || UNITY_5_3_OR_NEWER
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
#endif


    private void OnEnable()
    {
        this.m_Target = (PhotonAnimatorView) target;
        this.m_Animator = this.m_Target.GetComponent<Animator>();

#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
        this.m_Controller = AnimatorController.GetEffectiveAnimatorController(this.m_Animator);
		#elif UNITY_5 || UNITY_5_0 || UNITY_5_3_OR_NEWER
        this.m_Controller = this.GetEffectiveController(this.m_Animator) as AnimatorController;
#endif

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
        Rect containerRect = PhotonGUI.ContainerBody(this.GetLayerCount()*lineHeight);

        for (int i = 0; i < this.GetLayerCount(); ++i)
        {
            if (this.m_Target.DoesLayerSynchronizeTypeExist(i) == false)
            {
                this.m_Target.SetLayerSynchronized(i, PhotonAnimatorView.SynchronizeType.Disabled);

                #if !UNITY_MIN_5_3
                EditorUtility.SetDirty(this.m_Target);
                #endif
            }

            PhotonAnimatorView.SynchronizeType syncType = this.m_Target.GetLayerSynchronizeType(i);

            Rect elementRect = new Rect(containerRect.xMin, containerRect.yMin + i*lineHeight, containerRect.width, lineHeight);

            Rect labelRect = new Rect(elementRect.xMin + 5, elementRect.yMin + 2, EditorGUIUtility.labelWidth - 5, elementRect.height);
            GUI.Label(labelRect, "Layer " + i);

            Rect popupRect = new Rect(elementRect.xMin + EditorGUIUtility.labelWidth, elementRect.yMin + 2, elementRect.width - EditorGUIUtility.labelWidth - 5, EditorGUIUtility.singleLineHeight);
            syncType = (PhotonAnimatorView.SynchronizeType) EditorGUI.EnumPopup(popupRect, syncType);

            if (i < this.GetLayerCount() - 1)
            {
                Rect splitterRect = new Rect(elementRect.xMin + 2, elementRect.yMax, elementRect.width - 4, 1);
                PhotonGUI.DrawSplitter(splitterRect);
            }

            if (syncType != this.m_Target.GetLayerSynchronizeType(i))
            {
                Undo.RecordObject(target, "Modify Synchronize Layer Weights");
                this.m_Target.SetLayerSynchronized(i, syncType);

                #if !UNITY_MIN_5_3
                EditorUtility.SetDirty(this.m_Target);
                #endif
            }
        }
    }

    private int GetParameterCount()
    {
        #if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
        return (this.m_Controller == null) ? 0 : this.m_Controller.parameterCount;
		#elif UNITY_5 || UNITY_5_0 || UNITY_5_3_OR_NEWER
        return (this.m_Controller == null) ? 0 : this.m_Controller.parameters.Length;
        #else
        return (m_Animator == null) ? 0 : m_Animator.parameters.Length;
        #endif
    }

    private AnimatorControllerParameter GetAnimatorControllerParameter(int i)
    {
        #if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
        return this.m_Controller.GetParameter(i);
		#elif UNITY_5 || UNITY_5_0 || UNITY_5_3_OR_NEWER
        return this.m_Controller.parameters[i];
        #else
        return m_Animator.parameters[i];
        #endif
    }

    private bool DoesParameterExist(string name)
    {
        for (int i = 0; i < this.GetParameterCount(); ++i)
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
        var syncedParams = this.m_Target.GetSynchronizedParameters();
        List<string> paramsToRemove = new List<string>();

        for (int i = 0; i < syncedParams.Count; ++i)
        {
            string parameterName = syncedParams[i].Name;
            if (DoesParameterExist(parameterName) == false)
            {
                Debug.LogWarning("Parameter '" + this.m_Target.GetSynchronizedParameters()[i].Name + "' doesn't exist anymore. Removing it from the list of synchronized parameters");
                paramsToRemove.Add(parameterName);
            }
        }
        if (paramsToRemove.Count > 0)
        {
            foreach (string param in paramsToRemove)
            {
                this.m_Target.GetSynchronizedParameters().RemoveAll(item => item.Name == param);
            }

            #if !UNITY_MIN_5_3
            EditorUtility.SetDirty(this.m_Target);
            #endif
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
        Rect containerRect = PhotonGUI.ContainerBody(GetParameterCount()*lineHeight);

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
				}else{
                	defaultValue += parameter.defaultBool.ToString();
				}
            }
            else if (parameter.type == AnimatorControllerParameterType.Float)
            {
				if (Application.isPlaying && m_Animator.gameObject.activeInHierarchy)
				{
					defaultValue += m_Animator.GetFloat(parameter.name).ToString("0.00");
				}else{
               	 defaultValue += parameter.defaultFloat.ToString();
				}
            }
            else if (parameter.type == AnimatorControllerParameterType.Int)
            {
				if (Application.isPlaying && m_Animator.gameObject.activeInHierarchy)
				{
					defaultValue += m_Animator.GetInteger(parameter.name);
				}else{
                	defaultValue += parameter.defaultInt.ToString();
				}
            }
			else if (parameter.type == AnimatorControllerParameterType.Trigger)
			{
				if (Application.isPlaying && m_Animator.gameObject.activeInHierarchy)
				{
					defaultValue += m_Animator.GetBool(parameter.name);
				}else{
					defaultValue += parameter.defaultBool.ToString();
				}
			}

            if (this.m_Target.DoesParameterSynchronizeTypeExist(parameter.name) == false)
            {
                this.m_Target.SetParameterSynchronized(parameter.name, (PhotonAnimatorView.ParameterType) parameter.type, PhotonAnimatorView.SynchronizeType.Disabled);

                #if !UNITY_MIN_5_3
                EditorUtility.SetDirty(this.m_Target);
                #endif
            }

            PhotonAnimatorView.SynchronizeType value = this.m_Target.GetParameterSynchronizeType(parameter.name);

			// check if using trigger and actually synchronizing it
			if (value!=PhotonAnimatorView.SynchronizeType.Disabled &&parameter.type == AnimatorControllerParameterType.Trigger)
			{
				isUsingTriggers = true;
			}

            Rect elementRect = new Rect(containerRect.xMin, containerRect.yMin + i*lineHeight, containerRect.width, lineHeight);

            Rect labelRect = new Rect(elementRect.xMin + 5, elementRect.yMin + 2, EditorGUIUtility.labelWidth - 5, elementRect.height);
            GUI.Label(labelRect, parameter.name + " (" + defaultValue + ")");

            Rect popupRect = new Rect(elementRect.xMin + EditorGUIUtility.labelWidth, elementRect.yMin + 2, elementRect.width - EditorGUIUtility.labelWidth - 5, EditorGUIUtility.singleLineHeight);
            value = (PhotonAnimatorView.SynchronizeType) EditorGUI.EnumPopup(popupRect, value);

            if (i < GetParameterCount() - 1)
            {
                Rect splitterRect = new Rect(elementRect.xMin + 2, elementRect.yMax, elementRect.width - 4, 1);
                PhotonGUI.DrawSplitter(splitterRect);
            }



            if (value != this.m_Target.GetParameterSynchronizeType(parameter.name))
            {
                Undo.RecordObject(target, "Modify Synchronize Parameter " + parameter.name);
                this.m_Target.SetParameterSynchronized(parameter.name, (PhotonAnimatorView.ParameterType) parameter.type, value);

                #if !UNITY_MIN_5_3
                EditorUtility.SetDirty(this.m_Target);
                #endif
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