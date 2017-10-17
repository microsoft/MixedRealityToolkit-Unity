// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloToolkit.Examples.SpatialUnderstandingFeatureOverview
{
    /// <summary>
    /// 1. Decides when to show the cursor.
    /// 2. Positions the cursor at the gazed location.
    /// 3. Rotates the cursor to match hologram normals.
    /// </summary>
    public class SpatialUnderstandingBasicCursor : MonoBehaviour
    {
        public struct RaycastResult
        {
            public bool Hit;
            public Vector3 Position;
            public Vector3 Normal;
        }

        [Tooltip("Distance, in meters, to offset the cursor from the collision point.")]
        public float DistanceFromCollision = 0.01f;

        private Quaternion cursorDefaultRotation;

        private MeshRenderer meshRenderer;

        private GazeManager gazeManager;

        protected virtual void Awake()
        {
            meshRenderer = gameObject.GetComponent<MeshRenderer>();

            if (meshRenderer == null)
            {
                Debug.LogError("This script requires that your cursor asset has a MeshRenderer component on it.");
                return;
            }

            // Hide the Cursor to begin with.
            meshRenderer.enabled = false;

            // Cache the cursor default rotation so the cursor can be rotated with respect to the original rotation.
            cursorDefaultRotation = gameObject.transform.rotation;
        }

        protected virtual void Start()
        {
            gazeManager = GazeManager.Instance;

            if (gazeManager == null)
            {
                Debug.LogError("Must have a GazeManager somewhere in the scene.");
            }

            if ((GazeManager.Instance.RaycastLayerMasks[0] & (1 << gameObject.layer)) != 0)
            {
                Debug.LogError("The cursor has a layer that is checked in the GazeManager's Raycast Layer Mask.  Change the cursor layer (e.g.: to Ignore Raycast) or uncheck the layer in GazeManager: " +
                    LayerMask.LayerToName(gameObject.layer));
            }
        }

        protected virtual RaycastResult CalculateRayIntersect()
        {
            var result = new RaycastResult
            {
                Hit = (GazeManager.Instance.HitObject == null) ? false : true,
                Position = GazeManager.Instance.HitPosition,
                Normal = GazeManager.Instance.GazeNormal
            };

            return result;
        }

        protected virtual void LateUpdate()
        {
            if (meshRenderer == null || gazeManager == null)
            {
                return;
            }

            // Calculate the raycast result
            RaycastResult rayResult = CalculateRayIntersect();

            // Show or hide the Cursor based on if the user's gaze hit a hologram.
            meshRenderer.enabled = rayResult.Hit;

            // Place the cursor at the calculated position.
            gameObject.transform.position = rayResult.Position + rayResult.Normal * DistanceFromCollision;

            // Reorient the cursor to match the hit object normal.
            gameObject.transform.up = rayResult.Normal;
            gameObject.transform.rotation *= cursorDefaultRotation;
        }
    }
}