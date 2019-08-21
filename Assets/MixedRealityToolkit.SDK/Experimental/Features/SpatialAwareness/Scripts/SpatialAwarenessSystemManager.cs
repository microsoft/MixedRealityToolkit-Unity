// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness
{
    /// <summary>
    /// Service manager supporting running the spatial awareness system, without requiring the MixedRealityToolkit object.
    /// </summary>
    public class SpatialAwarenessSystemManager : BaseServiceManager
    {
        [SerializeField]
        [Tooltip("The spatial awareness system type that will be instantiated.")]
        [Implements(typeof(IMixedRealitySpatialAwarenessSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType SpatialAwarenessSystemType = null;

        [SerializeField]
        [Tooltip("The spatial awareness system configuration profile.")]
        private MixedRealitySpatialAwarenessSystemProfile profile = null;

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
            // The spatial awareness system class takes arguments for:
            // * The registrar
            // * The spatial awareness system profile
            object[] args = { this, profile };

            Initialize<IMixedRealitySpatialAwarenessSystem>(SpatialAwarenessSystemType.Type, args: args);
        }

        /// <summary>
        /// Uninitialize the manager.
        /// </summary>
        private void UninitializeManager()
        {
            Uninitialize<IMixedRealitySpatialAwarenessSystem>();
        }
    }
}