// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Boundary;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.BoundarySystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.XR;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.SDK.BoundarySystem
{
    /// <summary>
    /// The Boundary system controls the presentation and display of the users boundary in a scene.
    /// </summary>
    public class MixedRealityBoundaryManager : MixedRealityEventManager, IMixedRealityBoundarySystem
    {
        #region IMixedRealityManager Implementation

        private BoundaryEventData boundaryEventData = null;

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
            boundaryEventData = new BoundaryEventData(EventSystem.current);

            Scale = MixedRealityManager.Instance.ActiveProfile.TargetExperienceScale;

            BoundaryHeight = MixedRealityManager.Instance.ActiveProfile.BoundaryHeight;

            SetTrackingSpace();
            CalculateBoundaryBounds();
            Boundary.visible = true;

            if (ShowFloor)
            {
                GetFloorVisualization();
            }
            if (ShowPlayArea)
            {
                GetPlayAreaVisualization();
            }

            RaiseBoundaryVisualizationChanged();
        }

        /// <inheritdoc/>
        public override void Reset()
        {
            base.Reset();
            InitializeInternal();
        }

        public override void Destroy()
        {
            // Cleanup game objects created during execution.
            if (Application.isPlaying)
            {
                if (currentFloorPlane != null)
                {
                    if (Application.isEditor)
                    {
                        Object.DestroyImmediate(currentFloorPlane);
                    }
                    else
                    {
                        Object.Destroy(currentFloorPlane);
                    }
                    currentFloorPlane = null;
                }

                if (currentPlayArea != null)
                {
                    if (Application.isEditor)
                    {
                        Object.DestroyImmediate(currentPlayArea);
                    }
                    else
                    {
                        Object.Destroy(currentPlayArea);
                    }
                    currentPlayArea = null;
                }

                showFloor = false;
                showPlayArea = false;
                // todo: coming in Beta
                // set other boundary flags to false
                RaiseBoundaryVisualizationChanged();
            }
        }

        /// <summary>
        /// Raises an event to indicate that the visualization of the boundary has been changed by the boundary system.
        /// </summary>
        private void RaiseBoundaryVisualizationChanged()
        {
            boundaryEventData.Initialize(this, ShowFloor, ShowPlayArea); // todo: beta - add other boundary flags
            HandleEvent(boundaryEventData, OnVisualizationChanged);
        }

        /// <summary>
        /// Event sent whenever the boundary visualization changes.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealityBoundaryHandler> OnVisualizationChanged =
            delegate (IMixedRealityBoundaryHandler handler, BaseEventData eventData)
        {
            BoundaryEventData boundaryEventData = ExecuteEvents.ValidateEventData<BoundaryEventData>(eventData);
            handler.OnBoundaryVisualizationChanged(boundaryEventData);
        };

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

        #region IMixedRealityEventSource Implementation

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object x, object y)
        {
            // There shouldn't be other Boundary Managers to compare to.
            return false;
        }

        /// <inheritdoc />
        public int GetHashCode(object obj)
        {
            return Mathf.Abs(SourceName.GetHashCode());
        }

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        /// <inheritdoc />
        public string SourceName { get; } = "Mixed Reality Boundary System";

        #endregion IMixedRealityEventSource Implementation

        #region IMixedRealityBoundarySystem Implementation

        /// <inheritdoc/>
        public ExperienceScale Scale { get; set; }

        /// <inheritdoc/>
        public float BoundaryHeight { get; set; } = 3f;

        // todo: coming in Beta
        //// Default to not showing the boundary walls.
        // private bool showBoundaryWallss = false;
        ///// <inheritdoc/>
        //public bool ShowBoundaryWalls { get; set; } = false;

        // todo: coming in Beta
        //// Default to not showing the ceiling plane.
        // private bool showCeiling = false;
        ///// <inheritdoc/>
        //public bool ShowCeiling { get; set; } = false;

        // Default to showing the floor plane.
        private bool showFloor = true;

        /// <inheritdoc/>
        public bool ShowFloor
        {
            get { return showFloor; }
            set
            {
                if (showFloor != value)
                {
                    showFloor = value;

                    if (value && (currentFloorPlane == null))
                    {
                        GetFloorVisualization();
                    }

                    if (currentFloorPlane != null)
                    {
                        currentFloorPlane.SetActive(value);
                    }

                    RaiseBoundaryVisualizationChanged();
                }
            }
        }

        private bool showPlayArea = true;

        /// <inheritdoc/>
        public bool ShowPlayArea
        {
            get { return showPlayArea; }
            set
            {
                if (showPlayArea != value)
                {
                    showPlayArea = value;

                    if (value && (currentPlayArea == null))
                    {
                        GetPlayAreaVisualization();
                    }

                    if (currentPlayArea != null)
                    {
                        currentPlayArea.SetActive(value);
                    }

                    RaiseBoundaryVisualizationChanged();
                }
            }
        }

        // todo: coming in Beta
        //// Default to not showing the tracked area.
        // private bool showTrackedArea = false;
        ///// <inheritdoc/>
        //public bool ShowTrackedArea { get; set; } = false;

        /// <inheritdoc/>
        public Edge[] Bounds { get; private set; } = new Edge[0];

        /// <inheritdoc/>
        public float? FloorHeight { get; private set; } = null;

        /// <inheritdoc/>
        public bool Contains(Vector3 location, Boundary.Type boundaryType = Boundary.Type.TrackedArea)
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

            if (boundaryType == Boundary.Type.PlayArea)
            {
                // Check the inscribed rectangle.
                if (rectangularBounds != null)
                {
                    return rectangularBounds.IsInsideBoundary(point);
                }
            }
            else if (boundaryType == Boundary.Type.TrackedArea)
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
        public GameObject GetPlayAreaVisualization()
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
                // No rectangular bounds, therefore cannot create the play area.
                return null;
            }

            // Render the rectangular bounds.
            if (EdgeUtilities.IsValidPoint(center))
            {
                currentPlayArea = GameObject.CreatePrimitive(PrimitiveType.Quad);
                currentPlayArea.name = "Boundary System Play Area";
                currentPlayArea.transform.Translate(new Vector3(center.x, 0.005f, center.y)); // Add fudge factor to avoid z-fighting
                currentPlayArea.transform.Rotate(new Vector3(90, -angle, 0));
                currentPlayArea.transform.localScale = new Vector3(width, height, 1.0f);
                currentPlayArea.GetComponent<Renderer>().sharedMaterial = MixedRealityManager.Instance.ActiveProfile.BoundaryVisualizationProfile.PlayAreaMaterial;
            }

            return currentPlayArea;
        }

        /// <inheritdoc/>
        public GameObject GetFloorVisualization()
        {
            if (currentFloorPlane != null)
            {
                return currentFloorPlane;
            }

            if (!FloorHeight.HasValue)
            {
                // We were unable to locate the floor.
                return null;
            }

            // Render the floor.
            currentFloorPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
            currentFloorPlane.name = "Boundary System Floor";
            currentFloorPlane.transform.Translate(new Vector3(0f, FloorHeight.Value, 0f));
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

            if (Boundary.TryGetGeometry(boundaryGeometry, Boundary.Type.TrackedArea))
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
