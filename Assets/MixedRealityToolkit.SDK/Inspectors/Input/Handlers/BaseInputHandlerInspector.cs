// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    public class BaseInputHandlerInspector : UnityEditor.Editor
    {
        private SerializedProperty isFocusRequiredProperty;

        protected virtual void OnEnable()
        {
            //MixedRealityInspectorUtility.CheckMixedRealityConfigured(false);
            isFocusRequiredProperty = serializedObject.FindProperty("isFocusRequired");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(isFocusRequiredProperty);
            serializedObject.ApplyModifiedProperties();
        }

        protected bool CheckMixedRealityToolkit()
        {
            if (!MixedRealityToolkit.IsInitialized)
            {
                EditorGUILayout.HelpBox("There is no MRTK instance in the scene. Some properties may not be editable.", MessageType.Error);
                if (GUILayout.Button(new GUIContent("Add Mixed Reality Toolkit instance to scene"), EditorStyles.miniButton))
                {
                    MixedRealityInspectorUtility.AddMixedRealityToolkitToScene(MixedRealityInspectorUtility.GetDefaultConfigProfile());
                    // After the toolkit has been created, set the selection back to this item so the user doesn't get lost
                    Selection.activeObject = target;
                }
                return false;
            }
            else if (!MixedRealityToolkit.Instance.HasActiveProfile)
            {
                EditorGUILayout.HelpBox("There is no active profile assigned in the current MRTK instance. Some properties may not be editable.", MessageType.Error);
                return false;
            }

            return true;
        }
    }
}