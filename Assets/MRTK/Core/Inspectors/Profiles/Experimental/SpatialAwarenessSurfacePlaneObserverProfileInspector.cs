// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Linq;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Editor.SpatialAwareness
{
    [CustomEditor(typeof(Experimental.SpatialAwareness.SpatialAwarenessSurfacePlaneObserverProfile))]
    public class MixedRealitySpatialAwarenessSurfacePlaneObserverProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
       
        private SerializedProperty physicsLayer;
        private SerializedProperty surfaceTypes;
        private SerializedProperty defaultMaterial;
        private SerializedProperty firstUpdateDelay;
        private SerializedProperty planeThickness;

        private const string ProfileTitle = "Surface Plane Observer Settings";
        private const string ProfileDescription = "Settings for plane finder used on Hololens 1";

        protected override void OnEnable()
        {
            base.OnEnable();

            physicsLayer = serializedObject.FindProperty("physicsLayer");
            surfaceTypes = serializedObject.FindProperty("surfaceTypes");
            defaultMaterial = serializedObject.FindProperty("defaultMaterial");
            firstUpdateDelay = serializedObject.FindProperty("firstUpdateDelay");
            planeThickness = serializedObject.FindProperty("planeThickness");
        }

        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, ProfileDescription, target, true, BackProfileType.SpatialAwareness);

            //using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target)))
            using (new GUIEnabledWrapper())
            {
                serializedObject.Update();

                EditorGUILayout.PropertyField(physicsLayer);
                EditorGUILayout.PropertyField(surfaceTypes);
                EditorGUILayout.PropertyField(defaultMaterial);
                EditorGUILayout.PropertyField(firstUpdateDelay);
                EditorGUILayout.PropertyField(planeThickness);

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;

            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessSystemProfile != null &&
                   MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessSystemProfile.ObserverConfigurations != null &&
                   MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessSystemProfile.ObserverConfigurations.Any(s => s.ObserverProfile == profile);
        }
    }
}
