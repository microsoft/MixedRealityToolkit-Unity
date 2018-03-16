// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.WSA;
#endif

namespace MixedRealityToolkit.Boundary
{
    /// <summary>
    /// Places a floor quad to ground the scene.
    /// Allows you to check if your GameObject is within setup boundary on the immersive headset.
    /// </summary>
    public class BoundaryManager : Singleton<BoundaryManager>
    {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        [Tooltip("Quad prefab to display as the floor.")]
        public GameObject FloorQuad = null;
        private GameObject floorQuadInstance = null;

        [SerializeField]
        [Tooltip("Approximate max Y height of your space.")]
        private float boundaryHeight = 10f;

        private Bounds boundaryBounds;

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
#endif

        protected override void Awake()
        {
            base.Awake();

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (HolographicSettings.IsDisplayOpaque)
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
#endif
        }

        private void SetBoundaryRendering()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            // TODO: BUG: Unity: configured bool always returns false in 2017.2.0p2-MRTP5.
            if (UnityEngine.Experimental.XR.Boundary.configured)
            {
                UnityEngine.Experimental.XR.Boundary.visible = renderBoundary;
            }
#endif
        }

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
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
        /// <returns></returns>
        public bool ContainsObject(Vector3 gameObjectPosition)
        {
            // Check if the supplied game object's position is within the bounds volume.
            return boundaryBounds.Contains(gameObjectPosition);
        }

        /// <summary>
        /// Uses the TryGetGeometry call and Unity Bounds to create a volume out of the setup boundary.
        /// </summary>
        public void CalculateBoundaryVolume()
        {
            // TODO: BUG: Unity: Should return true if a floor and boundary has been established by user.
            // But this always returns false with in 2017.2.0p2-MRTP5.
            //if (!UnityEngine.Experimental.XR.Boundary.configured)
            //{
            //    Debug.Log("Boundary not configured.");
            //    return;
            //}

            if (XRDevice.GetTrackingSpaceType() != TrackingSpaceType.RoomScale)
            {
                Debug.Log("No boundary for non-room scale experiences.");
                return;
            }

            boundaryBounds = new Bounds();

            // Get all the bounds setup by the user.
            var boundaryGeometry = new List<Vector3>(0);

            if (UnityEngine.Experimental.XR.Boundary.TryGetGeometry(boundaryGeometry, UnityEngine.Experimental.XR.Boundary.Type.TrackedArea))
            {
                if (boundaryGeometry.Count > 0)
                {
                    // Create a UnityEngine.Bounds volume with those values.
                    foreach (Vector3 boundaryGeo in boundaryGeometry)
                    {
                        boundaryBounds.Encapsulate(boundaryGeo);
                    }
                }
            }

            // Ensuring that we set height of the bounds volume to be say 10 feet tall.
            boundaryBounds.Encapsulate(new Vector3(0, boundaryHeight, 0));
        }
#endif
    }
}
