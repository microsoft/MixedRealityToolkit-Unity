// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using Unity.Profiling;
using UnityEngine;

#if ARSUBSYSTEMS_ENABLED
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;

#if UNITY_2018
using UnityEngine.Experimental;
#endif // UNITY_2018
#endif // ARSUBSYSTEMS_ENABLED

#if UNITY_WSA && !UNITY_2020_1_OR_NEWER
using UnityEngine.XR.WSA;
#endif // UNITY_WSA && !UNITY_2020_1_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.Extensions.Tracking
{
    /// <summary>
    /// A service that detects when tracking is lost on WSA devices. 
    /// When tracking is lost, the service displays a visual indicator and sets the main camera's culling mask to hide all other objects.
    /// When tracking is restored, the camera mask is restored and the visual indicator is hidden.
    /// </summary>
    [MixedRealityExtensionService(
        SupportedPlatforms.WindowsUniversal,
        "Tracking Lost Service",
        "LostTrackingService/Profiles/DefaultLostTrackingServiceProfile.asset",
        "MixedRealityToolkit.Extensions")]
    public class LostTrackingService : BaseExtensionService, ILostTrackingService, IMixedRealityExtensionService
    {
        /// <inheritdoc />
        public bool TrackingLost { get; private set; } = false;

        /// <inheritdoc />
        public Action OnTrackingLost { get; set; }

        /// <inheritdoc />
        public Action OnTrackingRestored { get; set; }

        private readonly LostTrackingServiceProfile profile;
        private ILostTrackingVisual visual;
        private int cullingMaskOnTrackingLost;
        private float timeScaleOnTrackingLost;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        [Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public LostTrackingService(
            IMixedRealityServiceRegistrar registrar,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : this(name, priority, profile)
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public LostTrackingService(
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(name, priority, profile)
        {
            this.profile = profile as LostTrackingServiceProfile;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
#if UNITY_WSA && !UNITY_2020_1_OR_NEWER
            WorldManager.OnPositionalLocatorStateChanged += OnPositionalLocatorStateChanged;
#elif !ARSUBSYSTEMS_ENABLED
            Debug.LogWarning("This service is not supported on this platform.");
#endif
        }

#if ARSUBSYSTEMS_ENABLED
        private UnityEngine.XR.ARSubsystems.TrackingState lastTrackingState = UnityEngine.XR.ARSubsystems.TrackingState.None;
        private NotTrackingReason lastNotTrackingReason = NotTrackingReason.None;

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] LostTrackingService.Update");

        /// <inheritdoc />
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                XRSessionSubsystem sessionSubsystem = SessionSubsystem;
                if (sessionSubsystem == null)
                {
                    return;
                }

                if (sessionSubsystem.trackingState == lastTrackingState && sessionSubsystem.notTrackingReason == lastNotTrackingReason)
                {
                    return;
                }

                // This combination of states is from the Windows XR Plugin docs, describing the combination when positional tracking is inhibited.
                if (sessionSubsystem.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.None && sessionSubsystem.notTrackingReason == NotTrackingReason.Relocalizing)
                {
                    SetTrackingLost(true);
                }
                else
                {
                    SetTrackingLost(false);
                }

                lastTrackingState = sessionSubsystem.trackingState;
                lastNotTrackingReason = sessionSubsystem.notTrackingReason;
            }
        }
#endif // ARSUBSYSTEMS_ENABLED

#if UNITY_EDITOR
        /// <inheritdoc />
        public void EditorSetTrackingLost(bool trackingLost) => SetTrackingLost(trackingLost);
#endif // UNITY_EDITOR

        private static readonly ProfilerMarker DisableTrackingLostVisualPerfMarker = new ProfilerMarker("[MRTK] LostTrackingService.DisableTrackingLostVisual");

        private void DisableTrackingLostVisual()
        {
            using (DisableTrackingLostVisualPerfMarker.Auto())
            {
                if (visual != null && visual.Enabled)
                {
                    CameraCache.Main.cullingMask = cullingMaskOnTrackingLost;

                    if (profile.HaltTimeWhileTrackingLost)
                    {
                        Time.timeScale = timeScaleOnTrackingLost;
                    }

                    if (profile.HaltAudioOnTrackingLost)
                    {
                        AudioListener.pause = false;
                    }

                    visual.Enabled = false;
                }
            }
        }

        private static readonly ProfilerMarker EnableTrackingLostVisualPerfMarker = new ProfilerMarker("[MRTK] LostTrackingService.EnableTrackingLostVisual");

        private void EnableTrackingLostVisual()
        {
            using (EnableTrackingLostVisualPerfMarker.Auto())
            {
                if (visual == null)
                {
                    GameObject visualObject = UnityEngine.Object.Instantiate(profile.TrackingLostVisualPrefab);

                    if (visualObject != null)
                    {
                        visual = visualObject.GetComponentInChildren<ILostTrackingVisual>();
                    }

                    if (visual == null)
                    {
                        Debug.LogError("No ILostTrackingVisual found on prefab supplied by LostTrackingServiceProfile.");
                        return;
                    }

                    visual.Enabled = false;
                }

                if (!visual.Enabled)
                {
                    // Store these settings for later when tracking is regained
                    cullingMaskOnTrackingLost = CameraCache.Main.cullingMask;
                    timeScaleOnTrackingLost = Time.timeScale;
                    CameraCache.Main.cullingMask = profile.TrackingLostCullingMask;

                    if (profile.HaltTimeWhileTrackingLost)
                    {
                        Time.timeScale = 0.0f;
                    }

                    if (profile.HaltAudioOnTrackingLost)
                    {
                        AudioListener.pause = true;
                    }

                    visual.Enabled = true;
                    visual.SetLayer(profile.TrackingLostVisualLayer);
                    visual.ResetVisual();
                }
            }
        }

        private static readonly ProfilerMarker SetTrackingLostPerfMarker = new ProfilerMarker("[MRTK] LostTrackingService.SetTrackingLost");

        private void SetTrackingLost(bool trackingLost)
        {
            using (SetTrackingLostPerfMarker.Auto())
            {
                if (TrackingLost != trackingLost)
                {
                    TrackingLost = trackingLost;
                    if (TrackingLost)
                    {
                        OnTrackingLost?.Invoke();
                        EnableTrackingLostVisual();
                    }
                    else
                    {
                        OnTrackingRestored?.Invoke();
                        DisableTrackingLostVisual();
                    }
                }
            }
        }

#if UNITY_WSA && !UNITY_2020_1_OR_NEWER
        private static readonly ProfilerMarker OnPositionLocatorStateChangedPerfMarker = new ProfilerMarker("[MRTK] LostTrackingService.OnPositionalLocatorStateChanged");

        private void OnPositionalLocatorStateChanged(PositionalLocatorState oldState, PositionalLocatorState newState)
        {
            using (OnPositionLocatorStateChangedPerfMarker.Auto())
            {
                switch (newState)
                {
                    case PositionalLocatorState.Inhibited:
                        SetTrackingLost(true);
                        break;

                    default:
                        SetTrackingLost(false);
                        break;
                }
            }
        }
#endif // UNITY_WSA && !UNITY_2020_1_OR_NEWER

#if ARSUBSYSTEMS_ENABLED
        private static XRSessionSubsystem sessionSubsystem = null;
        private static readonly List<XRSessionSubsystem> XRSessionSubsystems = new List<XRSessionSubsystem>();

        /// <summary>
        /// The XR SDK display subsystem for the currently loaded XR plug-in.
        /// </summary>
        private static XRSessionSubsystem SessionSubsystem
        {
            get
            {
                if (sessionSubsystem == null || !sessionSubsystem.running)
                {
                    sessionSubsystem = null;
                    SubsystemManager.GetInstances(XRSessionSubsystems);
                    foreach (XRSessionSubsystem xrSessionSubsystem in XRSessionSubsystems)
                    {
                        if (xrSessionSubsystem.running)
                        {
                            sessionSubsystem = xrSessionSubsystem;
                            break;
                        }
                    }
                }
                return sessionSubsystem;
            }
        }
#endif // ARSUBSYSTEMS_ENABLED
    }
}
