// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

#if UNITY_WSA && !UNITY_2020_1_OR_NEWER
using UnityEngine.XR.WSA;
#endif // UNITY_WSA && !UNITY_2020_1_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.Experimental.Utilities
{
    /// <summary>
    /// StablizationPlaneOverride is a class used to describe the plane to be used by the StabilizationPlaneModifier class
    /// </summary>
    [Serializable]
    public struct StabilizationPlaneOverride
    {
        /// <summary>
        /// Center of the plane
        /// </summary>
        public Vector3 Center;

        /// <summary>
        /// Normal of the plane
        /// </summary>
        public Vector3 Normal;
    }

    /// <summary>
    /// StabilizationPlaneModifier handles the setting of the stabilization plane in several different modes.
    /// It does this via handling the platform call to HolographicPlatformSettings::SetFocusPointForFrame
    /// Using StabilizationPlaneModifier will override DepthLSR. This is automatically enabled via the depth buffer sharing in Unity build settings
    /// StabilizationPlaneModifier is recommended for HoloLens 1, can be used for HoloLens 2, and does a no op for WMR
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/StabilizationPlaneModifier")]
    public class StabilizationPlaneModifier : MonoBehaviour
    {
        [Serializable]
        private enum StabilizationPlaneMode
        {
            /// <summary>
            /// Does not call SetFocusPoint
            /// </summary>
            Off,

            /// <summary>
            /// Submits plane at a fixed distance based on DefaultPlaneDistance field along the users gaze.
            /// </summary>
            Fixed,

            /// <summary>
            /// Submits the plane at a fixed position along the users gaze based on the TargetOverride property.
            /// </summary>
            TargetOverride,

            /// <summary>
            /// Submits the plane based on the OverridePlane property.
            /// </summary>
            PlaneOverride,

            /// <summary>
            /// Submits plane along the users gaze at the position of gaze hit.
            /// </summary>
            GazeHit
        }

        [SerializeField, Tooltip("Choose mode for stabilization plane.\n1) Fixed- Submits plane at a fixed distance based on DefaultPlaneDistance field along the users gaze.\n2) Gaze Hit- Submits plane along the users gaze at the position of gaze hit.\n3) Target Override- Submits the plane at a fixed position along the users gaze based on the TargetOverride property.\n4) Plane Override- Submits the plane based on the OverridePlane property.\n5) Off- Does not call SetFocusPoint")]
        private StabilizationPlaneMode mode = StabilizationPlaneMode.Off;

        [SerializeField, Tooltip("When lerping, use unscaled time. This is useful for apps that have a pause mechanism or otherwise adjust the game timescale.")]
        private bool useUnscaledTime = true;

        [SerializeField, Tooltip("Lerp speed when moving focus point closer.")]
        private float lerpPowerCloser = 4.0f;

        [SerializeField, Tooltip("Lerp speed when moving focus point farther away.")]
        private float lerpPowerFarther = 7.0f;

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
        private bool trackVelocity;
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

        [SerializeField, Tooltip("Default distance to set plane if plane is gaze-locked or if no object is hit.")]
        private float defaultPlaneDistance = 2.0f;

        [SerializeField, Tooltip("Visualize the plane at runtime.")]
#pragma warning disable 414 // Field is used only in UNITY_EDITOR contexts
        private bool drawGizmos = false;
#pragma warning restore 414

        [SerializeField, Tooltip("Override plane to use. Usually used to set plane to a slate like a menu.")]
        private StabilizationPlaneOverride overridePlane;

        /// <summary>
        /// Override plane to use. Usually used to set plane to a slate like a menu.
        /// </summary>
        public StabilizationPlaneOverride OverridePlane
        {
            get => overridePlane;
            set => overridePlane = value;
        }

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

#if UNITY_EDITOR
        /// <summary>
        /// Used for representing latest plane drawn as gizmo
        /// </summary>
        private StabilizationPlaneOverride debugPlane;

        /// <summary>
        /// Used for representing the debug mesh
        /// </summary>
        private GameObject debugMesh;

        /// <summary>
        /// Debug mesh filter
        /// </summary>
        private MeshFilter debugMeshFilter;
#endif

        private void Awake()
        {
#if UNITY_EDITOR
            debugMesh = GameObject.CreatePrimitive(PrimitiveType.Quad);
            debugMesh.hideFlags |= HideFlags.HideInHierarchy;
            debugMesh.SetActive(false);
            debugMeshFilter = debugMesh.GetComponent<MeshFilter>();
#endif

            TrackVelocity = trackVelocity;
            TargetOverride = targetOverride;
        }

        /// <summary>
        /// Updates the focus point for every frame after all objects have finished moving.
        /// </summary>
        private void LateUpdate()
        {
            float deltaTime = useUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            switch (mode)
            {
                case StabilizationPlaneMode.Fixed:
                    ConfigureFixedDistancePlane(deltaTime);
                    break;
                case StabilizationPlaneMode.GazeHit:
                    ConfigureGazeManagerPlane(deltaTime);
                    break;
                case StabilizationPlaneMode.PlaneOverride:
                    ConfigureOverridePlane(deltaTime);
                    break;
                case StabilizationPlaneMode.TargetOverride:
                    ConfigureTransformOverridePlane(deltaTime);
                    break;
                case StabilizationPlaneMode.Off:
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets the origin of the gaze for purposes of placing the stabilization plane
        /// </summary>
        private Vector3 GazeOrigin
        {
            get
            {
                var gazeProvider = CoreServices.InputSystem?.GazeProvider;
                if (gazeProvider != null && gazeProvider.Enabled)
                {
                    return gazeProvider.GazeOrigin;
                }

                return CameraCache.Main.transform.position;
            }
        }

        /// <summary>
        /// Gets the direction of the gaze for purposes of placing the stabilization plane
        /// </summary>
        private Vector3 GazeNormal
        {
            get
            {
                var gazeProvider = CoreServices.InputSystem?.GazeProvider;
                if (gazeProvider != null && gazeProvider.Enabled)
                {
                    return gazeProvider.GazeDirection;
                }

                return CameraCache.Main.transform.forward;
            }
        }

        /// <summary>
        /// Gets the position hit on the object the user is gazing at, if gaze tracking is supported.
        /// </summary>
        /// <param name="hitPosition">The position at which gaze ray intersects with an object.</param>
        /// <returns>True if gaze is supported and an object was hit by gaze, otherwise false.</returns>
        private bool TryGetGazeHitPosition(out Vector3 hitPosition)
        {
            var gazeProvider = CoreServices.InputSystem?.GazeProvider;
            if (gazeProvider != null && gazeProvider.Enabled &&
                gazeProvider.HitInfo.raycastValid)
            {
                hitPosition = gazeProvider.HitPosition;
                return true;
            }

            hitPosition = Vector3.zero;
            return false;
        }

        /// <summary>
        /// Configures the stabilization plane to update its position based on an object in the scene.
        /// </summary>
        private void ConfigureTransformOverridePlane(float deltaTime)
        {
            Vector3 gazeDirection = ConfigureOverridePlaneHelper(TargetOverride.position, deltaTime);

#if UNITY_EDITOR
            debugPlane.Center = planePosition;
            debugPlane.Normal = -gazeDirection;
#endif
        }

        private void ConfigureOverridePlane(float deltaTime)
        {
            ConfigureOverridePlaneHelper(OverridePlane.Center, deltaTime);

#if UNITY_EDITOR
            debugPlane.Center = planePosition;
            debugPlane.Normal = -OverridePlane.Normal;
#endif
        }

        private Vector3 ConfigureOverridePlaneHelper(Vector3 position, float deltaTime)
        {
            planePosition = position;

            Vector3 velocity = Vector3.zero;
            if (TrackVelocity)
            {
                velocity = UpdateVelocity(deltaTime);
            }

            Vector3 gazeOrigin = GazeOrigin;
            Vector3 gazeToPlane = planePosition - gazeOrigin;
            float focusPointDistance = gazeToPlane.magnitude;
            float lerpPower = focusPointDistance > currentPlaneDistance ? lerpPowerFarther
                                                                        : lerpPowerCloser;

            // Smoothly move the focus point from previous hit position to new position.
            currentPlaneDistance = Mathf.Lerp(currentPlaneDistance, focusPointDistance, lerpPower * deltaTime);
            gazeToPlane.Normalize();
            planePosition = gazeOrigin + (gazeToPlane * currentPlaneDistance);

#if UNITY_2019_3_OR_NEWER
            XRSubsystemHelpers.DisplaySubsystem?.SetFocusPlane(planePosition, OverridePlane.Normal, velocity);
#endif // UNITY_2019_3_OR_NEWER

#if UNITY_WSA && !UNITY_2020_1_OR_NEWER
            // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
            // with legacy requirements.
            if (XRSubsystemHelpers.DisplaySubsystem == null)
            {
#pragma warning disable 0618
                // Place the plane at the desired depth in front of the user and billboard it to the gaze origin.
                HolographicSettings.SetFocusPointForFrame(planePosition, OverridePlane.Normal, velocity);
#pragma warning restore 0618
            }
#endif // UNITY_WSA && !UNITY_2020_1_OR_NEWER

            return gazeToPlane;
        }

        /// <summary>
        /// Configures the stabilization plane to update its position based on what your gaze intersects in the scene.
        /// </summary>
        private void ConfigureGazeManagerPlane(float deltaTime)
        {
            Vector3 gazeOrigin = GazeOrigin;
            Vector3 gazeDirection = GazeNormal;

            // Calculate the delta between gaze origin's position and current hit position. If no object is hit, use default distance.
            float focusPointDistance;
            Vector3 gazeHitPosition;
            if (TryGetGazeHitPosition(out gazeHitPosition))
            {
                focusPointDistance = (gazeOrigin - gazeHitPosition).magnitude;
            }
            else
            {
                focusPointDistance = defaultPlaneDistance;
            }

            float lerpPower = focusPointDistance > currentPlaneDistance ? lerpPowerFarther
                                                                        : lerpPowerCloser;

            // Smoothly move the focus point from previous hit position to new position.
            currentPlaneDistance = Mathf.Lerp(currentPlaneDistance, focusPointDistance, lerpPower * deltaTime);

            planePosition = gazeOrigin + (gazeDirection * currentPlaneDistance);

#if UNITY_EDITOR
            debugPlane.Center = planePosition;
            debugPlane.Normal = -gazeDirection;
#else
#if UNITY_2019_3_OR_NEWER
            XRSubsystemHelpers.DisplaySubsystem?.SetFocusPlane(planePosition, -gazeDirection, Vector3.zero);
#endif // UNITY_2019_3_OR_NEWER

#if UNITY_WSA && !UNITY_2020_1_OR_NEWER
            // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
            // with legacy requirements.
            if (XRSubsystemHelpers.DisplaySubsystem == null)
            {
#pragma warning disable 0618
                HolographicSettings.SetFocusPointForFrame(planePosition, -gazeDirection, Vector3.zero);
#pragma warning restore 0618
            }
#endif // UNITY_WSA && !UNITY_2020_1_OR_NEWER
#endif
        }

        /// <summary>
        /// Configures the stabilization plane to update based on a fixed distance away from you.
        /// </summary>
        private void ConfigureFixedDistancePlane(float deltaTime)
        {
            Vector3 gazeOrigin = GazeOrigin;
            Vector3 gazeNormal = GazeNormal;

            float lerpPower = defaultPlaneDistance > currentPlaneDistance ? lerpPowerFarther
                                                                          : lerpPowerCloser;

            // Smoothly move the focus point from previous hit position to new position.
            currentPlaneDistance = Mathf.Lerp(currentPlaneDistance, defaultPlaneDistance, lerpPower * deltaTime);

            planePosition = gazeOrigin + (gazeNormal * currentPlaneDistance);
#if UNITY_EDITOR
            debugPlane.Center = planePosition;
            debugPlane.Normal = -gazeNormal;
#else
#if UNITY_2019_3_OR_NEWER
            XRSubsystemHelpers.DisplaySubsystem?.SetFocusPlane(planePosition, -gazeNormal, Vector3.zero);
#endif // UNITY_2019_3_OR_NEWER

#if UNITY_WSA && !UNITY_2020_1_OR_NEWER
            // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
            // with legacy requirements.
            if (XRSubsystemHelpers.DisplaySubsystem == null)
            {
#pragma warning disable 0618
                HolographicSettings.SetFocusPointForFrame(planePosition, -gazeNormal, Vector3.zero);
#pragma warning restore 0618
            }
#endif // UNITY_WSA && !UNITY_2020_1_OR_NEWER
#endif
        }

        /// <summary>
        /// Tracks the velocity of the target object to be used as a hint for the plane stabilization.
        /// </summary>
        private Vector3 UpdateVelocity(float deltaTime)
        {
            // Roughly calculate the velocity based on previous position, current position, and frame time.
            Vector3 velocity = (TargetOverride.position - targetOverridePreviousPosition) / deltaTime;
            targetOverridePreviousPosition = TargetOverride.position;
            return velocity;
        }

#if UNITY_EDITOR
        /// <summary>
        /// When in editor, draws a magenta quad that visually represents the stabilization plane.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (Application.isPlaying && drawGizmos && mode != StabilizationPlaneMode.Off)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireMesh(debugMeshFilter.sharedMesh, debugPlane.Center, Quaternion.LookRotation(debugPlane.Normal));
                Gizmos.DrawRay(debugPlane.Center, debugPlane.Normal);
            }
        }
#endif
    }
}
