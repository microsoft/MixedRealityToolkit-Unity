// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    /// <summary>
    /// Configuration profile settings for setting up scene system.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Scene System Profile", fileName = "MixedRealitySceneSystemProfile", order = (int)CreateProfileMenuItemIndices.SceneSystem)]
    [MixedRealityServiceProfile(typeof(IMixedRealitySceneSystem))]
    public class MixedRealitySceneSystemProfile : BaseMixedRealityProfile
    {
        public bool UseManagerScene { get { return useManagerScene && managerSceneObject != null; } }

        public bool UseLightingScene { get { return useLightingScene && lightingSceneObject != null; } }

        public UnityEngine.Object ManagerSceneObject => managerSceneObject;

        public UnityEngine.Object LightingSceneObject => lightingSceneObject;

        [SerializeField]
        [Tooltip("Using a manager scene ensures a MixedRealityToolkit instance will always be loaded first in your application. It also ensures that this scene and instance will never be unloaded.")]
        private bool useManagerScene = true;

        [SerializeField]
#if (UNITY_EDITOR)
        [SceneAssetReference]
#endif
        private UnityEngine.Object managerSceneObject = null;

        [SerializeField]
        [Tooltip("Using a content scene ensures that the same lighting settings will be enforced across all loaded scenes.")]
        private bool useLightingScene = true;

        [SerializeField]
#if (UNITY_EDITOR)
        [SceneAssetReference]
#endif
        private UnityEngine.Object lightingSceneObject = null;
        
        [SerializeField]
        [Tooltip("The scene names in your build settings, in load order. These will be managed by the service.")]
        private string[] buildSceneNames;
    }
}