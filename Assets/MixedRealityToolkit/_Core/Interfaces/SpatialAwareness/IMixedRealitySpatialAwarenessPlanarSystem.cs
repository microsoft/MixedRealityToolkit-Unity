// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Events;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Observers;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem
{

    public interface IMixedRealitySpatialAwarenessPlanarSystem : IMixedRealitySpatialAwarenessSystem
    {
        #region Surface Finding Handling

        /// <summary>
        /// Indicates if the surface finding subsystem is in use by the application.
        /// </summary>
        /// <remarks>
        /// Setting this to false will suspend all surface finding events and cause the subsystem to return an empty collection
        /// when the GetSurfaces method is called.
        /// </remarks>
        bool UseSurfaceFindingSystem { get; set; }

        /// <summary>
        /// Get or sets the desired Unity Physics Layer on which to set the planar surfaces.
        /// </summary>
        int SurfacePhysicsLayer { get; set; }

        /// <summary>
        /// Gets the bit mask that corresponds to the value specified in <see cref="SurfacePhysicsLayer"/>.
        /// </summary>
        int SurfacePhysicsLayerMask { get; }

        /// <summary>
        /// Gets or sets the minimum surface area, in square meters, that must be satisfied before a surface is identified.
        /// </summary>
        float SurfaceFindingMinimumArea { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the surface finding subsystem is to automatically display
        /// floor surfaces within the application. When enabled, the surfaces will be added to the scene
        /// and rendered using the configured <see cref="FloorSurfaceMaterial"/>.
        /// </summary>
        bool DisplayFloorSurfaces { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when rendering floor surfaces.
        /// </summary>
        Material FloorSurfaceMaterial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the surface finding subsystem is to automatically display
        /// ceiling surfaces within the application. When enabled, the surfaces will be added to the scene
        /// and rendered using the configured <see cref="CeilingSurfaceMaterial"/>.
        /// </summary>
        bool DisplayCeilingSurfaces { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when rendering ceiling surfaces.
        /// </summary>
        Material CeilingSurfaceMaterial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the surface finding subsystem is to automatically display
        /// wall surfaces within the application. When enabled, the surfaces will be added to the scene
        /// and rendered using the configured <see cref="WallSurfaceMaterial"/>.
        /// </summary>
        bool DisplayWallSurfaces { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when rendering wall surfaces.
        /// </summary>
        Material WallSurfaceMaterial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the surface finding subsystem is to automatically display
        /// horizontal platform surfaces within the application. When enabled, the surfaces will be added to the scene
        /// and rendered using the configured <see cref="PlatformSurfaceMaterial"/>.
        /// </summary>
        bool DisplayPlatformSurfaces { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when rendering horizontal platform surfaces.
        /// </summary>
        Material PlatformSurfaceMaterial { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="GameObject"/>s being managed by the spatial awareness surface finding subsystem.
        /// </summary>
        IReadOnlyDictionary<int, SpatialAwarenessPlanarObject> PlanarSurfaces { get; }

        #region Surface Finding Events

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessSurfaceFindingHandler.OnSurfaceAdded"/> method to indicate a planar surface has been added.
        /// </summary>
        /// <param name="observer">The observer raising the event.</param>
        /// <param name="surfaceId">Value identifying the surface.</param>
        /// <param name="surfaceObject">The surface <see cref="GameObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseSurfaceAdded(IMixedRealitySpatialAwarenessPlaneObserver observer, int surfaceId, GameObject surfaceObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessSurfaceFindingHandler.OnSurfaceUpdated"/> method to indicate an existing planar surface has been updated.
        /// </summary>
        /// <param name="observer">The observer raising the event.</param>
        /// <param name="surfaceId">Value identifying the surface.</param>
        /// <param name="surfaceObject">The surface <see cref="GameObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseSurfaceUpdated(IMixedRealitySpatialAwarenessPlaneObserver observer, int surfaceId, GameObject surfaceObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessSurfaceFindingHandler.OnSurfaceUpdated"/> method to indicate an existing planar surface has been removed.
        /// </summary>
        /// <param name="observer">The observer raising the event.</param>
        /// <param name="surfaceId">Value identifying the surface.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseSurfaceRemoved(IMixedRealitySpatialAwarenessPlaneObserver observer, int surfaceId);

        #endregion Surface Finding Events

        #endregion Surface Finding Handling
    }
}
