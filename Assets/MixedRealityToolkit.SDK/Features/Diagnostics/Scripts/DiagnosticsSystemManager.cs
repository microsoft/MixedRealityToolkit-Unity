// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    public class DiagnosticsSystemManager : BaseServiceManager
    {
        // todo: tooltips, etc
        [SerializeField]
        [Implements(typeof(IMixedRealityDiagnosticsSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType DiagnosticsSystemType = null;

        // todo: tooltips, etc
        [SerializeField]
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
        ///  Initialize the manager.
        /// </summary>
        private void InitializeManager()
        {
            // The Diagnostics system class takes arguments for:
            // * The registrar
            // * The diagnostics system profile
            object[] args = { this, profile };

            Initialize<IMixedRealityDiagnosticsSystem>(DiagnosticsSystemType.Type, args: args);
        }

        /// <summary>
        ///  Uninitialize the manager.
        /// </summary>
        private void UninitializeManager()
        {
            Uninitialize<IMixedRealityDiagnosticsSystem>();
        }
    }
}