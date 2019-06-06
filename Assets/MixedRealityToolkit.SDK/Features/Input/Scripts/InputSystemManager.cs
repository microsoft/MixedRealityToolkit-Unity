// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class InputSystemManager : BaseServiceManager
    {
        // todo: tooltips, etc
        [SerializeField]
        [Implements(typeof(IMixedRealityInputSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType InputSystemType = null;

        // todo: tooltips, etc
        [SerializeField]
        private MixedRealityInputSystemProfile profile = null;

        private void Awake()
        {
            Initialize();
        }

        protected override void OnDestroy()
        {
            Uninitialize();
            base.OnDestroy();
        }

        /// <summary>
        ///  Initialize the manager.
        /// </summary>
        private void Initialize()
        {
            // The input system class takes arguments for:
            // * The registrar
            // * The input system profile
            object[] args = { this, profile };
            Initialize<IMixedRealityInputSystem>(InputSystemType.Type, args: args);

            // The input system uses the focus provider specified in the profile.
            // The args for the focus provider are:
            // * The registrar
            // * The input system profile
            args = new object[] { this, profile };
            Initialize<IMixedRealityFocusProvider>(profile.FocusProviderType.Type, args: args);
        }

        /// <summary>
        ///  Uninitialize the manager.
        /// </summary>
        private void Uninitialize()
        {
            Uninitialize<IMixedRealityInputSystem>();
        }
    }
}