// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// DirectionIndicator creates an indicator around the cursor showing
    /// what direction to turn to find this GameObject.
    /// </summary>
    public class DirectionIndicator : MonoBehaviour
    {
        [Tooltip("The Cursor object the direction indicator will be positioned around.")]
        public GameObject Cursor;

        [Tooltip("Model to display the direction to the object this script is attached to.")]
        public GameObject DirectionIndicatorObject;

        [Tooltip("Color to shade the direction indicator.")]
        public Color DirectionIndicatorColor = Color.blue;

        [Tooltip("Allowable percentage inside the holographic frame to continue to show a directional indicator.")]
        [Range(-0.3f, 0.3f)]
        public float VisibilitySafeFactor = 0.1f;

        [Tooltip("Multiplier to decrease the distance from the cursor center an object is rendered to keep it in view.")]
        [Range(0.1f, 1.0f)]
        public float MetersFromCursor = 0.3f;

        // The default rotation of the cursor direction indicator.
        private Quaternion directionIndicatorDefaultRotation = Quaternion.identity;

        // Cache the MeshRenderer for the on-cursor indicator since it will be enabled and disabled frequently.
        private Renderer directionIndicatorRenderer;

        // Cache the Material to prevent material leak.
        private Material indicatorMaterial;

        // Check if the cursor direction indicator is visible.
        private bool isDirectionIndicatorVisible;

        public void Awake()
        {
            if (Cursor == null)
            {
                Debug.LogError("Please include a GameObject for the cursor.");
            }

            if (DirectionIndicatorObject == null)
            {
                Debug.LogError("Please include a GameObject for the Direction Indicator.");
            }

            // Instantiate the direction indicator.
            DirectionIndicatorObject = InstantiateDirectionIndicator(DirectionIndicatorObject);

            if (DirectionIndicatorObject == null)
            {
                Debug.LogError("Direction Indicator failed to instantiate.");
            }
        }

        public void OnDestroy()
        {
            DestroyImmediate(indicatorMaterial);
            Destroy(DirectionIndicatorObject);
        }

        private GameObject InstantiateDirectionIndicator(GameObject directionIndicator)
        {
            if (directionIndicator == null)
            {
                return null;
            }

            GameObject indicator = Instantiate(directionIndicator);

            // Set local variables for the indicator.
            directionIndicatorDefaultRotation = indicator.transform.rotation;
            directionIndicatorRenderer = indicator.GetComponent<Renderer>();

            // Start with the indicator disabled.
            directionIndicatorRenderer.enabled = false;

            // Remove any colliders and rigidbodies so the indicators do not interfere with Unity's physics system.
            foreach (Collider indicatorCollider in indicator.GetComponents<Collider>())
            {
                Destroy(indicatorCollider);
            }

            foreach (Rigidbody rigidBody in indicator.GetComponents<Rigidbody>())
            {
                Destroy(rigidBody);
            }

            indicatorMaterial = directionIndicatorRenderer.material;
            indicatorMaterial.color = DirectionIndicatorColor;
            indicatorMaterial.SetColor("_TintColor", DirectionIndicatorColor);

            return indicator;
        }

        public void Update()
        {
            if (DirectionIndicatorObject == null)
            {
                return;
            }

            // Direction from the Main Camera to this script's parent gameObject.
            Vector3 camToObjectDirection = gameObject.transform.position - Camera.main.transform.position;
            camToObjectDirection.Normalize();

            // The cursor indicator should only be visible if the target is not visible.
            isDirectionIndicatorVisible = !IsTargetVisible();
            directionIndicatorRenderer.enabled = isDirectionIndicatorVisible;

            if (isDirectionIndicatorVisible)
            {
                Vector3 position;
                Quaternion rotation;
                GetDirectionIndicatorPositionAndRotation(
                    camToObjectDirection,
                    out position,
                    out rotation);

                DirectionIndicatorObject.transform.position = position;
                DirectionIndicatorObject.transform.rotation = rotation;
            }
        }

        private bool IsTargetVisible()
        {
            // This will return true if the target's mesh is within the Main Camera's view frustums.
            Vector3 targetViewportPosition = Camera.main.WorldToViewportPoint(gameObject.transform.position);
            return (targetViewportPosition.x > VisibilitySafeFactor && targetViewportPosition.x < 1 - VisibilitySafeFactor &&
                    targetViewportPosition.y > VisibilitySafeFactor && targetViewportPosition.y < 1 - VisibilitySafeFactor &&
                    targetViewportPosition.z > 0);
        }

        private void GetDirectionIndicatorPositionAndRotation(Vector3 camToObjectDirection, out Vector3 position, out Quaternion rotation)
        {
            // Find position:
            // Save the cursor transform position in a variable.
            Vector3 origin = Cursor.transform.position;

            // Project the camera to target direction onto the screen plane.
            Vector3 cursorIndicatorDirection = Vector3.ProjectOnPlane(camToObjectDirection, -1 * Camera.main.transform.forward);
            cursorIndicatorDirection.Normalize();

            // If the direction is 0, set the direction to the right.
            // This will only happen if the camera is facing directly away from the target.
            if (cursorIndicatorDirection == Vector3.zero)
            {
                cursorIndicatorDirection = Camera.main.transform.right;
            }

            // The final position is translated from the center of the screen along this direction vector.
            position = origin + cursorIndicatorDirection * MetersFromCursor;

            // Find the rotation from the facing direction to the target object.
            rotation = Quaternion.LookRotation(Camera.main.transform.forward, cursorIndicatorDirection) * directionIndicatorDefaultRotation;
        }
    }
}