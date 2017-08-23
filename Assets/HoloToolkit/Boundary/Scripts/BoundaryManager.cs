using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.Experimental.XR;

namespace HoloToolkit.Unity.Boundary
{
    /// <summary>
    /// Places a floor quad to ground the scene.
    /// Allows you to check if your gameobject is within setup boundary on the immersive headset.
    /// </summary>
    public class BoundaryManager : SingleInstance<BoundaryManager>
    {
        [Tooltip("Quad prefab to display as the floor.")]
        public GameObject FloorQuad;
        private GameObject floorQuadInstance;

        [SerializeField]
        [Tooltip("Approximate max Y height of your space.")]
        private float boundaryHeight = 10f;        
        private Bounds boundaryBounds = new Bounds();

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
            // TODO: BUG: Unity: configured bool always returns false.
            if (UnityEngine.Experimental.XR.Boundary.configured)
            {
                UnityEngine.Experimental.XR.Boundary.visible = renderBoundary;
            }
        }

        void Start()
        {
            // Render boundary if configured.
            SetBoundaryRendering();

            // Render the floor based on if you are in editor or immersive device.
            RenderFloorQuad();
        }

        private void RenderFloorQuad()
        {
            if (FloorQuad != null && HolographicSettings.IsDisplayOpaque)
            {
                floorQuadInstance = Instantiate(FloorQuad);
                floorQuadInstance.transform.SetParent(gameObject.transform.parent);

#if UNITY_EDITOR
                // So the floor quad does not occlude in editor testing, draw it lower.
                floorQuadInstance.transform.localPosition = new Vector3(0, -3, 0);
#else
                // Inside immersive headset draw floor quad at Y value of dimensions.
                Vector3 dimensions;
                // TODO: BUG: Unity: configured bool always returns false.
                // TODO: BUG: Unity: TryGetDimensions does not return true either.
                //if (UnityEngine.Experimental.XR.Boundary.configured &&
                //    UnityEngine.Experimental.XR.Boundary.TryGetDimensions(out dimensions))

                if (UnityEngine.Experimental.XR.Boundary.TryGetDimensions(out dimensions))
                {
                    if (dimensions != null)
                    {
                        Debug.Log("Drawing floor at dimensions Y.");
                        // Draw the floor at boundry Y.                    
                        floorQuadInstance.transform.localPosition = new Vector3(0, dimensions.y, 0);
                    }
                }
                else
                {
                    Debug.Log("Drawing floor at 0,0,0.");
                    // Draw the floor at 0,0,0.
                    floorQuadInstance.transform.localPosition = Vector3.zero;
                }
#endif
                floorQuadInstance.SetActive(true);
            }
        }

        /// <summary>
        /// Pass in the game object's position to check if it's within 
        /// the specified boundary space.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public bool ContainsObject(Vector3 gameObjectPosition)
        {
            // TODO: BUG: Unity
            //if (!UnityEngine.Experimental.XR.Boundary.configured)
            //{
            //    Debug.Log("Boundary not configured.");
            //    return;
            //}

            // Get all the bounds setup by the user.
            List<Vector3> boundaryGeometry = new List<Vector3>();
            if (UnityEngine.Experimental.XR.Boundary.TryGetGeometry(boundaryGeometry))
            {
                if (boundaryGeometry != null)
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

            // Check if the supplied game object's position is within the bounds volume.
            if (boundaryBounds != null && HolographicSettings.IsDisplayOpaque)
            {
                return boundaryBounds.Contains(gameObjectPosition);
            }

            return false;
        }
    }
}