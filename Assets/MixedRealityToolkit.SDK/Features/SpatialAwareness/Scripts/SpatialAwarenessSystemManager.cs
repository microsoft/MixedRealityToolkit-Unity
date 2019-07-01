// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    public class SpatialAwarenessSystemManager : BaseServiceManager
    {
        // todo: tooltips, etc
        [SerializeField]
        [Implements(typeof(IMixedRealitySpatialAwarenessSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType SpatialAwarenessSystemType = null;

        // todo: tooltips, etc
        [SerializeField]
        private MixedRealitySpatialAwarenessSystemProfile profile = null;

        #region MonoBehaviour implementation

        private void Awake()
        {
            InitializeManager();
        }

        protected override void OnDestroy()
        {
            UninitializeManager();
            base.OnDestroy();
        }

        #endregion MonoBehaviour implementation

        /// <summary>
        ///  Initialize the manager.
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
        ///  Uninitialize the manager.
        /// </summary>
        private void UninitializeManager()
        {
            Uninitialize<IMixedRealitySpatialAwarenessSystem>();
        }
    }
}