// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.SpatialObservers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.SpatialObservers
{
    /// <summary>
    /// Base <see cref="IMixedRealitySurfaceObserver"/> implementation.
    /// </summary>
    public abstract class BaseMixedRealitySurfaceObserver : BaseMixedRealitySpatialObserverDataProvider, IMixedRealitySurfaceObserver
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        protected BaseMixedRealitySurfaceObserver(string name, uint priority, BaseMixedRealitySurfaceObserverProfile profile) : base(name, priority, profile)
        {
            if (profile == null)
            {
                throw new Exception($"Missing profile for {name}");
            }

            SurfacePhysicsLayerOverride = profile.SurfacePhysicsLayerOverride;
            SurfaceFindingMinimumArea = profile.SurfaceFindingMinimumArea;
            DisplayFloorSurfaces = profile.DisplayFloorSurfaces;
            FloorSurfaceMaterial = profile.FloorSurfaceMaterial;
            DisplayCeilingSurfaces = profile.DisplayCeilingSurface;
            CeilingSurfaceMaterial = profile.CeilingSurfaceMaterial;
            DisplayWallSurfaces = profile.DisplayWallSurface;
            WallSurfaceMaterial = profile.WallSurfaceMaterial;
            DisplayPlatformSurfaces = profile.DisplayPlatformSurfaces;
            PlatformSurfaceMaterial = profile.PlatformSurfaceMaterial;
        }

        /// <inheritdoc />
        public override int PhysicsLayer => SurfacePhysicsLayerOverride == -1 ? base.PhysicsLayer : SurfacePhysicsLayerOverride;


        /// <inheritdoc />
        public int SurfacePhysicsLayerOverride { get; }

        /// <inheritdoc />
        public float SurfaceFindingMinimumArea { get; }

        /// <inheritdoc />
        public bool DisplayFloorSurfaces { get; set; }

        /// <inheritdoc />
        public Material FloorSurfaceMaterial { get; }

        /// <inheritdoc />
        public bool DisplayCeilingSurfaces { get; set; }

        /// <inheritdoc />
        public Material CeilingSurfaceMaterial { get; }

        /// <inheritdoc />
        public bool DisplayWallSurfaces { get; set; }

        /// <inheritdoc />
        public Material WallSurfaceMaterial { get; }

        /// <inheritdoc />
        public bool DisplayPlatformSurfaces { get; set; }

        /// <inheritdoc />
        public Material PlatformSurfaceMaterial { get; }

        private readonly Dictionary<int, GameObject> planarSurfaces = new Dictionary<int, GameObject>();

        /// <inheritdoc />
        public IReadOnlyDictionary<int, GameObject> PlanarSurfaces => new Dictionary<int, GameObject>(planarSurfaces);
    }
}