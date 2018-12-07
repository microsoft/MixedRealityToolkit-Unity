// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.NetworkingSystem
{
    /// <summary>
    /// Configuration profile settings for setting up networking.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Network System Profile", fileName = "MixedRealityNetworkSystemProfile", order = (int)CreateProfileMenuItemIndices.Networking)]
    public class MixedRealityNetworkSystemProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private NetworkDataProviderConfiguration[] registeredNetworkDataProviders;

        /// <summary>
        /// The list of registered network data providers.
        /// </summary>
        public NetworkDataProviderConfiguration[] RegisteredNetworkDataProviders => registeredNetworkDataProviders;
    }
}