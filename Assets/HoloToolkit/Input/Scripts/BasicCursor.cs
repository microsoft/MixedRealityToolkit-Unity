// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// 1. Decides when to show the cursor.
    /// 2. Positions the cursor at the gazed location.
    /// 3. Rotates the cursor to match hologram normals.
    /// </summary>
    public class BasicCursor : MonoBehaviour
    {
        [Tooltip("Distance, in meters, to offset the cursor from the collision point.")]
        public float DistanceFromCollision = 0.01f;

        private Quaternion cursorDefaultRotation;

        private MeshRenderer meshRenderer;

        void Awake()
        {
            if (GazeManager.Instance == null)
            {
                Debug.Log("Must have a GazeManager somewhere in the scene.");
                return;
            }

            if ((GazeManager.Instance.RaycastLayerMask & this.gameObject.layer) == 0)
            {
                Debug.LogError("The cursor has a layer that is checked in the GazeManager's Raycast Layer Mask.  Change the cursor layer (eg: to Ignore Raycast) or uncheck the layer in GazeManager: " +
                    LayerMask.LayerToName(this.gameObject.layer));
            }

            meshRenderer = this.gameObject.GetComponent<MeshRenderer>();

            if (meshRenderer == null)
            {
                Debug.Log("This script requires that your cursor asset has a MeshRenderer component on it.");
                return;
            }

            // Hide the Cursor to begin with.
            meshRenderer.enabled = false;

            // Cache the cursor default rotation so the cursor can be rotated with respect to the original orientation.
            cursorDefaultRotation = this.gameObject.transform.rotation;
        }

        void LateUpdate()
        {
            if (GazeManager.Instance == null || meshRenderer == null)
            {
                return;
            }

            // Show or hide the Cursor based on if the user's gaze hit a hologram.
            meshRenderer.enabled = GazeManager.Instance.Hit;

            // Place the cursor at the calculated position.
            this.gameObject.transform.position = GazeManager.Instance.Position + GazeManager.Instance.Normal * DistanceFromCollision;

            // Reorient the cursor to match the hit object normal.
            this.gameObject.transform.up = GazeManager.Instance.Normal;
            this.gameObject.transform.rotation *= cursorDefaultRotation;
        }
    }
}