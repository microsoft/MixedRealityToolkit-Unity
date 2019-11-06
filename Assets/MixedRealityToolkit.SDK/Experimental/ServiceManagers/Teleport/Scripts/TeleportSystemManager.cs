// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Teleport;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Teleport
{
    /// <summary>
    /// Service manager supporting running the teleport system, without requiring the MixedRealityToolkit object.
    /// </summary>
    public class TeleportSystemManager : BaseServiceManager
    {
        [SerializeField]
        [Tooltip("The teleport system type that will be instantiated.")]
        [Implements(typeof(IMixedRealityTeleportSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType TeleportSystemType = null;

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
            // The teleport system class takes no arguments.
            Initialize<IMixedRealityTeleportSystem>(TeleportSystemType.Type);
        }

        /// <summary>
        /// Uninitialize the manager.
        /// </summary>
        private void UninitializeManager()
        {
            Uninitialize<IMixedRealityTeleportSystem>();
        }
    }
}