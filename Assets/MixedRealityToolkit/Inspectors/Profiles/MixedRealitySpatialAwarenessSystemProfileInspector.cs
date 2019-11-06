// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness.Editor
{
    [CustomEditor(typeof(MixedRealitySpatialAwarenessSystemProfile))]
    public class MixedRealitySpatialAwarenessSystemProfileInspector : BaseDataProviderServiceInspector
    {
        private const string ObserverErrorMsg = "The Mixed Reality Spatial Awareness System requires one or more observers.";
        private static readonly GUIContent AddObserverContent = new GUIContent("+ Add Spatial Observer", "Add Spatial Observer");
        private static readonly GUIContent RemoveObserverContent = new GUIContent("-", "Remove Spatial Observer");

        private const string ProfileTitle = "Spatial Awareness System Settings";
        private const string ProfileDescription = "The Spatial Awareness System profile allows developers to configure cross-platform environmental awareness.";

        public override void OnInspectorGUI()
        {
            if (!RenderProfileHeader(ProfileTitle, ProfileDescription, target))
            {
                return;
            }

            using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                using (new EditorGUI.IndentLevelScope())
                {
                    RenderDataProviderList(AddObserverContent, RemoveObserverContent, ObserverErrorMsg, typeof(BaseSpatialAwarenessObserverProfile));
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessSystemProfile;
        }

        #region DataProvider Inspector Utilities

        protected override SerializedProperty GetDataProviderConfigurationList()
        {
            return serializedObject.FindProperty("observerConfigurations");
        }

        protected override ServiceConfigurationProperties GetDataProviderConfigurationProperties(SerializedProperty providerEntry)
        {
            return new ServiceConfigurationProperties()
            {
                Name = providerEntry.FindPropertyRelative("componentName"),
                Type = providerEntry.FindPropertyRelative("componentType"),
                providerProfile = providerEntry.FindPropertyRelative("observerProfile"),
                runtimePlatform = providerEntry.FindPropertyRelative("runtimePlatform"),
            };
        }

        protected override IMixedRealityServiceConfiguration GetDataProviderConfiguration(int index)
        {
            var configurations = (target as MixedRealitySpatialAwarenessSystemProfile)?.ObserverConfigurations;
            if (configurations != null && index >= 0 && index < configurations.Length)
            {
                return configurations[index];
            }

            return null;
        }

        #endregion
    }
}