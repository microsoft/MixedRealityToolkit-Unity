// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.XR;

namespace MixedRealityToolkit.Boundary
{
    /// <summary>
    /// Places a floor quad to ground the scene.
    /// Allows you to check if your GameObject is within setup boundary on the immersive headset.
    /// </summary>
    public class BoundaryManager : Singleton<BoundaryManager>
    {
        [Tooltip("Quad prefab to display as the floor.")]
        public GameObject FloorQuad;
        private GameObject floorQuadInstance;

        [SerializeField]
        [Tooltip("Approximate max Y height of your space.")]
        private float boundaryHeight = 10f;

        // Minimum boundary Y value
        private float boundaryFloor = 0.0f;

        private InscribedRectangle inscribedRectangle;

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
            // This will likely change as new hardwre comes to market.
            bool isDisplayOpaque = true;
#endif

            if (isDisplayOpaque)
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

            // Render the floor based on if you are in editor or immersive device.
            RenderFloorQuad();

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
            // TODO: BUG: Unity: configured bool always returns false in 2017.2.0p2-MRTP5.
            if (UnityEngine.Experimental.XR.Boundary.configured)
            {
                UnityEngine.Experimental.XR.Boundary.visible = renderBoundary;
            }
        }

        private void RenderFloorQuad()
        {
            if (FloorQuad != null && XRDevice.GetTrackingSpaceType() == TrackingSpaceType.RoomScale)
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
        /// Pass in the game object's position to check if it's within 
        /// the specified boundary space.
        /// </summary>
        /// <param name="gameObjectPosition"></param>
        /// <returns>True if the point is in the cuboid bounds</returns>
        public bool ContainsObject(Vector3 gameObjectPosition)
        {
            if(gameObjectPosition.y < this.boundaryFloor || gameObjectPosition.y > this.boundaryHeight)
            {
                return false;
            }

            if(this.inscribedRectangle == null || !this.inscribedRectangle.IsRectangleValid)
            {
                return false;
            }

            return this.inscribedRectangle.IsPointInRectangleBounds(new Vector2(gameObjectPosition.x, gameObjectPosition.z));
        }

        /// <summary>
        /// Returns the corner points of a 2D rectangle that is the
        /// largest rectangle we could find within the geometry of
        /// the space bounds.
        /// </summary>
        /// <returns>Array of 3D points, all with the same y value</returns>
        public Vector3[] TryGetBoundaryRectanglePoints()
        {
            if(this.inscribedRectangle == null || !this.inscribedRectangle.IsRectangleValid)
            {
                return null;
            }

            var points2d = this.inscribedRectangle.GetRectanglePoints();

            var positions = new Vector3[points2d.Length];
            for (int i = 0; i < points2d.Length; ++i)
            {
                positions[i] = new Vector3(points2d[i].x, this.boundaryFloor, points2d[i].y);
            }
            return positions;
        }

        /// <summary>
        /// Returns parameters describing the boundary rectangle
        /// </summary>
        internal bool TryGetBoundaryRectangleParams(out Vector3 center, out float angle, out float width, out float height)
        {
            if(this.inscribedRectangle == null || !this.inscribedRectangle.IsRectangleValid)
            {
                center = Vector3.zero;
                angle = width = height = 0.0f;
                return false;
            }

            Vector2 center2D;
            this.inscribedRectangle.GetRectangleParams(out center2D, out angle, out width, out height);
            center = new Vector3(center2D.x, this.boundaryFloor, center2D.y);
            return true;
        }

        /// <summary>
        /// Uses the TryGetGeometry call and Unity Bounds to create a volume out of the setup boundary.
        /// </summary>
        public void CalculateBoundaryVolume()
        {
            if (!UnityEngine.Experimental.XR.Boundary.configured)
            {
                Debug.Log("Boundary not configured.");
                // TODO: BUG: Unity: Should return true if a floor and boundary has been established by user.
                // But this always returns false with in 2017.2.0p2-MRTP5.
                //return;
            }

            if (XRDevice.GetTrackingSpaceType() != TrackingSpaceType.RoomScale)
            {
                Debug.Log("No boundary for non-room scale experiences.");
                return;
            }

            // Get all the bounds setup by the user.
            var boundaryGeometry = new List<Vector3>(0);
            if (UnityEngine.Experimental.XR.Boundary.TryGetGeometry(boundaryGeometry, UnityEngine.Experimental.XR.Boundary.Type.TrackedArea))
            {
                if (boundaryGeometry.Count > 0)
                {
                    // Create a UnityEngine.Bounds volume with those values.
                    foreach (Vector3 boundaryGeo in boundaryGeometry)
                    {
                        this.boundaryFloor = Math.Min(this.boundaryFloor, boundaryGeo.y);
                    }

                    this.inscribedRectangle = new InscribedRectangle(boundaryGeometry);
                }
            }
            else
            {
                Debug.Log("TryGetGeometry returned false.");
            }
        }
    }
}
