// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// GazeManager determines the location of the user's gaze, hit position and normals.
    /// </summary>
    public partial class GazeManager : Singleton<GazeManager>
    {
        /// <summary>
        /// Occurs when gaze enters a GameObject's collider.
        /// </summary>
        /// <param name="focusedObject">The GameObject your gaze has entered.</param>
        public delegate void OnGazeEnterEvent(GameObject focusedObject);
        public event OnGazeEnterEvent OnGazeEnter;

        /// <summary>
        /// Occurs when gaze exits a GameObject's collider.
        /// </summary>
        /// <param name="focusedObject">The GameObject your gaze has left.</param>
        public delegate void OnGazeExitEvent(GameObject focusedObject);
        public event OnGazeExitEvent OnGazeExit;

        /// <summary>
        /// Maximum gaze distance, in meters, for calculating a hit from a GameObject's collider.
        /// </summary>
        [Tooltip("Maximum gaze distance, in meters, for calculating a hit.")]
        public float MaxGazeDistance = 15.0f;

        /// <summary>
        /// Select the layers raycast should target.
        /// </summary>
        [Tooltip("Select the layers raycast should target.")]
        public LayerMask RaycastLayerMask = Physics.DefaultRaycastLayers;

        /// <summary>
        /// Use built in gaze stabilization that utilizes gavity wells.
        /// Change to false if you wish to use your own gabilization calculation
        /// and extend this class.
        /// </summary>
        [Tooltip("Use built in gaze stabilization that utilizes gavity wells.")]
        public bool UseBuiltInGazeStabilization = true;

        /// <summary>
        /// Physics.Raycast result is true if it hits a hologram.
        /// </summary>
        public bool Hit { get; private set; }

        /// <summary>
        /// HitInfo property gives access to RaycastHit public members.
        /// </summary>
        public RaycastHit HitInfo { get; private set; }

        /// <summary>
        /// Position of the intersection of the user's gaze and the holograms in the scene.
        /// </summary>
        public Vector3 Position { get; private set; }

        /// <summary>
        /// RaycastHit Normal direction.
        /// </summary>
        public Vector3 Normal { get; private set; }

        /// <summary>
        /// Object currently being focused on.
        /// </summary>
        public GameObject FocusedObject { get; private set; }

        /// <summary>
        /// Checking enables SetFocusPointForFrame to set the stabilization plane.
        /// </summary>
        [Tooltip("Checking enables SetFocusPointForFrame to set the stabilization plane.")]
        public bool SetStabilizationPlane = true;
        /// <summary>
        /// Lerp speed when moving focus point closer.
        /// </summary>
        [Tooltip("Lerp speed when moving focus point closer.")]
        public float LerpStabilizationPlanePowerCloser = 4.0f;

        /// <summary>
        /// Lerp speed when moving focus point farther away.
        /// </summary>
        [Tooltip("Lerp speed when moving focus point farther away.")]
        public float LerpStabilizationPlanePowerFarther = 7.0f;

        /// <summary>
        /// Helper class that stabilizes gaze using gravity wells
        /// </summary>
        public GazeStabilizer GazeStabilization { get; private set; }

        private Vector3 gazeOrigin;
        private Vector3 gazeDirection;
        private Quaternion gazeRotation;
        private float lastHitDistance = 15.0f;

        private void Awake()
        {
            if (UseBuiltInGazeStabilization)
            {
                GazeStabilization = gameObject.GetComponent<GazeStabilizer>() ??
                                    gameObject.AddComponent<GazeStabilizer>();
            }
        }

        private void Update()
        {
            gazeOrigin = Camera.main.transform.position;
            gazeDirection = Camera.main.transform.forward;
            gazeRotation = Camera.main.transform.rotation;

            if (GazeStabilization != null)
            {
                GazeStabilization.UpdateHeadStability(gazeOrigin, gazeRotation);
            }

            UpdateRaycast();
            UpdateStabilizationPlane();
        }

        /// <summary>
        /// Calculates the Raycast hit position and normal.
        /// </summary>
        private void UpdateRaycast()
        {
            // Get the raycast hit information from Unity's physics system.
            RaycastHit hitInfo;

            if (GazeStabilization != null)
            {
                Hit = Physics.Raycast(GazeStabilization.StableHeadRay, out hitInfo, MaxGazeDistance, RaycastLayerMask);
            }
            else
            {
                Hit = Physics.Raycast(gazeOrigin, gazeDirection, out hitInfo, MaxGazeDistance, RaycastLayerMask);
            }

            GameObject oldFocusedObject = FocusedObject;

            // Update the HitInfo property so other classes can use this hit information.
            HitInfo = hitInfo;

            if (Hit)
            {
                // If the raycast hits a hologram, set the position and normal to match the intersection point.
                Position = hitInfo.point;
                Normal = hitInfo.normal;
                lastHitDistance = hitInfo.distance;
                FocusedObject = hitInfo.collider.gameObject;
            }
            else
            {
                // If the raycast does not hit a hologram, default the position to last hit distance in front of the user,
                // and the normal to face the user.
                Position = gazeOrigin + (gazeDirection * lastHitDistance);
                Normal = -gazeDirection;
                FocusedObject = null;
            }

            // Check if the currently hit object has changed
            if (oldFocusedObject != FocusedObject)
            {
                if (oldFocusedObject != null)
                {
                    IInteractable oldInteractable = oldFocusedObject.GetComponent<IInteractable>();
                    if (oldInteractable != null)
                    {
                        oldInteractable.OnGazeExit();
                    }

                    if (OnGazeExit != null)
                    {
                        OnGazeExit(oldFocusedObject);
                    }

                    // Obsolete! Please subscribe to OnGazeExit event.
                    oldFocusedObject.SendMessage("OnGazeLeave", SendMessageOptions.DontRequireReceiver);
                    SendMessage("OnGazeLeave", SendMessageOptions.DontRequireReceiver);
                    // End Obsolete
                }
                if (FocusedObject != null)
                {
                    IInteractable newInteractable = FocusedObject.GetComponent<IInteractable>();
                    if (newInteractable != null)
                    {
                        newInteractable.OnGazeEnter();
                    }

                    if (OnGazeEnter != null)
                    {
                        OnGazeEnter(FocusedObject);
                    }

                    // Obsolete! Please subscribe to OnGazeEnter event.
                    FocusedObject.SendMessage("OnGazeEnter", SendMessageOptions.DontRequireReceiver);
                    SendMessage("OnGazeEnter", SendMessageOptions.DontRequireReceiver);
                    // End Obsolete
                }
            }
        }

        /// <summary>
        /// Adds the stabilization plane modifier if it's enabled and if it doesn't exist yet.
        /// </summary>
        private void UpdateStabilizationPlane()
        {
            // We want to use the stabilization logic.
            if (SetStabilizationPlane)
            {
                // Check if it exists in the scene.
                if (StabilizationPlaneModifier.Instance == null)
                {
                    // If not, add it to us.
                    gameObject.AddComponent<StabilizationPlaneModifier>();
                }
            }

            if (StabilizationPlaneModifier.Instance != null)
            {
                StabilizationPlaneModifier.Instance.SetStabilizationPlane = SetStabilizationPlane;
            }
        }
    }
}