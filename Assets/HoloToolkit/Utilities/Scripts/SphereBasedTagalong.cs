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

        [Tooltip("When moving, use unscaled time. This is useful for games that have a pause mechanism or otherwise adjust the game timescale.")]
        public bool UseUnscaledTime = true;

        [Tooltip("Display the sphere in red wireframe for debugging purposes.")]
        public bool DebugDisplaySphere = false;

        [Tooltip("Display a small green cube where the target position is.")]
        public bool DebugDisplayTargetPosition = false;

        private Vector3 targetPosition;
        private Vector3 optimalPosition;
        private float initialDistanceToCamera;

        void Start()
        {
            initialDistanceToCamera = Vector3.Distance(this.transform.position, Camera.main.transform.position);
        }

        void Update()
        {
            optimalPosition = Camera.main.transform.position + Camera.main.transform.forward * initialDistanceToCamera;

            Vector3 offsetDir = this.transform.position - optimalPosition;
            if (offsetDir.magnitude > SphereRadius)
            {
                targetPosition = optimalPosition + offsetDir.normalized * SphereRadius;

                float deltaTime = UseUnscaledTime
                    ? Time.unscaledDeltaTime
                    : Time.deltaTime;

                this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, MoveSpeed * deltaTime);
            }
        }

        public void OnDrawGizmos()
        {
            if (Application.isPlaying == false) return;

            Color oldColor = Gizmos.color;

            if (DebugDisplaySphere)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(optimalPosition, SphereRadius);
            }

            if (DebugDisplayTargetPosition)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(targetPosition, new Vector3(0.1f, 0.1f, 0.1f));
            }

            Gizmos.color = oldColor;
        }
    }
}