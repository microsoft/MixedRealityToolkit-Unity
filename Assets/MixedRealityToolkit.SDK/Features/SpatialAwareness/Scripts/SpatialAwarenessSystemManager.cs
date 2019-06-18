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
            Initialize();
        }

        protected override void Update()
        {
            base.Update();
            for (int i = 0; i < dataProviders.Count; i++)
            {
                dataProviders[i]?.Update();
            }
        }

        protected override void OnDestroy()
        {
            Uninitialize();
            base.OnDestroy();
        }

        #endregion MonoBehaviour implementation

        /// <summary>
        ///  Initialize the manager.
        /// </summary>
        private void Initialize()
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
        private void Uninitialize()
        {
            Uninitialize<IMixedRealitySpatialAwarenessSystem>();
        }
    }
}