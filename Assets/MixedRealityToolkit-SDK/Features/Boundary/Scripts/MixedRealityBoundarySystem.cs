// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.BoundarySystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.SDK.BoundarySystem
{
    /// <summary>
    /// The Boundary system controls the presentation and display of the users boundary in a scene.
    /// </summary>
    public class MixedRealityBoundaryManager : MixedRealityEventManager, IMixedRealityBoundarySystem
    {
        #region IMixedRealityManager Implementation

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
            Scale = MixedRealityManager.Instance.ActiveProfile.TargetExperienceScale;
            BoundaryHeight = MixedRealityManager.Instance.ActiveProfile.BoundaryHeight;
            EnablePlatformBoundaryRendering = MixedRealityManager.Instance.ActiveProfile.IsPlatformBoundaryRenderingEnabled;

            SetTrackingSpace();
            CalculateBoundaryBounds();
            UnityEngine.Experimental.XR.Boundary.visible = EnablePlatformBoundaryRendering;
        }

        /// <inheritdoc/>
        public override void Reset()
        {
            base.Reset();
            InitializeInternal();
        }

        public override void Destroy()
        {
            EnablePlatformBoundaryRendering = false;
        }

        #endregion IMixedRealityManager Implementation

        #region IMixedRealtyEventSystem Implementation

        /// <inheritdoc />
        public override void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
        {
            base.HandleEvent(eventData, eventHandler);
        }

        /// <summary>
        /// Registers the <see cref="GameObject"/> to listen for boundary events.
        /// </summary>
        /// <param name="listener"></param>
        public override void Register(GameObject listener)
        {
            base.Register(listener);
        }

        /// <summary>
        /// UnRegisters the <see cref="GameObject"/> to listen for boundary events.
        /// /// </summary>
        /// <param name="listener"></param>
        public override void Unregister(GameObject listener)
        {
            base.Unregister(listener);
        }

        #endregion

        #region IMixedRealityBoundarySystem Implementation

        /// <inheritdoc/>
        public ExperienceScale Scale { get; set; }

        /// <inheritdoc/>
        public float BoundaryHeight { get; set; }

        /// <inheritdoc/>
        public bool EnablePlatformBoundaryRendering
        {
            get { return enablePlatformBoundaryRendering; }
            set
            {
                enablePlatformBoundaryRendering = value;
                UnityEngine.Experimental.XR.Boundary.visible = value;

                if (value)
                {
                    CreateFloorPlaneVisualization();
                    CreatePlaySpaceVisualization();
                }
                else
                {
                    Object.Destroy(currentPlayArea);
                    Object.Destroy(currentFloorPlane);
                }
            }
        }

        private bool enablePlatformBoundaryRendering;

        /// <inheritdoc/>
        public Edge[] Bounds { get; private set; } = new Edge[0];

        /// <inheritdoc/>
        public float? FloorHeight { get; private set; } = null;

        /// <inheritdoc/>
        public bool Contains(Vector3 location, UnityEngine.Experimental.XR.Boundary.Type boundaryType = UnityEngine.Experimental.XR.Boundary.Type.TrackedArea)
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

            if (FloorHeight.Value > location.y ||
                BoundaryHeight < location.y)
            {
                // Location below the floor or above the boundary height.
                return false;
            }

            // Boundary coordinates are always "on the floor"
            Vector2 point = new Vector2(location.x, location.z);

            if (boundaryType == UnityEngine.Experimental.XR.Boundary.Type.PlayArea)
            {
                // Check the inscribed rectangle.
                if (rectangularBounds != null)
                {
                    return rectangularBounds.IsInsideBoundary(point);
                }
            }
            else if (boundaryType == UnityEngine.Experimental.XR.Boundary.Type.TrackedArea)
            {
                // Check the geometry
                return EdgeUtilities.IsInsideBoundary(Bounds, point);
            }

            // Not in either boundary type.
            return false;
        }

        /// <inheritdoc/>
        public bool TryGetRectangularBoundsParams(out Vector2 center, out float angle, out float width, out float height)
        {
            if (rectangularBounds == null || !rectangularBounds.IsValid)
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

        /// <inheritdoc/>
        public GameObject CreatePlaySpaceVisualization()
        {
            if (currentPlayArea != null)
            {
                return currentPlayArea;
            }

            // Get the rectangular bounds.
            Vector2 center;
            float angle;
            float width;
            float height;

            if (!TryGetRectangularBoundsParams(out center, out angle, out width, out height))
            {
                // No rectangular bounds, therefore do not render the quad.
                return null;
            }

            // Render the rectangular bounds.
            if (EdgeUtilities.IsValidPoint(center))
            {
                currentPlayArea = GameObject.CreatePrimitive(PrimitiveType.Quad);
                currentPlayArea.name = "Boundary System Play Area Visualization";
                currentPlayArea.transform.Translate(new Vector3(center.x, 0.005f, center.y)); // Add fudge factor to avoid z-fighting
                currentPlayArea.transform.Rotate(new Vector3(90, -angle, 0));
                currentPlayArea.transform.localScale = new Vector3(width, height, 1.0f);
                currentPlayArea.GetComponent<Renderer>().sharedMaterial = MixedRealityManager.Instance.ActiveProfile.BoundaryVisualizationProfile.PlayAreaMaterial;
            }

            return currentPlayArea;
        }

        /// <inheritdoc/>
        public GameObject CreateFloorPlaneVisualization()
        {
            if (currentFloorPlane != null)
            {
                return currentFloorPlane;
            }

            // Render the floor.
            currentFloorPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
            currentFloorPlane.transform.Translate(Vector3.zero);
            currentFloorPlane.transform.Rotate(90, 0, 0);
            currentFloorPlane.transform.localScale = MixedRealityManager.Instance.ActiveProfile.BoundaryVisualizationProfile.FloorPlaneScale;
            currentFloorPlane.GetComponent<Renderer>().sharedMaterial = MixedRealityManager.Instance.ActiveProfile.BoundaryVisualizationProfile.FloorPlaneMaterial;

            return currentFloorPlane;
        }

        #endregion IMixedRealityBoundarySystem Implementation

        /// <summary>
        /// The largest rectangle that is contained withing the play space geometry.
        /// </summary>
        private InscribedRectangle rectangularBounds = null;

        private GameObject currentPlayArea;
        private GameObject currentFloorPlane;

        /// <summary>
        /// Retrieves the boundary geometry and creates the boundary and inscribed play space volumes.
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
            var boundaryGeometry = new List<Vector3>(0);
            var boundaryEdges = new List<Edge>(0);

            if (UnityEngine.Experimental.XR.Boundary.TryGetGeometry(boundaryGeometry, UnityEngine.Experimental.XR.Boundary.Type.TrackedArea))
            {
                // FloorHeight starts out as null. Use a suitably high value for the floor to ensure
                // that we do not accidentally set it too low.
                float floorHeight = float.MaxValue;

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
                Debug.LogWarning("Failed to calculate boundary bounds.");
            }
        }

        /// <summary>
        /// Creates the two dimensional volume described by the largest rectangle that
        /// is contained withing the play space geometry and the configured height.
        /// </summary>
        private void CreateInscribedBounds()
        {
            // We always use the same seed so that from run to run, the inscribed bounds are consistent.
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
                // TODO: how best to handle this scenario?
            }
        }
    }
}
