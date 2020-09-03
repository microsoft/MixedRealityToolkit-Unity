// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License. 

using Microsoft.MixedReality.Toolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness.Editor
{
    /// <summary>
    /// Class handles rendering inspector view of MixedRealitySpatialAwarenessSystemProfile object
    /// </summary>
    [CustomEditor(typeof(MixedRealitySpatialAwarenessSystemProfile))]
    public class MixedRealitySpatialAwarenessSystemProfileInspector : BaseDataProviderServiceInspector
    {
        private const string ObserverErrorMsg = "The Mixed Reality Spatial Awareness System requires one or more observers.";
        private static readonly GUIContent AddObserverContent = new GUIContent("+ Add Spatial Observer", "Add Spatial Observer");
        private static readonly GUIContent RemoveObserverContent = new GUIContent("-", "Remove Spatial Observer");

        private const string ProfileTitle = "Spatial Awareness System Settings";
        private const string ProfileDescription = "The Spatial Awareness System profile allows developers to configure cross-platform environmental awareness.";

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            if (!RenderProfileHeader(ProfileTitle, ProfileDescription, target))
            {
                return;
            }

            using (new EditorGUI.DisabledGroupScope(IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                using (new EditorGUI.IndentLevelScope())
                {
                    RenderDataProviderList(AddObserverContent, RemoveObserverContent, ObserverErrorMsg, typeof(BaseSpatialAwarenessObserverProfile));
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        /// <inheritdoc/>
        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessSystemProfile;
        }

        #region DataProvider Inspector Utilities

        /// <inheritdoc/>
        protected override SerializedProperty GetDataProviderConfigurationList()
        {
            return serializedObject.FindProperty("observerConfigurations");
        }

        /// <inheritdoc/>
        protected override ServiceConfigurationProperties GetDataProviderConfigurationProperties(SerializedProperty providerEntry)
        {
            return new ServiceConfigurationProperties()
            {
                componentName = providerEntry.FindPropertyRelative("componentName"),
                componentType = providerEntry.FindPropertyRelative("componentType"),
                providerProfile = providerEntry.FindPropertyRelative("observerProfile"),
                runtimePlatform = providerEntry.FindPropertyRelative("runtimePlatform"),
            };
        }

        /// <inheritdoc/>
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