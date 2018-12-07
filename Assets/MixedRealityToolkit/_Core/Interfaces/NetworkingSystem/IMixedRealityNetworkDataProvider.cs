// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces.Events;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.NetworkingSystem
{
    /// <summary>
    /// The interface to define networking data providers, such as WebRTC, Photon, uNet, etc.
    /// </summary>
    public interface IMixedRealityNetworkDataProvider : IMixedRealityEventSource, IMixedRealityDataProvider
    {
    }
}