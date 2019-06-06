// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    public class CameraSystemManager : BaseServiceManager
    {
        // todo: tooltips, etc
        [SerializeField]
        [Implements(typeof(IMixedRealityCameraSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType CameraSystemType = null;

        // todo: tooltips, etc
        [SerializeField]
        private MixedRealityCameraProfile profile = null;

        /// <summary>
        /// Instance of the camera system being managed.
        /// </summary>
        private IMixedRealityCameraSystem CameraSystem => service as IMixedRealityCameraSystem;

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
            // The camera system class takes arguments for:
            // * The registrar
            // * The camera system profile
            object[] args = { this, profile };

            Initialize<IMixedRealityCameraSystem>(CameraSystemType.Type, args: args);
        }

        /// <summary>
        ///  Uninitialize the manager.
        /// </summary>
        private void Uninitialize()
        {
            Uninitialize<IMixedRealityCameraSystem>();
        }
    }
}