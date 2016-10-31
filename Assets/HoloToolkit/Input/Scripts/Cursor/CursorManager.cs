// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// CursorManager class uses two GameObjects to render the Cursor.
    /// One is used when on Holograms and the other when off Holograms.
    /// 1. Shows the appropriate Cursor when a Hologram is hit.
    /// 2. Places the appropriate Cursor at the hit position.
    /// 3. Matches the Cursor normal to the hit surface.
    /// </summary>
    public partial class CursorManager : Singleton<CursorManager>
    {
        [Tooltip("Drag the Cursor object to show when it hits a hologram.")]
        public GameObject CursorOnHolograms;

        [Tooltip("Drag the Cursor object to show when it does not hit a hologram.")]
        public GameObject CursorOffHolograms;

        [Tooltip("Distance, in meters, to offset the cursor from the collision point.")]
        public float DistanceFromCollision = 0.01f;

        protected override void Awake()
        {
            base.Awake();

            // Hide the Cursors to begin with.
            if (CursorOnHolograms != null)
            {
                CursorOnHolograms.SetActive(false);
            }
            if (CursorOffHolograms != null)
            {
                CursorOffHolograms.SetActive(false);
            }
        }

        private void Start()
        {
            // Make sure there is a GazeManager in the scene
            if (GazeManager.Instance == null)
            {
                Debug.LogWarning("CursorManager requires a GazeManager in your scene.");
                enabled = false;
            }
        }

        private void LateUpdate()
        {
            // Enable/Disable the cursor based whether gaze hit a hologram
            if (CursorOnHolograms != null)
            {
                CursorOnHolograms.SetActive(GazeManager.Instance.IsGazingAtObject);
            }
            if (CursorOffHolograms != null)
            {
                CursorOffHolograms.SetActive(!GazeManager.Instance.IsGazingAtObject);
            }

            Vector3 normal = GazeManager.Instance.IsGazingAtObject
                ? GazeManager.Instance.HitInfo.normal
                : -GazeManager.Instance.GazeNormal;
            // Place the cursor at the calculated position.
            gameObject.transform.position = GazeManager.Instance.HitPosition + normal * DistanceFromCollision;

            // Orient the cursor to match the surface being gazed at.
            gameObject.transform.up = normal;
        }
    }
}