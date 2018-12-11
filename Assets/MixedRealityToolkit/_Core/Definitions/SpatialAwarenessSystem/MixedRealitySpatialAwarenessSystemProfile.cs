// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.SpatialObservers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem
{
    /// <summary>
    /// Configuration profile settings for setting up the spatial awareness system.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Spatial Awareness System Profile", fileName = "MixedRealitySpatialAwarenessSystemProfile", order = (int)CreateProfileMenuItemIndices.SpatialAwareness)]
    public class MixedRealitySpatialAwarenessSystemProfile : BaseMixedRealityProfile
    {
        /// <summary>
        /// The name of the Spatial Awareness Mesh Physics Layer.
        /// </summary>
        public const string SpatialAwarenessMeshesLayerName = "Spatial Awareness Meshes";

        /// <summary>
        /// The name of the Spatial Awareness Surfaces Physics Layer.
        /// </summary>
        public const string SpatialAwarenessSurfacesLayerName = "Spatial Awareness Surfaces";

        [SerializeField]
        [Tooltip("The list of registered spatial observer data providers.")]
        private SpatialObserverDataProviderConfiguration[] registeredSpatialObserverDataProviders = new SpatialObserverDataProviderConfiguration[0];

        /// <summary>
        /// The list of registered <see cref="IMixedRealitySpatialObserverDataProvider"/>s.
        /// </summary>
        public SpatialObserverDataProviderConfiguration[] RegisteredSpatialObserverDataProviders => registeredSpatialObserverDataProviders;
    }
}
