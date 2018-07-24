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
        public Edge[] Bounds { get; private set; } = new Edge[0];

        /// <inheritdoc/>
        public float? FloorHeight { get; private set; } = null;

        /// <summary>
        /// The largest rectangle that is contained withing the playspace geometry.
        /// </summary>
        private InscribedRectangle rectangularBounds = null;

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

            // Handle the user teleporting (boundary moves with them).
            location = CameraCache.Main.transform.parent.InverseTransformPoint(location);

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
                if (rectangularBounds != null)
                {
                    return rectangularBounds.IsInsideBoundary(point);
                }
            }
            else if(boundaryType == Boundary.Type.TrackedArea)
            {
                // Check the geometry
                return EdgeUtilities.IsInsideBoundary(Bounds, point);
            }

            // Not in either boundary type.
            return false;
        }

        /// <inheritdoc/>
        public bool TryGetRectangularBounds(
            out Vector2 center, 
            out float angle, 
            out float width,
            out float height)
        {
            if (!rectangularBounds.IsValid)
            {
                center = EdgeUtilities.InvalidPoint;
                angle = 0f;
                width = 0f;
                height = 0f;
                return false;
            }

            // Handle the user teleporting (boundary moves with them).
            Vector3 transformedCenter = CameraCache.Main.transform.parent.TransformPoint(
                new Vector3(rectangularBounds.Center.x, 0f, rectangularBounds.Center.y));

            center = new Vector2(transformedCenter.x, transformedCenter.z);
            angle = rectangularBounds.Angle;
            width = rectangularBounds.Width;
            height = rectangularBounds.Height;
            return true;
        }

        /// <summary>
        /// Retrieves the boundary geometry and creates the boundary and inscribed playspace volumes.
        /// </summary>
        private void CalculateBoundaryBounds()
        {
            // Reset the bounds
            Bounds = new Edge[0];
            FloorHeight = null;
            rectangularBounds = null;

            // Boundaries are supported for Room Scale experiences only.
            if (XRDevice.GetTrackingSpaceType() != TrackingSpaceType.RoomScale)
            {
                return;
            }

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
                Bounds = boundaryEdges.ToArray();
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
            rectangularBounds = new InscribedRectangle(Bounds, Mathf.Abs("Mixed Reality Toolkit".GetHashCode()));
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
            Boundary.visible = EnablePlatformBoundaryRendering;
        }
    }
}
