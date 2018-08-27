// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityTouchInputProfile))]
    public class MixedRealityTouchInputProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private SerializedProperty pointerAction;
        private SerializedProperty holdAction;

        private void OnEnable()
        {
            CheckMixedRealityManager(false);

            pointerAction = serializedObject.FindProperty("pointerAction");
            holdAction = serializedObject.FindProperty("holdAction");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            EditorGUILayout.LabelField("Touch Screen Input", EditorStyles.boldLabel);

            if (!CheckMixedRealityManager())
            {
                return;
            }

            EditorGUILayout.HelpBox("Input Actions for Touch Screen Input Sources.", MessageType.Info);

            if (!MixedRealityManager.Instance.ActiveProfile.IsInputSystemEnabled)
            {
                EditorGUILayout.HelpBox("No input system is enabled, or you need to specify the type in the main configuration profile.", MessageType.Error);
                return;
            }

            if (MixedRealityManager.Instance.ActiveProfile.InputActionsProfile == null)
            {
                EditorGUILayout.HelpBox("No input actions found, please specify a input action profile in the main configuration.", MessageType.Error);
                return;
            }

            serializedObject.Update();
            EditorGUILayout.PropertyField(pointerAction);
            EditorGUILayout.PropertyField(holdAction);
            serializedObject.ApplyModifiedProperties();
        }
    }
}