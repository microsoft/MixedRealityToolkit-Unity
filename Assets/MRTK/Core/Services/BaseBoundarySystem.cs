// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Boundary
{
    public abstract class BaseBoundarySystem : BaseCoreSystem, IMixedRealityBoundarySystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile">The configuration profile for the service.</param>
        /// <param name="scale">The application's configured <see cref="Utilities.ExperienceScale"/>.</param>
        protected BaseBoundarySystem(
            MixedRealityBoundaryVisualizationProfile profile,
            ExperienceScale scale) : base(profile)
        {
            Scale = scale;
            BoundaryProfile = profile;
        }

        /// <summary>
        /// Reads the visualization profile contents and stores the values in class properties.
        /// </summary>
        private void ReadProfile()
        {
            if (BoundaryProfile == null) { return; }

            BoundaryHeight = BoundaryProfile.BoundaryHeight;
            ShowFloor = BoundaryProfile.ShowFloor;
            FloorPhysicsLayer = BoundaryProfile.FloorPhysicsLayer;
            ShowPlayArea = BoundaryProfile.ShowPlayArea;
            PlayAreaPhysicsLayer = BoundaryProfile.PlayAreaPhysicsLayer;
            ShowTrackedArea = BoundaryProfile.ShowTrackedArea;
            TrackedAreaPhysicsLayer = BoundaryProfile.TrackedAreaPhysicsLayer;
            ShowBoundaryWalls = BoundaryProfile.ShowBoundaryWalls;
            BoundaryWallsPhysicsLayer = BoundaryProfile.BoundaryWallsPhysicsLayer;
            ShowBoundaryCeiling = BoundaryProfile.ShowBoundaryCeiling;
            CeilingPhysicsLayer = BoundaryProfile.CeilingPhysicsLayer;
        }

        /// <summary>
        /// Whether any XR device is present.
        /// </summary>
        [System.Obsolete("This value is no longer used.")]
        protected virtual bool IsXRDevicePresent { get; } = true;

        #region IMixedRealityService Implementation

        private MixedRealityBoundaryVisualizationProfile BoundaryProfile { get; }

        private BoundaryEventData boundaryEventData = null;

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "Mixed Reality Boundary System";

        /// <inheritdoc/>
        public override void Initialize()
        {
            // Initialize this value earlier than other systems, so we can use it to block boundary events being raised too early
            IsInitialized = false;

            // The profile needs to be read on initialization to ensure that re-initialization
            // after profile change reads the correct data.
            ReadProfile();

            if (!Application.isPlaying || !DeviceUtility.IsPresent) { return; }

            boundaryEventData = new BoundaryEventData(EventSystem.current);

            SetTrackingSpace();
            CalculateBoundaryBounds();

            base.Initialize();

            RefreshVisualization();
            RaiseBoundaryVisualizationChanged();
        }

#if UNITY_EDITOR
        public override void Update()
        {
            base.Update();

            // If a device is attached late, initialize with the new state of the world
            if (!IsInitialized && DeviceUtility.IsPresent)
            {
                Initialize();
            }
        }
#endif // UNITY_EDITOR

        /// <inheritdoc/>
        public override void Destroy()
        {
            // First, detach the child objects (we are tracking them separately)
            // and clean up the parent.
            if (boundaryVisualizationParent != null)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(boundaryVisualizationParent);
                }
                else
                {
                    boundaryVisualizationParent.transform.DetachChildren();
                    Object.Destroy(boundaryVisualizationParent);
                }

                boundaryVisualizationParent = null;
            }

            // Next, clean up the detached children.
            if (currentFloorObject != null)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(currentFloorObject);
                }
                else
                {
                    Object.Destroy(currentFloorObject);
                }
                currentFloorObject = null;
            }

            if (currentPlayAreaObject != null)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(currentPlayAreaObject);
                }
                else
                {
                    Object.Destroy(currentPlayAreaObject);
                }
                currentPlayAreaObject = null;
            }

            if (currentTrackedAreaObject != null)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(currentTrackedAreaObject);
                }
                else
                {
                    Object.Destroy(currentTrackedAreaObject);
                }
                currentTrackedAreaObject = null;
            }

            if (currentBoundaryWallObject != null)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(currentBoundaryWallObject);
                }
                else
                {
                    Object.Destroy(currentBoundaryWallObject);
                }
                currentBoundaryWallObject = null;
            }

            if (currentCeilingObject != null)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(currentCeilingObject);
                }
                else
                {
                    Object.Destroy(currentCeilingObject);
                }
                currentCeilingObject = null;
            }

            showFloor = false;
            showPlayArea = false;
            showTrackedArea = false;
            showBoundaryWalls = false;
            showCeiling = false;

            RaiseBoundaryVisualizationChanged();

            base.Destroy();
        }


        /// <summary>
        /// Raises an event to indicate that the visualization of the boundary has been changed by the boundary system.
        /// </summary>
        private void RaiseBoundaryVisualizationChanged()
        {
            if (!Application.isPlaying || boundaryEventData == null) { return; }
            boundaryEventData.Initialize(this, ShowFloor, ShowPlayArea, ShowTrackedArea, ShowBoundaryWalls, ShowBoundaryCeiling);
            HandleEvent(boundaryEventData, OnVisualizationChanged);
        }

        /// <summary>
        /// Event sent whenever the boundary visualization changes.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealityBoundaryHandler> OnVisualizationChanged =
            delegate (IMixedRealityBoundaryHandler handler, BaseEventData eventData)
            {
                var boundaryEventData = ExecuteEvents.ValidateEventData<BoundaryEventData>(eventData);
                handler.OnBoundaryVisualizationChanged(boundaryEventData);
            };

        #endregion IMixedRealityService Implementation

        #region IMixedRealtyEventSystem Implementation

        /// <inheritdoc />
        public override void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
        {
            base.HandleEvent(eventData, eventHandler);
        }

        /// <summary>
        /// Registers the <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> to listen for boundary events.
        /// </summary>
        public override void Register(GameObject listener)
        {
            base.Register(listener);
        }

        /// <summary>
        /// UnRegisters the <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> to listen for boundary events.
        /// /// </summary>
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

        /// <summary>
        /// The thickness of three dimensional generated boundary objects.
        /// </summary>
        private const float boundaryObjectThickness = 0.005f;

        /// <summary>
        /// A small offset to avoid render conflicts, primarily with the floor.
        /// </summary>
        /// <remarks>
        /// This offset is used to avoid consuming multiple physics layers.
        /// </remarks>
        private const float boundaryObjectRenderOffset = 0.001f;

        private GameObject boundaryVisualizationParent;

        /// <summary>
        /// Parent <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> which will encapsulate all of the teleportable boundary visualizations.
        /// </summary>
        private GameObject BoundaryVisualizationParent
        {
            get
            {
                if (boundaryVisualizationParent != null)
                {
                    return boundaryVisualizationParent;
                }

                var visualizationParent = new GameObject("Boundary System Visualizations");
                MixedRealityPlayspace.AddChild(visualizationParent.transform);
                return boundaryVisualizationParent = visualizationParent;
            }
        }

        /// <summary>
        /// Layer used to tell the (non-floor) boundary objects to not accept raycasts
        /// </summary>
        private readonly int ignoreRaycastLayerValue = 2;

        private MixedRealityBoundaryVisualizationProfile boundaryVisualizationProfile = null;

        /// <inheritdoc/>
        public MixedRealityBoundaryVisualizationProfile BoundaryVisualizationProfile
        {
            get
            {
                if (boundaryVisualizationProfile == null)
                {
                    boundaryVisualizationProfile = ConfigurationProfile as MixedRealityBoundaryVisualizationProfile;
                }
                return boundaryVisualizationProfile;
            }
        }

        /// <inheritdoc/>
        public ExperienceScale Scale { get; set; }

        /// <inheritdoc/>
        public float BoundaryHeight { get; set; } = 3f;

        private bool showFloor = false;

        /// <inheritdoc/>
        public bool ShowFloor
        {
            get { return showFloor; }
            set
            {
                if (showFloor != value)
                {
                    showFloor = value;

                    PropertyAction(value, currentFloorObject, () => GetFloorVisualization());
                }
            }
        }

        private bool showPlayArea = false;

        private int floorPhysicsLayer;

        /// <inheritdoc/>
        public int FloorPhysicsLayer
        {
            get
            {
                if (currentFloorObject != null)
                {
                    floorPhysicsLayer = currentFloorObject.layer;
                }

                return floorPhysicsLayer;
            }
            set
            {
                floorPhysicsLayer = value;
                if (currentFloorObject != null)
                {
                    currentFloorObject.layer = floorPhysicsLayer;
                }
            }
        }

        /// <inheritdoc/>
        public bool ShowPlayArea
        {
            get { return showPlayArea; }
            set
            {
                if (showPlayArea != value)
                {
                    showPlayArea = value;

                    PropertyAction(value, currentPlayAreaObject, () => GetPlayAreaVisualization());
                }
            }
        }

        private bool showTrackedArea = false;

        private int playAreaPhysicsLayer;

        /// <inheritdoc/>
        public int PlayAreaPhysicsLayer
        {
            get
            {
                if (currentPlayAreaObject != null)
                {
                    playAreaPhysicsLayer = currentPlayAreaObject.layer;
                }

                return playAreaPhysicsLayer;
            }
            set
            {
                playAreaPhysicsLayer = value;

                if (currentPlayAreaObject != null)
                {
                    currentPlayAreaObject.layer = playAreaPhysicsLayer;
                }
            }
        }

        /// <inheritdoc/>
        public bool ShowTrackedArea
        {
            get { return showTrackedArea; }
            set
            {
                if (showTrackedArea != value)
                {
                    showTrackedArea = value;

                    PropertyAction(value, currentTrackedAreaObject, () => GetTrackedAreaVisualization());
                }
            }
        }

        private bool showBoundaryWalls = false;

        private int trackedAreaPhysicsLayer;

        /// <inheritdoc/>
        public int TrackedAreaPhysicsLayer
        {
            get
            {
                if (currentTrackedAreaObject != null)
                {
                    trackedAreaPhysicsLayer = currentTrackedAreaObject.layer;
                }

                return trackedAreaPhysicsLayer;
            }
            set
            {
                trackedAreaPhysicsLayer = value;

                if (currentTrackedAreaObject != null)
                {
                    currentTrackedAreaObject.layer = trackedAreaPhysicsLayer;
                }
            }
        }

        /// <inheritdoc/>
        public bool ShowBoundaryWalls
        {
            get { return showBoundaryWalls; }
            set
            {
                if (showBoundaryWalls != value)
                {
                    showBoundaryWalls = value;

                    PropertyAction(value, currentBoundaryWallObject, () => GetBoundaryWallVisualization());
                }
            }
        }

        private bool showCeiling = false;

        private int boundaryWallsPhysicsLayer;

        /// <inheritdoc/>
        public int BoundaryWallsPhysicsLayer
        {
            get
            {
                if (currentBoundaryWallObject != null)
                {
                    boundaryWallsPhysicsLayer = currentBoundaryWallObject.layer;
                }

                return boundaryWallsPhysicsLayer;
            }
            set
            {
                boundaryWallsPhysicsLayer = value;

                if (currentBoundaryWallObject != null)
                {
                    currentBoundaryWallObject.layer = boundaryWallsPhysicsLayer;
                }
            }
        }

        /// <inheritdoc/>
        public bool ShowBoundaryCeiling
        {
            get { return showCeiling; }
            set
            {
                if (showCeiling != value)
                {
                    showCeiling = value;

                    PropertyAction(value, currentCeilingObject, () => GetBoundaryCeilingVisualization());
                }
            }
        }

        private int ceilingPhysicsLayer;

        /// <inheritdoc/>
        public int CeilingPhysicsLayer
        {
            get
            {
                if (currentCeilingObject != null)
                {
                    ceilingPhysicsLayer = currentCeilingObject.layer;
                }

                return ceilingPhysicsLayer;
            }
            set
            {
                ceilingPhysicsLayer = value;

                if (currentCeilingObject != null)
                {
                    currentFloorObject.layer = ceilingPhysicsLayer;
                }
            }
        }

        private void PropertyAction(bool value, GameObject boundaryObject, System.Action getVisualizationMethod, bool raiseEvent = true)
        {
            // If not done initializing, no need to raise the changed event or check the visualization.
            // These will both happen at the end of the initialization flow.
            if (!IsInitialized)
            {
                return;
            }

            if (value && (boundaryObject == null))
            {
                getVisualizationMethod();
            }

            if (boundaryObject != null)
            {
                boundaryObject.SetActive(value);
            }

            if (raiseEvent)
            {
                RaiseBoundaryVisualizationChanged();
            }
        }

        /// <summary>
        /// Refreshes the current boundary visualizations without raising changed events.
        /// Used during the initialization flow.
        /// </summary>
        private void RefreshVisualization()
        {
            PropertyAction(ShowFloor, currentFloorObject, () => GetFloorVisualization(), false);
            PropertyAction(ShowPlayArea, currentPlayAreaObject, () => GetPlayAreaVisualization(), false);
            PropertyAction(ShowTrackedArea, currentTrackedAreaObject, () => GetTrackedAreaVisualization(), false);
            PropertyAction(ShowBoundaryWalls, currentBoundaryWallObject, () => GetBoundaryWallVisualization(), false);
            PropertyAction(ShowBoundaryCeiling, currentCeilingObject, () => GetBoundaryCeilingVisualization(), false);
        }

        /// <inheritdoc/>
        public Edge[] Bounds { get; protected set; } = System.Array.Empty<Edge>();

        /// <inheritdoc/>
        public float? FloorHeight { get; protected set; } = null;

        /// <inheritdoc/>
        public bool Contains(Vector3 location, BoundaryType boundaryType = BoundaryType.TrackedArea)
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
            location = MixedRealityPlayspace.InverseTransformPoint(location);

            if (FloorHeight.Value > location.y ||
                BoundaryHeight < location.y)
            {
                // Location below the floor or above the boundary height.
                return false;
            }

            // Boundary coordinates are always "on the floor"
            Vector2 point = new Vector2(location.x, location.z);

            if (boundaryType == BoundaryType.PlayArea)
            {
                // Check the inscribed rectangle.
                if (RectangularBounds != null)
                {
                    return RectangularBounds.IsInsideBoundary(point);
                }
            }
            else if (boundaryType == BoundaryType.TrackedArea)
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
            if (RectangularBounds == null || !RectangularBounds.IsValid)
            {
                center = EdgeUtilities.InvalidPoint;
                angle = 0f;
                width = 0f;
                height = 0f;
                return false;
            }

            // Handle the user teleporting (boundary moves with them).
            Vector3 transformedCenter = MixedRealityPlayspace.TransformPoint(
                new Vector3(RectangularBounds.Center.x, 0f, RectangularBounds.Center.y));

            center = new Vector2(transformedCenter.x, transformedCenter.z);
            angle = RectangularBounds.Angle;
            width = RectangularBounds.Width;
            height = RectangularBounds.Height;
            return true;
        }

        /// <inheritdoc/>
        public GameObject GetFloorVisualization()
        {
            if (!Application.isPlaying) { return null; }

            if (currentFloorObject != null)
            {
                return currentFloorObject;
            }

            MixedRealityBoundaryVisualizationProfile profile = ConfigurationProfile as MixedRealityBoundaryVisualizationProfile;
            if (profile == null) { return null; }

            if (!FloorHeight.HasValue)
            {
                // We were unable to locate the floor.
                return null;
            }

            Vector2 floorScale = profile.FloorScale;

            // Render the floor.
            currentFloorObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            currentFloorObject.name = "Boundary System Floor";
            currentFloorObject.transform.localScale = new Vector3(floorScale.x, boundaryObjectThickness, floorScale.y);
            currentFloorObject.transform.Translate(new Vector3(
                MixedRealityPlayspace.Position.x,
                FloorHeight.Value - (currentFloorObject.transform.localScale.y * 0.5f),
                MixedRealityPlayspace.Position.z));
            currentFloorObject.layer = FloorPhysicsLayer;
            currentFloorObject.GetComponent<Renderer>().sharedMaterial = profile.FloorMaterial;

            return currentFloorObject;
        }

        /// <inheritdoc/>
        public GameObject GetPlayAreaVisualization()
        {
            if (!Application.isPlaying) { return null; }

            if (currentPlayAreaObject != null)
            {
                return currentPlayAreaObject;
            }

            MixedRealityBoundaryVisualizationProfile profile = ConfigurationProfile as MixedRealityBoundaryVisualizationProfile;
            if (profile == null) { return null; }

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
            if (!EdgeUtilities.IsValidPoint(center))
            {
                // Invalid rectangle / play area not found
                return null;
            }

            currentPlayAreaObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            currentPlayAreaObject.name = "Play Area";
            currentPlayAreaObject.layer = PlayAreaPhysicsLayer;
            currentPlayAreaObject.transform.Translate(new Vector3(center.x, boundaryObjectRenderOffset, center.y));
            currentPlayAreaObject.transform.Rotate(new Vector3(90, -angle, 0));
            currentPlayAreaObject.transform.localScale = new Vector3(width, height, 1.0f);
            currentPlayAreaObject.GetComponent<Renderer>().sharedMaterial = profile.PlayAreaMaterial;

            currentPlayAreaObject.transform.parent = BoundaryVisualizationParent.transform;

            return currentPlayAreaObject;
        }

        /// <inheritdoc/>
        public GameObject GetTrackedAreaVisualization()
        {
            if (!Application.isPlaying) { return null; }

            if (currentTrackedAreaObject != null)
            {
                return currentTrackedAreaObject;
            }

            MixedRealityBoundaryVisualizationProfile profile = ConfigurationProfile as MixedRealityBoundaryVisualizationProfile;
            if (profile == null) { return null; }

            if (Bounds.Length == 0)
            {
                // If we do not have boundary edges, we cannot render them.
                return null;
            }

            // Get the line vertices
            List<Vector3> lineVertices = new List<Vector3>();
            for (int i = 0; i < Bounds.Length; i++)
            {
                lineVertices.Add(new Vector3(Bounds[i].PointA.x, 0f, Bounds[i].PointA.y));
            }
            // Add the first vertex again to ensure the loop closes.
            lineVertices.Add(lineVertices[0]);

            // We use an empty object and attach a line renderer.
            currentTrackedAreaObject = new GameObject("Tracked Area")
            {
                layer = ignoreRaycastLayerValue
            };
            currentTrackedAreaObject.AddComponent<LineRenderer>();
            currentTrackedAreaObject.transform.Translate(new Vector3(
                MixedRealityPlayspace.Position.x,
                boundaryObjectRenderOffset,
                MixedRealityPlayspace.Position.z));
            currentPlayAreaObject.layer = TrackedAreaPhysicsLayer;

            // Configure the renderer properties.
            float lineWidth = 0.01f;
            LineRenderer lineRenderer = currentTrackedAreaObject.GetComponent<LineRenderer>();
            lineRenderer.sharedMaterial = profile.TrackedAreaMaterial;
            lineRenderer.useWorldSpace = false;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.positionCount = lineVertices.Count;
            lineRenderer.SetPositions(lineVertices.ToArray());

            currentTrackedAreaObject.transform.parent = BoundaryVisualizationParent.transform;

            return currentTrackedAreaObject;
        }

        /// <inheritdoc/>
        public GameObject GetBoundaryWallVisualization()
        {
            if (!Application.isPlaying) { return null; }

            if (currentBoundaryWallObject != null)
            {
                return currentBoundaryWallObject;
            }

            MixedRealityBoundaryVisualizationProfile profile = ConfigurationProfile as MixedRealityBoundaryVisualizationProfile;
            if (profile == null) { return null; }

            if (!FloorHeight.HasValue)
            {
                // We need a floor on which to place the walls.
                return null;
            }

            if (Bounds.Length == 0)
            {
                // If we do not have boundary edges, we cannot render walls.
                return null;
            }

            currentBoundaryWallObject = new GameObject("Tracked Area Walls")
            {
                layer = BoundaryWallsPhysicsLayer
            };

            // Create and parent the child objects
            float wallDepth = boundaryObjectThickness;
            for (int i = 0; i < Bounds.Length; i++)
            {
                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = $"Wall {i}";
                wall.GetComponent<Renderer>().sharedMaterial = profile.BoundaryWallMaterial;
                wall.transform.localScale = new Vector3((Bounds[i].PointB - Bounds[i].PointA).magnitude, BoundaryHeight, wallDepth);
                wall.layer = ignoreRaycastLayerValue;

                // Position and rotate the wall.
                Vector2 mid = Vector2.Lerp(Bounds[i].PointA, Bounds[i].PointB, 0.5f);
                wall.transform.position = new Vector3(mid.x, (BoundaryHeight * 0.5f), mid.y);
                float rotationAngle = MathUtilities.GetAngleBetween(Bounds[i].PointB, Bounds[i].PointA);
                wall.transform.rotation = Quaternion.Euler(0.0f, -rotationAngle, 0.0f);

                wall.transform.parent = currentBoundaryWallObject.transform;
            }

            currentBoundaryWallObject.transform.parent = BoundaryVisualizationParent.transform;

            return currentBoundaryWallObject;
        }

        /// <inheritdoc/>
        public GameObject GetBoundaryCeilingVisualization()
        {
            if (!Application.isPlaying) { return null; }

            if (currentCeilingObject != null)
            {
                return currentCeilingObject;
            }

            MixedRealityBoundaryVisualizationProfile profile = ConfigurationProfile as MixedRealityBoundaryVisualizationProfile;
            if (profile == null) { return null; }

            if (Bounds.Length == 0)
            {
                // If we do not have boundary edges, we cannot render a ceiling.
                return null;
            }

            // Get the smallest rectangle that contains the entire boundary.
            Bounds boundaryBoundingBox = new Bounds();
            for (int i = 0; i < Bounds.Length; i++)
            {
                // The boundary geometry is a closed loop. As such, we can encapsulate only PointA of each Edge.
                boundaryBoundingBox.Encapsulate(new Vector3(Bounds[i].PointA.x, BoundaryHeight * 0.5f, Bounds[i].PointA.y));
            }

            // Render the ceiling.
            float ceilingDepth = boundaryObjectThickness;
            currentCeilingObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            currentCeilingObject.name = "Ceiling";
            currentCeilingObject.layer = ignoreRaycastLayerValue;
            currentCeilingObject.transform.localScale = new Vector3(boundaryBoundingBox.size.x, ceilingDepth, boundaryBoundingBox.size.z);
            currentCeilingObject.transform.Translate(new Vector3(
                boundaryBoundingBox.center.x,
                BoundaryHeight + (currentCeilingObject.transform.localScale.y * 0.5f),
                boundaryBoundingBox.center.z));
            currentCeilingObject.GetComponent<Renderer>().sharedMaterial = profile.BoundaryCeilingMaterial;
            currentCeilingObject.layer = CeilingPhysicsLayer;
            currentCeilingObject.transform.parent = BoundaryVisualizationParent.transform;

            return currentCeilingObject;
        }

        #endregion IMixedRealityBoundarySystem Implementation

        /// <summary>
        /// The largest rectangle that is contained withing the play space geometry.
        /// </summary>
        protected InscribedRectangle RectangularBounds = null;

        private GameObject currentFloorObject = null;
        private GameObject currentPlayAreaObject = null;
        private GameObject currentTrackedAreaObject = null;
        private GameObject currentBoundaryWallObject = null;
        private GameObject currentCeilingObject = null;

        /// <summary>
        /// Retrieves the boundary geometry.
        /// </summary>
        /// <returns>A list of geometry points, or null if geometry was unavailable.</returns>
        protected abstract List<Vector3> GetBoundaryGeometry();

        /// <summary>
        /// Updates the tracking space on the XR device.
        /// </summary>
        protected abstract void SetTrackingSpace();

        /// <summary>
        /// Retrieves the boundary geometry and creates the boundary and inscribed play space volumes.
        /// </summary>
        private void CalculateBoundaryBounds()
        {
            // Reset the bounds
            Bounds = System.Array.Empty<Edge>();
            FloorHeight = null;
            RectangularBounds = null;

            // Get the boundary geometry.
            var boundaryGeometry = GetBoundaryGeometry();

            if (boundaryGeometry != null && boundaryGeometry.Count > 0)
            {
                // Get the boundary geometry.
                var boundaryEdges = new List<Edge>(0);

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
            RectangularBounds = new InscribedRectangle(Bounds, Mathf.Abs("Mixed Reality Toolkit".GetHashCode()));
        }
    }
}
