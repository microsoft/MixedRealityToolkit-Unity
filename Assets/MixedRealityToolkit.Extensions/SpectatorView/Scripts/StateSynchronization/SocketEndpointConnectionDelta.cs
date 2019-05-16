// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Wrapper class for announcing the current state of network connections
    /// </summary>
    public class SocketEndpointConnectionDelta
    {
        public SocketEndpointConnectionDelta(IReadOnlyList<SocketEndpoint> addedConnections, IReadOnlyList<SocketEndpoint> removedConnections, IReadOnlyList<SocketEndpoint> continuedConnections)
        {
            AddedConnections = addedConnections;
            RemovedConnections = removedConnections;
            ContinuedConnections = continuedConnections;
        }

        /// <summary>
        /// Returns true if any connections exist.
        /// </summary>
        public bool HasConnections
        {
            get
            {
                return
                    ContinuedConnections.Count > 0 || 
                    AddedConnections.Count > 0 ||
                    RemovedConnections.Count > 0;
            }
        }

        /// <summary>
        /// Network connections that were newly added.
        /// </summary>
        public IReadOnlyList<SocketEndpoint> AddedConnections { get; }

        /// <summary>
        /// Network connections that were recently lost.
        /// </summary>
        public IReadOnlyList<SocketEndpoint> RemovedConnections { get; }

        /// <summary>
        /// Network connections that already existed.
        /// </summary>
        public IReadOnlyList<SocketEndpoint> ContinuedConnections { get; }
    }
}
