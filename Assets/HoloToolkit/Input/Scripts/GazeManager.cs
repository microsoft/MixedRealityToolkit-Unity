// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VR.WSA;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// GazeManager makes the location of the user's gaze, hit position and normals accessible to other components.
    /// </summary>
    public partial class GazeManager : Singleton<GazeManager>
    {
        public GGVInputModule InputModule;

        /// <summary>
        /// Indicates whether the user is currently gazing at a hologram.
        /// </summary>
        public bool IsGazingAtHologram { get; private set; }

        /// <summary>
        /// HitInfo property gives access to information at the hologram being gazed at, if any.
        /// </summary>
        public RaycastResult HitInfo { get; private set; }

        /// <summary>
        /// Position of the intersection of the user's gaze and the holograms in the scene.
        /// </summary>
        public Vector3 Position { get; private set; }

        /// <summary>
        /// Normal direction.
        /// </summary>
        public Vector3 Normal { get; private set; }

        private Vector3 gazeOrigin;
        private Vector3 gazeDirection;
        private float lastHitDistance = 4.0f;

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
            // Update the HitInfo property so other classes can use this hit information.
            HitInfo = InputModule.RaycastResult;
            IsGazingAtHologram = HitInfo.isValid;

            if (this.IsGazingAtHologram)
            {
                // If the raycast hits a hologram, set the position and normal to match the intersection point.
                Position = HitInfo.worldPosition;
                Normal = HitInfo.worldNormal;
                lastHitDistance = HitInfo.distance;

                if (Position == Vector3.zero)
                {
                    Position = gazeOrigin + (gazeDirection * this.lastHitDistance);
                }
                if (Normal == Vector3.zero)
                {
                    Normal = gazeDirection;
                }
            }
            else
            {
                // If the raycast does not hit a hologram, default the position to last hit distance in front of the user,
                // and the normal to face the user.
                Position = gazeOrigin + (gazeDirection * this.lastHitDistance);
                Normal = gazeDirection;
            }
        }
    }
}