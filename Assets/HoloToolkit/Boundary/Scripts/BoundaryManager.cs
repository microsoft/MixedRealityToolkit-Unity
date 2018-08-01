// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_2017_2_OR_NEWER
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
#endif

namespace HoloToolkit.Unity.Boundary
{
    /// <summary>
    /// Places a floor quad to ground the scene.
    /// Allows you to check if your GameObject is within setup boundary on the immersive headset.
    /// </summary>
    public class BoundaryManager : Singleton<BoundaryManager>
    {
#if UNITY_2017_2_OR_NEWER
        [Tooltip("Quad prefab to display as the floor.")]
        public GameObject FloorQuad = null;
        private GameObject floorQuadInstance = null;

        [SerializeField]
        [Tooltip("Approximate max Y height of your space.")]
        private float boundaryHeight = 10f;

        // Minimum boundary Y value
        private float boundaryFloor = 0.0f;

        private InscribedRectangle inscribedRectangle;

        private Edge[] boundaryGeometryEdges = new Edge[0];

        [SerializeField]
        // Defaulting coordinate system to RoomScale in immersive headsets.
        // This puts the origin (0, 0, 0) on the floor if a floor has been established during setup via MixedRealityPortal.
        private TrackingSpaceType opaqueTrackingSpaceType = TrackingSpaceType.RoomScale;

        // Removed for now, until the HoloLens tracking space type story is more clear.
        //[SerializeField]
        // Defaulting coordinate system to Stationary for transparent headsets, like HoloLens.
        // This puts the origin (0, 0, 0) at the first place where the user started the application.
        //private TrackingSpaceType transparentTrackingSpaceType = TrackingSpaceType.Stationary;

        // Testing in the editor found that this moved the floor out of the way enough, and it is only
        // used in the case where a headset isn't attached. Otherwise, the floor is positioned like normal.
        private readonly Vector3 floorPositionInEditor = new Vector3(0f, -3f, 0f);

        [SerializeField]
        private bool renderFloor = true;
        public bool RenderFloor
        {
            get { return renderFloor; }
            set
            {
                if (renderFloor != value)
                {
                    renderFloor = value;
                    SetFloorRendering();
                }
            }
        }

