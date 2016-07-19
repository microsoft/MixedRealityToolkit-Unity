// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.VR.WSA;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// StabilizationPlaneModifier handles the setting of the stabilization plane in several ways.
    /// </summary>
    public class StabilizationPlaneModifier : Singleton<StabilizationPlaneModifier>
    {
        [Tooltip("Checking enables SetFocusPointForFrame to set the stabilization plane.")]
        public bool SetStabilizationPlane = true;
        [Tooltip("Lerp speed when moving focus point closer.")]
        public float LerpStabilizationPlanePowerCloser = 4.0f;
        [Tooltip("Lerp speed when moving focus point farther away.")]
        public float LerpStabilizationPlanePowerFarther = 7.0f;

        [SerializeField, Tooltip("Used to temporarily override the location of the stabilization plane.")]
        private Transform targetOverride;
        public Transform TargetOverride
        {
            get
            {
                return targetOverride;
            }
            set
            {
                if (targetOverride != value)
                {
                    targetOverride = value;
                    if (targetOverride)
                    {
                        targetOverridePreviousPosition = targetOverride.position;
                    }
                }
            }
        }

        [SerializeField, Tooltip("Keeps track of position-based velocity for the target object.")]
        private bool trackVelocity = false;
        public bool TrackVelocity
        {
            get
            {
                return trackVelocity;
            }
            set
            {
                trackVelocity = value;
                if (TargetOverride)
                {
                    targetOverridePreviousPosition = TargetOverride.position;
                }
            }
        }

        [Tooltip("Use the GazeManager class to set the plane to the gazed upon hologram.")]
        public bool UseGazeManager = true;

        [Tooltip("Default distance to set plane if plane is gaze-locked.")]
        public float DefaultPlaneDistance = 2.0f;

        [Tooltip("Visualize the plane at runtime.")]
        public bool DrawGizmos = false;

        /// <summary>
        /// Position of the plane in world space.
        /// </summary>
        private Vector3 planePosition;

        /// <summary>
        /// Current distance of the plane from the user's head. Only used when not using the target override
        /// or the GazeManager to set the plane's position. 
        /// </summary>
        private float currentPlaneDistance = 4.0f;

        /// <summary>
        /// Tracks the previous position of the target override object. Used if velocity is being tracked.
        /// </summary>
        private Vector3 targetOverridePreviousPosition;

        /// <summary>
        /// Updates the focus point for every frame after all objects have finished moving.
        /// </summary>
        private void LateUpdate()
        {
            if (SetStabilizationPlane && Camera.main != null)
            {
                if (TargetOverride != null)
                {
                    ConfigureTransformOverridePlane();
                }
                else if (UseGazeManager && GazeManager.Instance != null)
                {
                    ConfigureGazeManagerPlane();
                }
                else
                {
                    ConfigureFixedDistancePlane();
                }

#if UNITY_EDITOR
                if (DrawGizmos)
                {
                    OnDrawGizmos();
                }
#endif
            }
        }

        private void OnValidate()
        {
            TrackVelocity = trackVelocity;
            TargetOverride = targetOverride;
        }

        /// <summary>
        /// Configures the stabilization plane to update its position based on an object in the scene.        
        /// </summary>
        private void ConfigureTransformOverridePlane()
        {
            planePosition = TargetOverride.position;

            Vector3 velocity = Vector3.zero;
            if (TrackVelocity)
            {
                velocity = UpdateVelocity();
            }
            
            // Place the plane at the desired depth in front of the camera and billboard it to the camera.
            HolographicSettings.SetFocusPointForFrame(planePosition, -Camera.main.transform.forward, velocity);
        }

        /// <summary>
        /// Configures the stabilization plane to update its position based on what your gaze intersects in the scene.
        /// </summary>
        private void ConfigureGazeManagerPlane()
        {
            Vector3 gazeOrigin = Camera.main.transform.position;
            Vector3 gazeDirection = Camera.main.transform.forward;

            // Calculate the delta between camera's position and current hit position.
            float focusPointDistance = (gazeOrigin - GazeManager.Instance.Position).magnitude;
            float lerpPower = focusPointDistance > currentPlaneDistance ? LerpStabilizationPlanePowerFarther
                                                                        : LerpStabilizationPlanePowerCloser;

            // Smoothly move the focus point from previous hit position to new position.
            currentPlaneDistance = Mathf.Lerp(currentPlaneDistance, focusPointDistance, lerpPower * Time.deltaTime);

            planePosition = gazeOrigin + (gazeDirection * currentPlaneDistance);

            HolographicSettings.SetFocusPointForFrame(planePosition, -gazeDirection, Vector3.zero);
        }

        /// <summary>
        /// Configures the stabilization plane to update based on a fixed distance away from you.
        /// </summary>
        private void ConfigureFixedDistancePlane()
        {
            float lerpPower = DefaultPlaneDistance > currentPlaneDistance ? LerpStabilizationPlanePowerFarther
                                                                          : LerpStabilizationPlanePowerCloser;

            // Smoothly move the focus point from previous hit position to new position.
            currentPlaneDistance = Mathf.Lerp(currentPlaneDistance, DefaultPlaneDistance, lerpPower * Time.deltaTime);

            planePosition = Camera.main.transform.position + (Camera.main.transform.forward * currentPlaneDistance);
            HolographicSettings.SetFocusPointForFrame(planePosition, -Camera.main.transform.forward, Vector3.zero);
        }

        /// <summary>
        /// Tracks the velocity of the target object to be used as a hint for the plane stabilization.
        /// </summary>
        private Vector3 UpdateVelocity()
        {
            // Roughly calculate the velocity based on previous position, current position, and frame time.
            Vector3 velocity = (TargetOverride.position - targetOverridePreviousPosition) / Time.deltaTime;
            targetOverridePreviousPosition = TargetOverride.position;
            return velocity;
        }

        /// <summary>
        /// When in editor, draws a magenta quad that visually represents the stabilization plane.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (UnityEngine.Application.isPlaying)
            {
                Vector3 focalPlaneNormal = -Camera.main.transform.forward;
                Vector3 planeUp = Vector3.Cross(Vector3.Cross(focalPlaneNormal, Vector3.up), focalPlaneNormal);
                Gizmos.matrix = Matrix4x4.TRS(planePosition, Quaternion.LookRotation(focalPlaneNormal, planeUp), new Vector3(4.0f, 3.0f, 0.01f));

                Color gizmoColor = Color.magenta;
                gizmoColor.a = 0.5f;
                Gizmos.color = gizmoColor;

                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }
    }
}