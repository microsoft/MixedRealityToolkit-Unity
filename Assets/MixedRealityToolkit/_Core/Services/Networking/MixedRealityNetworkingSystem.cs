// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.NetworkingSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.NetworkingSystem;
using System.Collections.Generic;

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
        public MixedRealityNetworkingSystem(MixedRealityNetworkSystemProfile profile)
            : base(profile)
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
        }

        #region IMixedRealityNetworkingSystem Implementation

        /// <inheritdoc />
        public HashSet<IMixedRealityNetworkDataProvider> NetworkDataProviders { get; } = new HashSet<IMixedRealityNetworkDataProvider>();

        /// <inheritdoc />
        public uint RequestNetworkDataProviderSourceId()
        {
            return 0;
        }

        /// <inheritdoc />
        public void RaiseNetworkDataProviderDetected(IMixedRealityNetworkDataProvider networkDataProvider)
        {
        }

        /// <inheritdoc />
        public void RaiseNetworkDataProviderLost(IMixedRealityNetworkDataProvider networkDataProvider)
        {
        }

        /// <inheritdoc />
        public void SendData<T>(T data)
        {
        }

        /// <inheritdoc />
        public void OnDataReceived<T>(T data)
        {
        }

        #endregion IMixedRealityNetworkingSystem Implementation
    }
}