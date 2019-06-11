// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(PointerClickHandler))]
    public class PointerClickHandlerInspector : BaseInputHandlerInspector
    {
        private SerializedProperty pointerUpProperty;
        private SerializedProperty pointerDownProperty;
        private SerializedProperty pointerClickedProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            pointerUpProperty = serializedObject.FindProperty("onPointerUpActionEvent");
            pointerDownProperty = serializedObject.FindProperty("onPointerDownActionEvent");
            pointerClickedProperty = serializedObject.FindProperty("onPointerClickedActionEvent");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            bool enabled = CheckMixedRealityToolkit();
            if (enabled)
            {
                if (!MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled)
                {
                    EditorGUILayout.HelpBox("No input system is enabled, or you need to specify the type in the main configuration profile.", MessageType.Warning);
                }

                if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile == null 
                    || MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile == null)
                {
                    EditorGUILayout.HelpBox("No Input System or Input Actions Profile Found, be sure to specify a profile in the Input System's configuration profile.", MessageType.Error);
                    enabled = false;
                }
            }

            serializedObject.Update();

            bool wasGUIEnabled = GUI.enabled;
            GUI.enabled = enabled;

            EditorGUILayout.PropertyField(pointerUpProperty, true);
            EditorGUILayout.PropertyField(pointerDownProperty, true);
            EditorGUILayout.PropertyField(pointerClickedProperty, true);

            GUI.enabled = wasGUIEnabled;

            serializedObject.ApplyModifiedProperties();

        }
    }
}