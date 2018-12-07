// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.NetworkingSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.Networking.Profiles
{
    /// <summary>
    /// The base profile for <see cref="IMixedRealityNetworkDataProvider"/>s
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/DataProviders/Networking/WebRTC Profile", fileName = "MixedRealityWebRtcNetworkProviderProfile", order = (int)CreateProfileMenuItemIndices.Networking)]
    public abstract class BaseMixedRealityNetworkProviderProfile : BaseMixedRealityProfile
    {
        // TODO Fill out with the configuration options that ALL network data providers must satisfy.
    }
}