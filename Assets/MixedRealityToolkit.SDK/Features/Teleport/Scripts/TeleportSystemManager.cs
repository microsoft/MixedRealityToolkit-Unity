// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Teleport
{
    public class TeleportSystemManager : BaseServiceManager
    {
        // todo: tooltips, etc
        [SerializeField]
        [Implements(typeof(IMixedRealityTeleportSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType TeleportSystemType = null;

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
            // The teleport system class takes arguments for:
            // * The registrar
            object[] args = { this };

            Initialize<IMixedRealityTeleportSystem>(TeleportSystemType.Type, args: args);
        }

        /// <summary>
        ///  Uninitialize the manager.
        /// </summary>
        private void Uninitialize()
        {
            Uninitialize<IMixedRealityTeleportSystem>();
        }
    }
}