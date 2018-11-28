// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.SpatialObservers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Spatial Awareness Data Providers Profile", fileName = "MixedRealitySpatialAwarenessDataProvidersProfile", order = (int)CreateProfileMenuItemIndices.SpatialAwarenessDataProviders)]
    public class MixedRealitySpatialObserverDataProvidersProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("The list of registered IMixedRealitySpatialAwarenessObservers for this system.")]
        private SpatialObserverDataProviderConfiguration[] registeredSpatialObserverDataProviders = null;

        /// <summary>
        /// The list of registered <see cref="IMixedRealitySpatialAwarenessObserver"/>s for this system.
        /// </summary>
        public SpatialObserverDataProviderConfiguration[] RegisteredSpatialObserverDataProviders => registeredSpatialObserverDataProviders;
    }
}