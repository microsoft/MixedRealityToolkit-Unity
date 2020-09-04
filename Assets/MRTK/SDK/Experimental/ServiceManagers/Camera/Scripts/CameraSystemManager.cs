// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.CameraSystem
{
    /// <summary>
    /// Service manager supporting running the camera system, without requiring the MixedRealityToolkit object.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/CameraSystemManager")]
    public class CameraSystemManager : BaseServiceManager
    {
        [SerializeField]
        [Tooltip("The camera system type that will be instantiated.")]
        [Implements(typeof(IMixedRealityCameraSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType CameraSystemType = null;

        [SerializeField]
        [Tooltip("The camera system configuration profile.")]
        private MixedRealityCameraProfile profile = null;

        private void Awake()
        {
            InitializeManager();
        }

        protected override void OnDestroy()
        {
            UninitializeManager();
            base.OnDestroy();
        }

        /// <summary>
        /// Initialize the manager.
        /// </summary>
        private void InitializeManager()
        {
            // The camera system class takes arguments for:
            // * The camera system profile
            object[] args = { profile };

            Initialize<IMixedRealityCameraSystem>(CameraSystemType.Type, args: args);
        }

        /// <summary>
        /// Uninitialize the manager.
        /// </summary>
        private void UninitializeManager()
        {
            Uninitialize<IMixedRealityCameraSystem>();
        }
    }
}