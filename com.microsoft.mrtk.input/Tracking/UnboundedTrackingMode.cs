// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

#if MROPENXR_PRESENT
using Microsoft.MixedReality.OpenXR.Remoting;
#endif

#if UNITYXR_MANAGEMENT_PRESENT
using UnityEngine.XR.Management;
#endif

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// This component works along side Unity's <see cref="XROrigin"/> to ensure that the <see cref="XRInputSubsystem"/> has
    /// the <see cref="TrackingOriginModeFlags.Unbounded"/> flag set on its tracking origin mode if this flag is supported.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Devices like the HoloLens 2 should use unbounded reference space. This reference space enables the viewer to move
    /// freely through a complex environment, often many meters from where they started, while always optimizing for coordinate
    /// system stability near the viewer. For more information about unbounded space see the
    /// <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#XR_MSFT_unbounded_reference_space">OpenXR Specification</see>.
    /// </para>
    /// <para>
    /// The <see cref="TrackingOriginModeFlags.Unbounded"/> flag is only applied if the <see cref="XROrigin.RequestedTrackingOriginMode"/>
    /// is set to <see cref="XROrigin.TrackingOriginMode.NotSpecified"/>, the device supports unbounded spaces, and <see cref="XRInputSubsystem"/>
    /// current tracking mode is set to <see cref="TrackingOriginModeFlags.Device"/>.
    /// </para> 
    /// </remarks>
    [RequireComponent(typeof(XROrigin))]
    [AddComponentMenu("MRTK/Input/Unbounded Tracking Mode")]
    public class UnboundedTrackingMode : MonoBehaviour
    {
#if UNITYXR_MANAGEMENT_PRESENT
        private XRInputSubsystem m_inputSubsystem;
        private XROrigin.TrackingOriginMode m_requestedTrackingOriginMode = XROrigin.TrackingOriginMode.NotSpecified;

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary>
        /// <remarks>
        /// This will attempt to set <see cref="XRInputSubsystem"/> tracking mode to <see cref="TrackingOriginModeFlags.Unbounded"/>.
        /// </remarks>
        private void OnEnable()
        {
            XRGeneralSettings xrSettings = XRGeneralSettings.Instance;
            if (xrSettings == null)
            {
                Debug.LogWarning($"EyeLevelSceneOrigin: XRGeneralSettings is null.");
                return;
            }

            XRManagerSettings xrManager = xrSettings.Manager;
            if (xrManager == null)
            {
                Debug.LogWarning($"EyeLevelSceneOrigin: XRManagerSettings is null.");
                return;
            }

            XRLoader xrLoader = xrManager.activeLoader;
            if (xrLoader == null)
            {
                Debug.LogWarning($"EyeLevelSceneOrigin: XRLoader is null.");
                return;
            }

            m_inputSubsystem = xrLoader.GetLoadedSubsystem<XRInputSubsystem>();
            if (m_inputSubsystem == null)
            {
                Debug.LogWarning($"EyeLevelSceneOrigin: XRInputSubsystem is null.");
                return;
            }

            XROrigin xrOrigin = gameObject.GetComponent<XROrigin>();
            if (xrOrigin != null)
            {
                m_requestedTrackingOriginMode = xrOrigin.RequestedTrackingOriginMode;
            }

            m_inputSubsystem.trackingOriginUpdated += XrInput_trackingOriginUpdated;

            EnsureUnboundedModeSetIfSupported();
        }

        /// <summary>
        /// A Unity event function that is called when the script component has been disabled.
        /// </summary>
        private void OnDisable()
        {
            if (m_inputSubsystem != null)
            {
                m_inputSubsystem.trackingOriginUpdated -= XrInput_trackingOriginUpdated;
                m_inputSubsystem = null;
            }
        }

        private void XrInput_trackingOriginUpdated(XRInputSubsystem obj)
        {
            if (isActiveAndEnabled)
            {
                EnsureUnboundedModeSetIfSupported();
            }
        }

        private void EnsureUnboundedModeSetIfSupported()
        {
            TrackingOriginModeFlags currentMode = m_inputSubsystem.GetTrackingOriginMode();
            TrackingOriginModeFlags desiredMode = GetDesiredTrackingOriginMode(m_inputSubsystem);
            
            if (m_requestedTrackingOriginMode == XROrigin.TrackingOriginMode.NotSpecified &&
                (currentMode == TrackingOriginModeFlags.Device || currentMode == TrackingOriginModeFlags.Unbounded) &&
                currentMode != desiredMode)
            {
                Debug.Log($"UnboundedTrackingMode: TrySetTrackingOriginMode to {desiredMode}");
                if (!m_inputSubsystem.TrySetTrackingOriginMode(desiredMode))
                {
                    Debug.LogWarning($"UnboundedTrackingMode: Failed to set tracking origin to {desiredMode}.");
                }
                else if (desiredMode == TrackingOriginModeFlags.Unbounded)
                {
                    ApplyUnboundedCameraOffset();
                }
            }
        }

        /// <summary>
        /// Apply the <see cref="XROrigin.CameraYOffset"/> to the transform.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="XROrigin"/> class does not yet support "unbounded" devices, which results in 
        /// <see cref="XROrigin"/> failing to correctly apply the specified camera height offset for
        /// "unbounded" device. This function addresses this limitation that is typically seen during
        /// remoting scenarios.
        /// </para>
        /// <para>
        /// Note, problems with the camera height offset are typically seen when using holographic
        /// remoting on HoloLens.
        /// </para>
        /// </remarks>
        private void ApplyUnboundedCameraOffset()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            XROrigin xrOrigin = gameObject.GetComponent<XROrigin>();
            if (xrOrigin != null &&
                xrOrigin.CameraFloorOffsetObject != null)
            {
                var offsetTransform = xrOrigin.CameraFloorOffsetObject.transform;
                var desiredPosition = offsetTransform.localPosition;
                desiredPosition.y = xrOrigin.CameraYOffset;
                offsetTransform.localPosition = desiredPosition;
            }
        }

        private static TrackingOriginModeFlags GetDesiredTrackingOriginMode(XRInputSubsystem xrInput)
        {
            TrackingOriginModeFlags supportedFlags = xrInput.GetSupportedTrackingOriginModes();
            TrackingOriginModeFlags targetFlag = TrackingOriginModeFlags.Device;   // All OpenXR runtime must support LOCAL space

            if (supportedFlags.HasFlag(TrackingOriginModeFlags.Unbounded))
            {
                targetFlag = TrackingOriginModeFlags.Unbounded;
            }

            return targetFlag;
        }
#endif
    }
}
