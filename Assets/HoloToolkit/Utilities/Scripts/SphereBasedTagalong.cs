// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// A Tagalong that stays at a fixed distance from the camera and always
    /// seeks to stay on the edge or inside a sphere that is straight in front of the camera.
    /// </summary>
    public class SphereBasedTagalong : MonoBehaviour
    {
        [Tooltip("Sphere radius.")]
        public float SphereRadius = 1.0f;

        [Tooltip("How fast the object will move to the target position.")]
        public float MoveSpeed = 2.0f;

        /// <summary>
        /// When moving, use unscaled time. This is useful for games that have a pause mechanism or otherwise adjust the game timescale.
        /// </summary>
        [SerializeField]
        [Tooltip("When moving, use unscaled time. This is useful for games that have a pause mechanism or otherwise adjust the game timescale.")]
        private bool useUnscaledTime = true;

        /// <summary>
        /// Used to initialize the initial position of the SphereBasedTagalong before being hidden on LateUpdate.
        /// </summary>
        [SerializeField]
        [Tooltip("Used to initialize the initial position of the SphereBasedTagalong before being hidden on LateUpdate.")]
        private bool hideOnStart;

        [SerializeField]
        [Tooltip("Display the sphere in red wireframe for debugging purposes.")]
        private bool debugDisplaySphere = false;

        [SerializeField]
        [Tooltip("Display a small green cube where the target position is.")]
        private bool debugDisplayTargetPosition = false;

        private Vector3 targetPosition;
        private Vector3 optimalPosition;
        private float initialDistanceToCamera;

        private void Start()
        {
            initialDistanceToCamera = Vector3.Distance(transform.position, CameraCache.Main.transform.position);
        }

        private void Update()
        {
            optimalPosition = CameraCache.Main.transform.position + CameraCache.Main.transform.forward * initialDistanceToCamera;
            Vector3 offsetDir = transform.position - optimalPosition;

            if (offsetDir.magnitude > SphereRadius)
            {
                targetPosition = optimalPosition + offsetDir.normalized * SphereRadius;

                float deltaTime = useUnscaledTime
                    ? Time.unscaledDeltaTime
                    : Time.deltaTime;

                transform.position = Vector3.Lerp(transform.position, targetPosition, MoveSpeed * deltaTime);
            }
        }

        private void LateUpdate()
        {
            if (hideOnStart)
            {
                hideOnStart = !hideOnStart;
                gameObject.SetActive(false);
            }
        }

        public void OnDrawGizmos()
        {
            if (Application.isPlaying == false) { return; }

            Color oldColor = Gizmos.color;

            if (debugDisplaySphere)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(optimalPosition, SphereRadius);
            }

            if (debugDisplayTargetPosition)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(targetPosition, new Vector3(0.1f, 0.1f, 0.1f));
            }

            Gizmos.color = oldColor;
        }
    }
}
