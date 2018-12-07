// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.NetworkingSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.NetworkingSystem;

namespace Microsoft.MixedReality.Toolkit.Core.Services.Networking
{
    /// <summary>
    /// The Mixed Reality Toolkit's default implementation of the <see cref="IMixedRealityNetworkingSystem"/>
    /// </summary>
    public class MixedRealityNetworkingSystem : BaseEventSystem, IMixedRealityNetworkingSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile"></param>
        public MixedRealityNetworkingSystem(MixedRealityNetworkingProfile profile)
            : base(profile)
        {
        }
    }
}