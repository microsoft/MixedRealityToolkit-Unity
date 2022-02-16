﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(MixedRealityHandTrackingProfile))]
    public class MixedRealityHandTrackingProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private SerializedProperty jointPrefab;
        private SerializedProperty palmPrefab;
        private SerializedProperty fingertipPrefab;
        private SerializedProperty handMeshPrefab;
        private SerializedProperty handMeshVisualizationModes;
        private SerializedProperty handJointVisualizationModes;

        private const string ProfileTitle = "Articulated Hand Tracking Settings";
        private const string ProfileDescription = "Use this for hand tracking settings.";

        protected override void OnEnable()
        {
            base.OnEnable();

            jointPrefab = serializedObject.FindProperty("jointPrefab");
            fingertipPrefab = serializedObject.FindProperty("fingertipPrefab");
            palmPrefab = serializedObject.FindProperty("palmPrefab");
            handMeshPrefab = serializedObject.FindProperty("handMeshPrefab");
            handMeshVisualizationModes = serializedObject.FindProperty("handMeshVisualizationModes");
            handJointVisualizationModes = serializedObject.FindProperty("handJointVisualizationModes");
        }

        public override void OnInspectorGUI()
        {
            if (!RenderProfileHeader(ProfileTitle, ProfileDescription, target, true, BackProfileType.Input))
            {
                return;
            }

            using (new EditorGUI.DisabledGroupScope(IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                EditorGUILayout.LabelField("General settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(jointPrefab);
                EditorGUILayout.PropertyField(palmPrefab);
                EditorGUILayout.PropertyField(fingertipPrefab);
                EditorGUILayout.PropertyField(handMeshPrefab);

                EditorGUILayout.LabelField("Visualization settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(handMeshVisualizationModes);
                EditorGUILayout.PropertyField(handJointVisualizationModes);

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile != null &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.HandTrackingProfile;
        }
    }
}
