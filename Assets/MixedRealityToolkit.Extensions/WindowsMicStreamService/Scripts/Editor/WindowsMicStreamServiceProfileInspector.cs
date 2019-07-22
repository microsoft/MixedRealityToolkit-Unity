// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Experimental.Audio.Editor
{
    [CustomEditor(typeof(WindowsMicrophoneStreamServiceProfile))]
    public class WindowsMicStreamServiceProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private const string ProfileTitle = "Windows Microphone Stream Settings";
        private const string ProfileDescription = "The microphone stream settings can help improve voice driven experiences.";

        // General settings
        private SerializedProperty startupBehavior;
        private SerializedProperty localPlayback;
        private SerializedProperty keepData;
        // Stream settings
        private SerializedProperty streamType;
        private SerializedProperty inputGain;
        // todo: recording setting

        protected override void OnEnable()
        {
            base.OnEnable();

            // General settings
            startupBehavior = serializedObject.FindProperty("startupBehavior");
            localPlayback = serializedObject.FindProperty("localPlayback");
            keepData = serializedObject.FindProperty("keepData");

            // Stream settings
            streamType = serializedObject.FindProperty("streamType");
            inputGain = serializedObject.FindProperty("inputGain");

            // todo: recording settings
        }

        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, ProfileDescription, target);

            using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(startupBehavior);
                    EditorGUILayout.PropertyField(localPlayback);
                    EditorGUILayout.PropertyField(keepData);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Stream Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(streamType);
                    EditorGUILayout.PropertyField(inputGain);
                }

                // todo: recording settings

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            //var profile = target as BaseMixedRealityProfile;
            //return MixedRealityToolkit.IsInitialized && profile != null &&
            //       profile == MixedRealityToolkit.Instance.ActiveProfile.BoundaryVisualizationProfile;

            // todo - think on how to do the above correctly
            return false;
        }
    }
}