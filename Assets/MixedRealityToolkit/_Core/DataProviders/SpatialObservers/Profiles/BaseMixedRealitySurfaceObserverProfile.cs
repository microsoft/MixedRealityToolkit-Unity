// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Attributes;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.SpatialObservers
{
    // No Scriptable Object Menu constructor attributes here, as this class is meant to be inherited.

    /// <summary>
    /// The base surface observer profile.
    /// </summary>
    public abstract class BaseMixedRealitySurfaceObserverProfile : BaseMixedRealitySpatialObserverProfile
    {
        [PhysicsLayer]
        [SerializeField]
        [Tooltip("Optional physics layer override to specify for generated surface objects.")]
        private int surfacePhysicsLayerOverride = -1;

        /// <summary>
        /// The surface physics layer override to use on generated surface objects.
        /// </summary>
        public int SurfacePhysicsLayerOverride => surfacePhysicsLayerOverride;

        [SerializeField]
        [Tooltip("The minimum area, in square meters, of the planar surfaces")]
        private float surfaceFindingMinimumArea = 0.025f;

        /// <summary>
        /// The minimum area, in square meters, of the planar surfaces.
        /// </summary>
        public float SurfaceFindingMinimumArea => surfaceFindingMinimumArea;

        [SerializeField]
        [Tooltip("Automatically display floor surfaces?")]
        private bool displayFloorSurfaces = false;

        /// <summary>
        /// Indicates if the surface finding subsystem is to automatically display floor surfaces within the application.
        /// </summary>
        public bool DisplayFloorSurfaces => displayFloorSurfaces;

        [SerializeField]
        [Tooltip("Material to use when displaying floor surfaces")]
        private Material floorSurfaceMaterial = null;

        /// <summary>
        /// The material to be used when automatically displaying floor surfaces.
        /// </summary>
        public Material FloorSurfaceMaterial => floorSurfaceMaterial;

        [SerializeField]
        [Tooltip("Automatically display ceiling surfaces?")]
        private bool displayCeilingSurfaces = false;

        /// <summary>
        /// Indicates if the surface finding subsystem is to automatically display ceiling surfaces within the application.
        /// </summary>
        public bool DisplayCeilingSurface => displayCeilingSurfaces;

        [SerializeField]
        [Tooltip("Material to use when displaying ceiling surfaces")]
        private Material ceilingSurfaceMaterial = null;

        /// <summary>
        /// The material to be used when automatically displaying ceiling surfaces.
        /// </summary>
        public Material CeilingSurfaceMaterial => ceilingSurfaceMaterial;

        [SerializeField]
        [Tooltip("Automatically display wall surfaces?")]
        private bool displayWallSurfaces = false;

        /// <summary>
        /// Indicates if the surface finding subsystem is to automatically display wall surfaces within the application.
        /// </summary>
        public bool DisplayWallSurface => displayWallSurfaces;

        [SerializeField]
        [Tooltip("Material to use when displaying wall surfaces")]
        private Material wallSurfaceMaterial = null;

        /// <summary>
        /// The material to be used when automatically displaying wall surfaces.
        /// </summary>
        public Material WallSurfaceMaterial => wallSurfaceMaterial;

        [SerializeField]
        [Tooltip("Automatically display platform surfaces?")]
        private bool displayPlatformSurfaces = false;

        /// <summary>
        /// Indicates if the surface finding subsystem is to automatically display platform surfaces within the application.
        /// </summary>
        public bool DisplayPlatformSurfaces => displayPlatformSurfaces;

        [SerializeField]
        [Tooltip("Material to use when displaying platform surfaces")]
        private Material platformSurfaceMaterial = null;

        /// <summary>
        /// The material to be used when automatically displaying platform surfaces.
        /// </summary>
        public Material PlatformSurfaceMaterial => platformSurfaceMaterial;
    }
}