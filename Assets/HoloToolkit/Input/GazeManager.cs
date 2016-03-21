// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// GazeManager determines the location of the user's gaze, hit position and normals.
    /// </summary>
    public class GazeManager : Singleton<GazeManager>
    {
        [Tooltip("Maximum gaze distance, in meters, for calculating a hit.")]
        public float MaxGazeDistance = 15.0f;

        [Tooltip("Select the layers raycast should target.")]
        public LayerMask RaycastLayerMask = Physics.DefaultRaycastLayers;

        /// <summary>
        /// Physics.Raycast result is true if it hits a Hologram.
        /// </summary>
        public bool Hit { get; private set; }

        /// <summary>
        /// HitInfo property gives access
        /// to RaycastHit public members.
        /// </summary>
        public RaycastHit HitInfo { get; private set; }

        /// <summary>
        /// Position of the intersection of the user's gaze and the hologram's in the scene.
        /// </summary>
        public Vector3 Position { get; private set; }

        /// <summary>
        /// RaycastHit Normal direction.
        /// </summary>
        public Vector3 Normal { get; private set; }

        private Vector3 gazeOrigin;
        private Vector3 gazeDirection;
        private float lastHitDistance = 15.0f;

        private void Update()
        {
            gazeOrigin = Camera.main.transform.position;
            gazeDirection = Camera.main.transform.forward;

            UpdateRaycast();
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

            // Update the HitInfo property so other classes can use this hit information.
            HitInfo = hitInfo;
            
            if (Hit)
            {
                // If the raycast hits a hologram, set the position and normal to match the intersection point.
                Position = hitInfo.point;
                Normal = hitInfo.normal;
                lastHitDistance = hitInfo.distance;
            }
            else
            {
                // If the raycast does not hit a hologram, default the position to last hit distance in front of the user,
                // and the normal to face the user.
                Position = gazeOrigin + (gazeDirection * lastHitDistance);
                Normal = gazeDirection;
            }
        }
    }
}