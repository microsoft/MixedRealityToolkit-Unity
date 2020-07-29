// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Physics;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// The MousePointer represents a mouse cursor in world space.
    /// It uses spherical movement around the camera.
    /// Its movement is bound to screenspace, but based in the delta movement of the computer mouse.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/MousePointer")]
    public class MousePointer : BaseMousePointer
    {
        /// <inheritdoc />
        protected override string ControllerName => "Spatial Mouse Pointer";

        private static readonly ProfilerMarker OnPreSceneQueryPerfMarker = new ProfilerMarker("[MRTK] MousePointer.OnPreSceneQuery");

        /// <inheritdoc />
        public override void OnPreSceneQuery()
        {
            using (OnPreSceneQueryPerfMarker.Auto())
            {
                // screenspace to ray conversion
                transform.position = CameraCache.Main.transform.position;

                Ray ray = new Ray(transform.position, transform.forward);
                Rays[0].CopyRay(ray, PointerExtent);

                if (MixedRealityRaycaster.DebugEnabled)
                {
                    Debug.DrawRay(ray.origin, ray.direction * PointerExtent, Color.green);
                }
            }
        }

        #region IMixedRealityInputHandler Implementation

        private static readonly ProfilerMarker OnInputChangedVector2PerfMarker = new ProfilerMarker("[MRTK] MousePointer.OnInputChanged(Vector2)");

        /// <inheritdoc />
        public override void OnInputChanged(InputEventData<Vector2> eventData)
        {
            using (OnInputChangedVector2PerfMarker.Auto())
            {
                if (eventData.SourceId == Controller?.InputSource.SourceId)
                {
                    if (PoseAction == eventData.MixedRealityInputAction && !UseSourcePoseData)
                    {
                        UpdateMouseRotation(eventData.InputData);
                    }
                }
            }
        }

        private static readonly ProfilerMarker OnInputChangedPosePerfMarker = new ProfilerMarker("[MRTK] MousePointer.OnInputChanged(Pose)");

        /// <inheritdoc />
        public override void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            using (OnInputChangedPosePerfMarker.Auto())
            {
                if (eventData.SourceId == Controller?.InputSource.SourceId)
                {
                    if (UseSourcePoseData)
                    {
                        UpdateMouseRotation(eventData.InputData.Rotation.eulerAngles);
                    }
                }
            }
        }

        #endregion IMixedRealityInputHandler Implementation

        private static readonly ProfilerMarker UpdateMouseRotationPerfMarker = new ProfilerMarker("[MRTK] MousePointer.UpdateMouseRotation");

        private void UpdateMouseRotation(Vector3 mouseDeltaRotation)
        {
            using (UpdateMouseRotationPerfMarker.Auto())
            {
                if (isDisabled)
                {
                    if (mouseDeltaRotation.magnitude >= MovementThresholdToUnHide)
                    {
                        // if cursor was hidden reset to center
                        SetVisibility(true);
                        transform.rotation = CameraCache.Main.transform.rotation;
                    }
                }
                else
                {
                    timeoutTimer = 0.0f;
                }

                transform.Rotate(mouseDeltaRotation, Space.Self);
            }
        }

        protected override void Start()
        {
            base.Start();

#if UNITY_EDITOR
            if (UnityEditor.EditorWindow.focusedWindow != null)
            {
                UnityEditor.EditorWindow.focusedWindow.ShowNotification(new GUIContent("Press \"ESC\" to regain mouse control"));
            }
#endif

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        /// <inheritdoc />
        protected override void SetVisibility(bool visible)
        {
            base.SetVisibility(visible);
            BaseCursor?.SetVisibility(visible);
        }
    }
}