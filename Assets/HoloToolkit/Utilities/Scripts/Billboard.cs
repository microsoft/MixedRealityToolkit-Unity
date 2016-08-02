// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    public enum PivotAxis
    {
        // Rotate about all axes.
        Free,
        // Rotate about an individual axis.
        X,
        Y
    }

    /// <summary>
    /// The Billboard class implements the behaviors needed to keep a GameObject
    /// oriented towards the user.
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        /// <summary>
        /// The axis about which the object will rotate.
        /// </summary>
        [Tooltip("Specifies the axis about which the object will rotate (Free rotates about both X and Y).")]
        public PivotAxis PivotAxis = PivotAxis.Free;

        /// <summary>
        /// Overrides the cached value of the GameObject's default rotation.
        /// </summary>
        public Quaternion DefaultRotation { get; private set; }

        private void Awake()
        {
            // Cache the GameObject's default rotation.
            DefaultRotation = gameObject.transform.rotation;
        }

        /// <summary>
        /// Keeps the object facing the camera.
        /// </summary>
        private void Update()
        {
            // Get a Vector that points from the Camera to the target.
            Vector3 forward;
            Vector3 up;

            // Adjust for the pivot axis.
            switch (PivotAxis)
            {
                case PivotAxis.X:
                    Vector3 right = transform.right;
                    forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, right).normalized;
                    up = Vector3.Cross(forward, right);
                    break;

                case PivotAxis.Y:
                    up = transform.up;
                    forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, up).normalized;
                    break;

                case PivotAxis.Free:
                default:
                    forward = Camera.main.transform.forward;
                    up = Camera.main.transform.up;
                    break;
            }


            // Calculate and apply the rotation required to reorient the object
            transform.rotation = Quaternion.LookRotation(forward, up);
        }
    }
}