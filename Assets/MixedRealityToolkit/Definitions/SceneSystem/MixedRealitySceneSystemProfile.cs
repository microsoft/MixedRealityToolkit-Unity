// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
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
        public bool UseManagerScene => useManagerScene;

        public UnityEngine.Object ManagerSceneObject => managerSceneObject;

        [SerializeField]
        [Tooltip("Using a manager scene ensures a MixedRealityToolkit instance will always be loaded first in your application. It also ensures that this scene and instance will never be unloaded.")]
        private bool useManagerScene = true;

        [SerializeField]
#if (UNITY_EDITOR)
        [SceneAssetReference]
#endif
        private UnityEngine.Object managerSceneObject;
    }
}