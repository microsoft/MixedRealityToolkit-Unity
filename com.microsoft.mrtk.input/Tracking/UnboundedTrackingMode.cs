// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

#if UNITYXR_MANAGEMENT_PRESENT
using UnityEngine.XR.Management;
#endif

namespace Microsoft.MixedReality.Toolkit
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
    /// is set to <see cref="XROrigin.TrackingOriginMode.NotSpecified"/> and the device supports unbounded spaces.
    /// </para>  UnboundedTrackingModeInspector
    /// </remarks>
    [RequireComponent(typeof(XROrigin))]
    public class UnboundedTrackingMode : MonoBehaviour
    {
#if UNITYXR_MANAGEMENT_PRESENT
        private XRInputSubsystem m_inputSubsystem;
        private XROrigin.TrackingOriginMode m_requestedTrackingOriginMode = XROrigin.TrackingOriginMode.NotSpecified;

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

            EnsureSceneOriginAtEyeLevel();
        }

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
                EnsureSceneOriginAtEyeLevel();
            }
        }

        private void EnsureSceneOriginAtEyeLevel()
        {
            TrackingOriginModeFlags currentMode = m_inputSubsystem.GetTrackingOriginMode();
            TrackingOriginModeFlags desiredMode = GetDesiredTrackingOriginMode(m_inputSubsystem);
            if (m_requestedTrackingOriginMode == TrackingOriginMode.NotSpecified &&
                currentMode == TrackingOriginModeFlags.Device &&
                currentMode != desiredMode)
            {
                Debug.Log($"EyeLevelSceneOrigin: TrySetTrackingOriginMode to {desiredMode}");
                if (!m_inputSubsystem.TrySetTrackingOriginMode(desiredMode))
                {
                    Debug.LogWarning($"EyeLevelSceneOrigin: Failed to set tracking origin to {desiredMode}.");
                }
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
