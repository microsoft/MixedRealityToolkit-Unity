// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.SpatialObservers
{
    public interface IMixedRealitySurfaceObserver : IMixedRealitySpatialObserverDataProvider
    {
        /// <summary>
        /// Overrides the <see cref="IMixedRealitySpatialObserverDataProvider.PhysicsLayer"/> with the provided value.
        /// </summary>
        int SurfacePhysicsLayerOverride { get; }

        /// <summary>
        /// Gets or sets the minimum surface area, in square meters, that must be satisfied before a surface is identified.
        /// </summary>
        float SurfaceFindingMinimumArea { get; }

        /// <summary>
        /// Gets or sets a value indicating if the surface finding subsystem is to automatically display
        /// floor surfaces within the application. When enabled, the surfaces will be added to the scene
        /// and rendered using the configured <see cref="FloorSurfaceMaterial"/>.
        /// </summary>
        bool DisplayFloorSurfaces { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when rendering floor surfaces.
        /// </summary>
        Material FloorSurfaceMaterial { get; }

        /// <summary>
        /// Gets or sets a value indicating if the surface finding subsystem is to automatically display
        /// ceiling surfaces within the application. When enabled, the surfaces will be added to the scene
        /// and rendered using the configured <see cref="CeilingSurfaceMaterial"/>.
        /// </summary>
        bool DisplayCeilingSurfaces { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when rendering ceiling surfaces.
        /// </summary>
        Material CeilingSurfaceMaterial { get; }

        /// <summary>
        /// Gets or sets a value indicating if the surface finding subsystem is to automatically display
        /// wall surfaces within the application. When enabled, the surfaces will be added to the scene
        /// and rendered using the configured <see cref="WallSurfaceMaterial"/>.
        /// </summary>
        bool DisplayWallSurfaces { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when rendering wall surfaces.
        /// </summary>
        Material WallSurfaceMaterial { get; }

        /// <summary>
        /// Gets or sets a value indicating if the surface finding subsystem is to automatically display
        /// horizontal platform surfaces within the application. When enabled, the surfaces will be added to the scene
        /// and rendered using the configured <see cref="PlatformSurfaceMaterial"/>.
        /// </summary>
        bool DisplayPlatformSurfaces { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when rendering horizontal platform surfaces.
        /// </summary>
        Material PlatformSurfaceMaterial { get; }

        /// <summary>
        /// Gets the collection of <see cref="GameObject"/>s being managed by the spatial awareness surface finding subsystem.
        /// </summary>
        IReadOnlyDictionary<int, GameObject> PlanarSurfaces { get; }
    }
}