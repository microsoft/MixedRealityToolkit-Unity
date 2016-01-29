using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// CursorManager takes GameObjects for cursors:
    /// One that is visible when the user gazes at a hologram,
    /// and another that is visible when the user is not gazing
    /// at a hologram.  CursorManager then determines which cursor
    /// should be visible based on the user's gaze and places this
    /// cursor at the intersection of the gaze view vector and any
    /// holograms in the scene. This class also orients the cursor
    /// to match the normal of this hologram.
    /// </summary>
    public class CursorManager : Singleton<CursorManager>
    {
        [Tooltip("The cursor GameObject to show when the user's gaze intersects with a hologram.")]
        public GameObject CursorOnHolograms;

        [Tooltip("The cursor GameObject to show when the user's gaze does not hit a hologram.")]
        public GameObject CursorOffHolograms;

        [Tooltip("Distance, in meters, to offset the cursor from the collision point.")]
        public float DistanceFromCollision = 0.01f;

        private Quaternion cursorOnHologramsDefaultRotation;
        private Quaternion cursorOffHologramsDefaultRotation;
        
        void Awake()
        {
            if (CursorOnHolograms == null || CursorOffHolograms == null)
            {
                return;
            }

            if (GazeManager.Instance == null)
            {
                Debug.LogError("Must have a GazeManager in the scene.");
            }

            if ((GazeManager.Instance.RaycastLayerMask & CursorOnHolograms.layer) == 0 ||
                (GazeManager.Instance.RaycastLayerMask & CursorOffHolograms.layer) == 0)
            {
                Debug.LogError("One or both of the cursors have layers that are checked in the GazeManager's Raycast Layer Mask.  Please change the cursor layers (eg: to Ignore Raycast) or uncheck these layers in GazeManager: " + 
                    LayerMask.LayerToName(CursorOnHolograms.layer) + ", " + LayerMask.LayerToName(CursorOffHolograms.layer));
            }

            // Hide the Cursors to begin with.
            CursorOnHolograms.SetActive(false);
            CursorOffHolograms.SetActive(false);
            
            // Cache the cursor default rotations so the cursors can be rotated with respect to the original orientation.
            cursorOnHologramsDefaultRotation = CursorOnHolograms.transform.rotation;
            cursorOffHologramsDefaultRotation = CursorOffHolograms.transform.rotation;
        }

        void Update()
        {
            if (GazeManager.Instance == null || CursorOnHolograms == null || CursorOffHolograms == null)
            {
                return;
            }
            
            // Ensure that the cursor to display is set to active at the end of each block below, so that
            // the visible cursor will always be shown, even in the case where CursorOffHolograms is the 
            // same GameObject as CursorOnHolograms.
            if (GazeManager.Instance.Hit)
            {
                // If the user's gaze vector intersects with a hologram,
                // hide the CursorOffHolograms and show the CursorOnHolograms.
                CursorOffHolograms.SetActive(false);
                CursorOnHolograms.SetActive(true);
            }
            else
            {
                // If the user's gaze vector does not intersect with a hologram,
                // hide the CursorOnHolograms and show the CursorOffHolograms.
                CursorOnHolograms.SetActive(false);
                CursorOffHolograms.SetActive(true);
            }

            // Update the position and normal of both cursor objects so other scripts that rely on the cursor position
            // can use either GameObject as a dependency.
            // Place the cursor at the calculated position.
            CursorOnHolograms.transform.position = GazeManager.Instance.Position + GazeManager.Instance.Normal * DistanceFromCollision;
            CursorOffHolograms.transform.position = GazeManager.Instance.Position;

            // Reorient the cursors to match the hit object normal.
            CursorOnHolograms.transform.up = GazeManager.Instance.Normal;
            CursorOnHolograms.transform.rotation *= cursorOnHologramsDefaultRotation;

            CursorOffHolograms.transform.up = GazeManager.Instance.Normal;
            CursorOffHolograms.transform.rotation *= cursorOffHologramsDefaultRotation;
        }
    }
}
