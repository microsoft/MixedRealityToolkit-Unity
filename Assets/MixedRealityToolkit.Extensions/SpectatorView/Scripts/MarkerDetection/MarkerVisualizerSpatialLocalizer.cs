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
    internal class MarkerVisualizerSpatialLocalizer : SpatialLocalizer
    {
        private ISpatialCoordinateService spatialCoordinateService = null;

        /// <inheritdoc/>
        protected override ISpatialCoordinateService SpatialCoordinateService => spatialCoordinateService;

        internal override Task<Guid> InitializeAsync(LocalizerRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        internal override Task<ISpatialCoordinate> LocalizeAsync(LocalizerRole role, Guid token, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        internal override bool TrySetCoordinateId(LocalizerRole role, Guid token, string coordinateId)
        {
            throw new NotImplementedException();
        }

        internal override void Uninitialize(LocalizerRole role, Guid token)
        {
            throw new NotImplementedException();
        }
    }
}
