// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using System.Collections.Generic;
using UnityEngine.XR.WSA;
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
        [Tooltip("Quad prefab to display as the floor.")]
        public GameObject FloorQuad;
        private GameObject floorQuadInstance;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        [SerializeField]
        [Tooltip("Approximate max Y height of your space.")]
        private float boundaryHeight = 10f;
        private Bounds boundaryBounds;
#endif

        private bool renderFloor = true;
        public bool RenderFloor
        {
            get
            {
                return renderFloor;
            }
            set
            {
                if (renderFloor != value)
                {
                    renderFloor = value;
                    SetFloorRendering();
                }
            }
        }

        private void SetFloorRendering()
        {
            if (floorQuadInstance != null)
            {
                floorQuadInstance.SetActive(renderFloor);
            }
        }

        private bool renderBoundary = true;
        public bool RenderBoundary
        {
            get
            {
                return renderBoundary;
            }
            set
            {
                if (renderBoundary != value)
                {
                    renderBoundary = value;
                    SetBoundaryRendering();
                }
            }
        }

        private void SetBoundaryRendering()
        {
#if UNITY_2017_2_OR_NEWER
            // TODO: BUG: Unity: configured bool always returns false.
            if (UnityEngine.Experimental.XR.Boundary.configured)
            {
                UnityEngine.Experimental.XR.Boundary.visible = renderBoundary;
            }
#endif
        }

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        protected override void Awake()
        {
            base.Awake();

            // Render the floor based on if you are in editor or immersive device.
            RenderFloorQuad();

            // Render boundary if configured.
            SetBoundaryRendering();

            // Create a volume out of the specified user boundary.
            CalculateBoundaryVolume();
        }

        private void RenderFloorQuad()
        {
            if (HolographicSettings.IsDisplayOpaque)
            {
                // Defaulting coordinate system to RoomScale in immersive headsets.
                // This puts the origin 0,0,0 on the floor if a floor has been established during RunSetup via MixedRealityPortal
                XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
            }
            else
            {
                // Defaulting coordinate system to Stationary for HoloLens.
                // This puts the origin 0,0,0 at the first place where the user started the application.
                XRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);
            }

            if (FloorQuad != null && HolographicSettings.IsDisplayOpaque)
            {
                floorQuadInstance = Instantiate(FloorQuad);

                if (!XRDevice.isPresent)
                {
                    // So the floor quad does not occlude in editor testing, draw it lower.
                    floorQuadInstance.transform.localPosition = new Vector3(0, -3, 0);
                }
                else
                {
                    // Inside immersive headset draw floor quad at Y value of dimensions.
                    Vector3 dimensions;
                    // TODO: BUG: Unity: TryGetDimensions does not return true either.
                    //if (UnityEngine.Experimental.XR.Boundary.TryGetDimensions(out dimensions,
                    //UnityEngine.Experimental.XR.Boundary.Type.TrackedArea))
                    if (UnityEngine.Experimental.XR.Boundary.TryGetDimensions(out dimensions,
                        UnityEngine.Experimental.XR.Boundary.Type.TrackedArea))
                    {
                        Debug.Log("Got dimensions of tracked area.  Drawing floor at height offset: " + dimensions.y);
                        floorQuadInstance.transform.localPosition = new Vector3(0, dimensions.y, 0);
                    }
                    else
                    {
                        Debug.Log("Drawing floor at 0,0,0.");
                        floorQuadInstance.transform.localPosition = Vector3.zero;
                    }
                }
                floorQuadInstance.SetActive(true);
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
            if (HolographicSettings.IsDisplayOpaque)
            {
                return boundaryBounds.Contains(gameObjectPosition);
            }

            return false;
        }

        /// <summary>
        /// Uses the TryGetGeometry call and Unity Bounds to create a volume out of the setup boundary.
        /// </summary>
        public void CalculateBoundaryVolume()
        {
            // TODO: BUG: Unity: Should return true if a floor and boundary has been established by user.
            // But this always returns false with b8.
            //if (!UnityEngine.Experimental.XR.Boundary.configured)
            //{
            //    Debug.Log("Boundary not configured.");
            //    return;
            //}

            if (XRDevice.GetTrackingSpaceType() != TrackingSpaceType.RoomScale)
            {
                Debug.Log("No boundary for stationary scale experiences.");
                return;
            }

            boundaryBounds = new Bounds();
            // Get all the bounds setup by the user.
            var boundaryGeometry = new List<Vector3>(0);
            if (UnityEngine.Experimental.XR.Boundary.TryGetGeometry(boundaryGeometry))
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