        [SerializeField]
        private bool renderBoundary = true;
        public bool RenderBoundary
        {
            get { return renderBoundary; }
            set
            {
                if (renderBoundary != value)
                {
                    renderBoundary = value;
                    SetBoundaryRendering();
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

#if UNITY_WSA
            bool isDisplayOpaque = UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque;
#else
            // Assume displays on non Windows MR platforms are all opaque.
            // This will likely change as new hardware comes to market.
            bool isDisplayOpaque = true;
#endif

            if (isDisplayOpaque && XRSettings.enabled)
            {
                XRDevice.SetTrackingSpaceType(opaqueTrackingSpaceType);
            }
            else
            {
                // Removed for now, until the HoloLens tracking space type story is more clear.
                //XRDevice.SetTrackingSpaceType(transparentTrackingSpaceType);

                Destroy(this);
                return;
            }

            if (XRDevice.GetTrackingSpaceType() == TrackingSpaceType.RoomScale)
            {
                // Render the floor if you are in editor or a room scale device.
                RenderFloorQuad();
            }

            // Render boundary if configured.
            SetBoundaryRendering();

            // Create a volume out of the specified user boundary.
            CalculateBoundaryVolume();
        }

        private void SetFloorRendering()
        {
            if (floorQuadInstance != null)
            {
                floorQuadInstance.SetActive(renderFloor);
            }
        }

        private void SetBoundaryRendering()
        {
            // This always returns false in WindowsMR.
            if (UnityEngine.Experimental.XR.Boundary.configured)
            {
                UnityEngine.Experimental.XR.Boundary.visible = renderBoundary;
            }
        }

        private void RenderFloorQuad()
        {
            if (FloorQuad != null)
            {
                floorQuadInstance = Instantiate(FloorQuad);

                if (!XRDevice.isPresent)
                {
                    // So the floor quad does not occlude in editor testing, draw it lower.
                    floorQuadInstance.transform.position = floorPositionInEditor;
                }
                else
                {
                    floorQuadInstance.transform.position = Vector3.zero;
                }

                SetFloorRendering();
            }
        }

        /// <summary>
        /// Pass in the game object's position to check if
        /// it's within the tracked area boundary space.
        /// </summary>
        /// <param name="gameObjectPosition">The position of the GameObject to check.</param>
        /// <returns>True if the point is in the tracked area boundary space.</returns>
        public bool ContainsObject(Vector3 gameObjectPosition)
        {
            return ContainsObject(gameObjectPosition, UnityEngine.Experimental.XR.Boundary.Type.TrackedArea);
        }

        /// <summary>
        /// Pass in the game object's position to check if it's within 
        /// the specified boundary space.
        /// </summary>
        /// <param name="gameObjectPosition">The position of the GameObject to check.</param>
        /// <param name="boundaryType">The type of the boundary. Use PlayArea for the inscribed rectangle or TrackedArea for the bounds containing the whole space.</param>
        /// <returns>True if the point is in the boundary type's bounds.</returns>
        public bool ContainsObject(Vector3 gameObjectPosition, UnityEngine.Experimental.XR.Boundary.Type boundaryType)
        {
            gameObjectPosition = CameraCache.Main.transform.parent.InverseTransformPoint(gameObjectPosition);

            if (gameObjectPosition.y < boundaryFloor || gameObjectPosition.y > boundaryHeight)
            {
                return false;
            }

            if (boundaryType == UnityEngine.Experimental.XR.Boundary.Type.PlayArea)
            {
                if (inscribedRectangle == null || !inscribedRectangle.IsRectangleValid)
                {
                    return false;
                }

                return inscribedRectangle.IsPointInRectangleBounds(new Vector2(gameObjectPosition.x, gameObjectPosition.z));
            }
            else if (boundaryType == UnityEngine.Experimental.XR.Boundary.Type.TrackedArea)
            {
                // Check if the supplied game object's position is within the bounds volume.
                return EdgeHelpers.IsInside(boundaryGeometryEdges, new Vector2(gameObjectPosition.x, gameObjectPosition.z));
            }

            return false;
        }

        /// <summary>
        /// Returns the corner points of a 2D rectangle that is the
        /// largest rectangle we could find within the geometry of
        /// the space bounds.
        /// </summary>
        /// <returns>Array of 3D points, all with the same y value.</returns>
        public Vector3[] TryGetBoundaryRectanglePoints()
        {
            if (inscribedRectangle == null || !inscribedRectangle.IsRectangleValid)
            {
                return null;
            }

            var points2d = inscribedRectangle.GetRectanglePoints();

            var positions = new Vector3[points2d.Length];
            for (int i = 0; i < points2d.Length; ++i)
            {
                positions[i] = CameraCache.Main.transform.parent.TransformPoint(new Vector3(points2d[i].x, boundaryFloor, points2d[i].y));
            }
            return positions;
        }

        /// <summary>
        /// Returns parameters describing the boundary rectangle.
        /// </summary>
        internal bool TryGetBoundaryRectangleParams(out Vector3 center, out float angle, out float width, out float height)
        {
            if (inscribedRectangle == null || !inscribedRectangle.IsRectangleValid)
            {
                center = Vector3.zero;
                angle = width = height = 0.0f;
                return false;
            }

            Vector2 center2D;
            inscribedRectangle.GetRectangleParams(out center2D, out angle, out width, out height);
            center = CameraCache.Main.transform.parent.TransformPoint(new Vector3(center2D.x, boundaryFloor, center2D.y));
            return true;
        }

        /// <summary>
        /// Uses the TryGetGeometry call and Unity Bounds to create a volume out of the setup boundary.
        /// </summary>
        public void CalculateBoundaryVolume()
        {
#if !UNITY_WSA
            // This always returns false in WindowsMR.
            if (!UnityEngine.Experimental.XR.Boundary.configured)
            {
                Debug.Log("Boundary not configured.");
                return;
            }
#endif

            if (XRDevice.GetTrackingSpaceType() != TrackingSpaceType.RoomScale)
            {
                Debug.Log("No boundary for non-room scale experiences.");
                return;
            }

            // Get all the bounds setup by the user.
            var boundaryGeometryPoints = new List<Vector3>(0);
            if (UnityEngine.Experimental.XR.Boundary.TryGetGeometry(boundaryGeometryPoints, UnityEngine.Experimental.XR.Boundary.Type.TrackedArea))
            {
                if (boundaryGeometryPoints.Count > 0)
                {
                    for (int pointIndex = 0; pointIndex < boundaryGeometryPoints.Count; pointIndex++)
                    {
                        boundaryFloor = Math.Min(boundaryFloor, boundaryGeometryPoints[pointIndex].y);
                    }

                    boundaryGeometryEdges = EdgeHelpers.ConvertVector3ListToEdgeArray(boundaryGeometryPoints);

                    inscribedRectangle = new InscribedRectangle(boundaryGeometryEdges);
                }
            }
            else
            {
                Debug.Log("TryGetGeometry returned false.");
            }
        }
#endif
    }
}
