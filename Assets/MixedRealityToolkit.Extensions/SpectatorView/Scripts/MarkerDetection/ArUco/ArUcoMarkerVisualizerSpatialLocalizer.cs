// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// SpatialLocalizer that shows an ArUco marker
    /// </summary>
    internal class ArUcoMarkerVisualizerSpatialLocalizer : SpatialLocalizer
    {
        private ISpatialCoordinateService spatialCoordinateService = null;

        /// <inheritdoc/>
        protected override ISpatialCoordinateService SpatialCoordinateService => spatialCoordinateService;

        internal override Task<Guid> InitializeAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        internal override Task<ISpatialCoordinate> LocalizeAsync(Role role, Guid token, Action<Action<BinaryWriter>> sendMessage, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        internal override void ProcessIncomingMessage(Role role, Guid token, BinaryReader r)
        {
            throw new NotImplementedException();
        }

        internal override void Uninitialize(Role role, Guid token)
        {
            throw new NotImplementedException();
        }
    }
}
