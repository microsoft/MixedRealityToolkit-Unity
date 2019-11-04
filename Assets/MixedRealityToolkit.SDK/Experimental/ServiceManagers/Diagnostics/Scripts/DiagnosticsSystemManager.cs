// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Diagnostics;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Diagnostics
{
    /// <summary>
    /// Service manager supporting running the diagnostics system without requiring the MixedRealityToolkit object.
    /// </summary>
    public class DiagnosticsSystemManager : BaseServiceManager
    {
        [SerializeField]
        [Tooltip("The diagnostics system type that will be instantiated.")]
        [Implements(typeof(IMixedRealityDiagnosticsSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType DiagnosticsSystemType = null;

        [SerializeField]
        [Tooltip("The diagnostics system configuration profile.")]
        private MixedRealityDiagnosticsProfile profile = null;

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
            // The Diagnostics system class takes arguments for:
            // * The diagnostics system profile
            object[] args = { profile };

            Initialize<IMixedRealityDiagnosticsSystem>(DiagnosticsSystemType.Type, args: args);
        }

        /// <summary>
        /// Uninitialize the manager.
        /// </summary>
        private void UninitializeManager()
        {
            Uninitialize<IMixedRealityDiagnosticsSystem>();
        }
    }
}