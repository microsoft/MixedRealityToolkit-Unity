﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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
        /// Occurs when gaze enters a GameObjects collider.
        /// </summary>
        /// <param name="g"></param>
        public delegate void OnGazeEnterEvent(GameObject g);
        public event OnGazeEnterEvent OnGazeEnter;

        /// <summary>
        /// Occurs when gaze exits a GameObjects collider.
        /// </summary>
        /// <param name="g"></param>
        public delegate void OnGazeExitEvent(GameObject g);
        public event OnGazeEnterEvent OnGazeExit;

        [Tooltip("Maximum gaze distance, in meters, for calculating a hit.")]
        public float MaxGazeDistance = 15.0f;

        [Tooltip("Select the layers raycast should target.")]
        public LayerMask RaycastLayerMask = Physics.DefaultRaycastLayers;

        /// <summary>
        /// Physics.Raycast result is true if it hits a hologram.
        /// </summary>
        public bool Hit { get; private set; }

        /// <summary>
        /// HitInfo property gives access
        /// to RaycastHit public members.
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

        [Tooltip("Checking enables SetFocusPointForFrame to set the stabilization plane.")]
        public bool SetStabilizationPlane = true;
        [Tooltip("Lerp speed when moving focus point closer.")]
        public float LerpStabilizationPlanePowerCloser = 4.0f;
        [Tooltip("Lerp speed when moving focus point farther away.")]
        public float LerpStabilizationPlanePowerFarther = 7.0f;

        private Vector3 gazeOrigin;
        private Vector3 gazeDirection;
        private float lastHitDistance = 15.0f;

        private void Update()
        {
            gazeOrigin = Camera.main.transform.position;
            gazeDirection = Camera.main.transform.forward;

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
            Hit = Physics.Raycast(gazeOrigin,
                           gazeDirection,
                           out hitInfo,
                           MaxGazeDistance,
                           RaycastLayerMask);

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
                if (oldFocusedObject != null && OnGazeExit != null)
                {
                    OnGazeExit(oldFocusedObject);
                }
                if (FocusedObject != null && OnGazeEnter != null)
                {
                    OnGazeEnter(FocusedObject);
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