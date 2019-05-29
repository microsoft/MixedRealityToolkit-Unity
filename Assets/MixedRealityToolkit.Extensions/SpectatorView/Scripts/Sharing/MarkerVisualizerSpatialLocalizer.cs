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
    /// SpatialLocalizer that shows a marker
    /// </summary>
    internal class MarkerVisualizerSpatialLocalizer : SpatialLocalizerBase
    {
        /// <inheritdoc/>
        protected override ISpatialCoordinateService SpatialCoordinateService => spatialCoordinateService;

        /// <inheritdoc/>
        protected override Task<ISpatialCoordinate> GetHostCoordinateAsync(Guid token)
        {
            throw new NotImplementedException();
        }
    }
}
