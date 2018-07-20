// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Internal.Managers
{
    /// <summary>
    /// The Boundary system controls the presentation and display of the users boundary in a scene
    /// </summary>
    public class MixedRealityBoundaryManager : BaseManager, IMixedRealityBoundarySystem
    {
        /// <inheritdoc/>
        public ExperienceScale Scale { get; set; } = ExperienceScale.Room;

        /// <inheritdoc/>
        public float BoundaryHeight { get; set; } = 3.0f;

        /// <inheritdoc/>
        public bool EnablePlatformBoundaryRendering { get; set; } = true;

        /// <inheritdoc/>
        public Edge[] GeometryBounds { get; private set; } = new Edge[0];

        /// <inheritdoc/>
        public float? FloorHeight { get; private set; } = null;

        /// <inheritdoc/>
        public InscribedRectangle InscribedRectangularBounds { get; private set; } = null;

        /// <summary>
        /// MixedRealityBoundaryManager constructor
        /// </summary>
        public MixedRealityBoundaryManager()
        {
            Scale = MixedRealityManager.Instance.ActiveProfile.TargetExperienceScale;
            BoundaryHeight = MixedRealityManager.Instance.ActiveProfile.BoundaryHeight;
            EnablePlatformBoundaryRendering = MixedRealityManager.Instance.ActiveProfile.EnablePlatformBoundaryRendering;
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            base.Initialize();
            InitializeInternal();
        }

        /// <summary>
        /// Performs initialization tasks for the BoundaryManager.
        /// </summary>
        private void InitializeInternal()
        {
            SetTrackingSpace();
            CalculateBoundaryBounds();
            SetPlatformBoundaryVisibility();
        }

        /// <inheritdoc/>
        public override void Reset()
        {
            base.Reset();
            InitializeInternal();
        }

        public bool Contains(Vector3 location)
        {
            return Contains(location, Boundary.Type.TrackedArea);
        }

        /// <inheritdoc/>
        public bool Contains(Vector3 location, Boundary.Type boundaryType)
        {
            if (!EdgeUtilities.IsValidPoint(location))
            {
                // Invalid location.
                return false;
            }

            if (!FloorHeight.HasValue)
            {
                // No floor.
                return false;
            }

            if ((FloorHeight.Value > location.y) ||
                (BoundaryHeight < location.y))
            {
                // Location below the floor or above the boundary height.
                return false;
            }

            // Boundary coordinates are always "on the floor"
            Vector2 point = new Vector2(location.x, location.z);

            if (boundaryType == Boundary.Type.PlayArea)
            {
                // Check the inscribed rectangle.
                if (InscribedRectangularBounds != null)
                {
                    return InscribedRectangularBounds.IsInsideBoundary(point);
                }
            }
            else if(boundaryType == Boundary.Type.TrackedArea)
            {
                // Check the geometry
                return EdgeUtilities.IsInsideBoundary(GeometryBounds, point);
            }

            // Not in either boundary type.
            return false;
        }

        /// <summary>
        /// Retrieves the boundary geometry and creates the boundary and inscribed playspace volumes.
        /// </summary>
        private void CalculateBoundaryBounds()
        {
            // Reset the bounds
            GeometryBounds = new Edge[0];
            FloorHeight = null;
            InscribedRectangularBounds = null;

            // Boundaries are supported for Room Scale experiences only.
            if (XRDevice.GetTrackingSpaceType() != TrackingSpaceType.RoomScale)
            {
                return;
            }

#if !UNITY_WSA
            // This always returns false in Windows Mixed Reality.
            if (!Boundary.configured)
            {
                Debug.Log("No boundary configured.");
                return;
            }
#endif // !UNITY_WSA

            // Get the boundary geometry.
            List<Vector3> boundaryGeometry = new List<Vector3>(0);
            List<Edge> boundaryEdges = new List<Edge>(0);

            if (Boundary.TryGetGeometry(boundaryGeometry, Boundary.Type.TrackedArea))
            {
                // FloorHeight starts out as null. Use a suitably high value for the floor to ensure
                // that we do not accidentally set it too low.
                float floorHeight = 10000f;

                for (int i = 0; i < boundaryGeometry.Count; i++)
                {
                    Vector3 pointA = boundaryGeometry[i];
                    Vector3 pointB = boundaryGeometry[(i + 1) % boundaryGeometry.Count];
                    boundaryEdges.Add(new Edge(pointA, pointB));

                    floorHeight = Mathf.Min(floorHeight, boundaryGeometry[i].y); 
                }

                FloorHeight = floorHeight;
                GeometryBounds = boundaryEdges.ToArray();
                CreateInscribedBounds();
            }
            else
            {
                // TODO: How do we determine if we have a floor without boundaries
                Debug.LogWarning("Failed to calculate boundary bounds.");
            }
        }

        /// <summary>
        /// Creates the two dimensional volume described by the largest rectangle that
        /// is contained withing the playspace geoometry and the configured height.
        /// </summary>
        private void CreateInscribedBounds()
        {
            // We always use the same seed so that from run to run, the inscribed bounds are
            // consistent.
            InscribedRectangularBounds = new InscribedRectangle(GeometryBounds, Mathf.Abs("Mixed Reality Toolkit".GetHashCode()));
        }

        /// <summary>
        /// Updates the <see cref="TrackingSpaceType"/> on the XR device.
        /// </summary>
        private void SetTrackingSpace()
        {
            TrackingSpaceType trackingSpace;

            // In current versions of Unity, there are two types of tracking spaces. For boundaries, if the scale
            // is not Room or Standing, it currently maps to TrackingSpaceType.Stationary.
            switch (Scale)
            {
                case ExperienceScale.Standing:
                case ExperienceScale.Room:
                    trackingSpace = TrackingSpaceType.RoomScale;
                    break;

                case ExperienceScale.OrientationOnly:
                case ExperienceScale.Seated:
                case ExperienceScale.World:
                    trackingSpace = TrackingSpaceType.Stationary;
                    break;

                default:
                    trackingSpace = TrackingSpaceType.Stationary;
                    Debug.LogWarning("Unknown / unsupported ExperienceScale. Defaulting to Stationary tracking space.");
                    break;
            }

            bool trackingSpaceSet = XRDevice.SetTrackingSpaceType(trackingSpace);
            if (!trackingSpaceSet)
            {
                // TODO: Implement ExperienceScale fallback logic
            }
        }

        /// <summary>
        /// Sets the property indicating if the boundary should be rendered by the platform.
        /// </summary>
        /// <remarks>
        /// Not all platforms support specifying whether or not to render the playspace boundary.
        /// For platforms without boundary rendering control, the default behavior will be unchanged 
        /// regardless of the value provided.
        /// </remarks>
        private void SetPlatformBoundaryVisibility()
        {
#if !UNITY_WSA
            // This always returns false in Windows Mixed Reality.
            if (!Boundary.configured)
            {
                Debug.Log("No boundary configured.");
                return;
            }

            // This value cannot be configured on Windows Mixed Reality. Automatic boundary rendering is performed.
            Boundary.visible = EnablePlatformBoundaryRendering;
#endif // !UNITY_WSA
        }
    }
}
