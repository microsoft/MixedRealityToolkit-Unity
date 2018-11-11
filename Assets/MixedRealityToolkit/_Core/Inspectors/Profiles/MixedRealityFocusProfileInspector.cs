// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityFocusProfile))]
    public class MixedRealityFocusProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private SerializedProperty pointingExtent;
        private SerializedProperty pointingRaycastLayerMasks;
        private SerializedProperty debugDrawPointingRays;
        private SerializedProperty debugDrawPointingRayColors;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!CheckMixedRealityConfigured(false))
            {
                return;
            }

            pointingExtent = serializedObject.FindProperty("pointingExtent");
            pointingRaycastLayerMasks = serializedObject.FindProperty("pointingRaycastLayerMasks");
            debugDrawPointingRays = serializedObject.FindProperty("debugDrawPointingRays");
            debugDrawPointingRayColors = serializedObject.FindProperty("debugDrawPointingRayColors");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();

            if (!CheckMixedRealityConfigured())
            {
                return;
            }

            if (!MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled)
            {
                EditorGUILayout.HelpBox("No input system is enabled, or you need to specify the type in the main configuration profile.", MessageType.Error);

                if (GUILayout.Button("Back to Configuration Profile"))
                {
                    Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile;
                }

                return;
            }

            if (GUILayout.Button("Back to Input Profile"))
            {
                Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Focus Provider", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The focus provider calculates which GameObjects are focused by pointers.", MessageType.Info);
            serializedObject.Update();

            CheckProfileLock(target);

            EditorGUILayout.PropertyField(pointingExtent);
            EditorGUILayout.PropertyField(pointingRaycastLayerMasks, true);
            EditorGUILayout.PropertyField(debugDrawPointingRays);
            EditorGUILayout.PropertyField(debugDrawPointingRayColors, true);

            EditorGUILayout.HelpBox("The gaze provider uses the default settings above, but further customization of the gaze can be done on the Gaze Provider.", MessageType.Info);

            if (GUILayout.Button("Customize Gaze Provider Settings"))
            {
                Selection.activeObject = CameraCache.Main.gameObject;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}