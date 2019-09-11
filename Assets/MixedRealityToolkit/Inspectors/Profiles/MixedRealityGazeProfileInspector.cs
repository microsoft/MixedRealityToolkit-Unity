// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(MixedRealityGazeProfile))]
    public class MixedRealityGazeProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private SerializedProperty gazeCursorPrefab;
        private SerializedProperty setCursorInvisibleWhenFocusLocked;
        private SerializedProperty maxGazeCollisionDistance;
        private SerializedProperty raycastLayerMasks;
        private SerializedProperty stabilizer;
        private SerializedProperty gazeTransform;
        private SerializedProperty minHeadVelocityThreshold;
        private SerializedProperty maxHeadVelocityThreshold;
        private SerializedProperty useEyeTracking;

        private const string ProfileTitle = "Gaze Settings";
        private const string ProfileDescription = "Use this for gaze settings.";

        protected override void OnEnable()
        {
            base.OnEnable();

            gazeCursorPrefab = serializedObject.FindProperty("gazeCursorPrefab");
            setCursorInvisibleWhenFocusLocked = serializedObject.FindProperty("setCursorInvisibleWhenFocusLocked");
            maxGazeCollisionDistance = serializedObject.FindProperty("maxGazeCollisionDistance");
            raycastLayerMasks = serializedObject.FindProperty("raycastLayerMasks");
            stabilizer = serializedObject.FindProperty("stabilizer");
            gazeTransform = serializedObject.FindProperty("gazeTransform");
            minHeadVelocityThreshold = serializedObject.FindProperty("minHeadVelocityThreshold");
            maxHeadVelocityThreshold = serializedObject.FindProperty("maxHeadVelocityThreshold");
            useEyeTracking = serializedObject.FindProperty("useEyeTracking");
        }

        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, ProfileDescription, target, true, BackProfileType.Input);

            using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                EditorGUILayout.PropertyField(gazeCursorPrefab);
                EditorGUILayout.PropertyField(setCursorInvisibleWhenFocusLocked);
                EditorGUILayout.PropertyField(maxGazeCollisionDistance);
                EditorGUILayout.PropertyField(raycastLayerMasks);
                EditorGUILayout.PropertyField(stabilizer);
                EditorGUILayout.PropertyField(gazeTransform);
                EditorGUILayout.PropertyField(minHeadVelocityThreshold);
                EditorGUILayout.PropertyField(maxHeadVelocityThreshold);
                EditorGUILayout.PropertyField(useEyeTracking);

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile != null &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.GazeProfile;
        }
    }
}
