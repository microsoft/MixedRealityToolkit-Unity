// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    [Serializable]
    public enum AppRole : byte
    {
        /// <summary>
        /// The sharing app's role has not yet been defined.
        /// </summary>
        None = 0,  
        /// <summary>
        /// The app is considered a client.
        /// </summary>
        Client = 1,
        /// <summary>
        /// The app is considered a host.
        /// </summary>
        Host = 2,
        /// <summary>
        /// The app is considered a server. This should be used for dedicated-server analogue setups.
        /// </summary>
        Server = 3,
    }
}