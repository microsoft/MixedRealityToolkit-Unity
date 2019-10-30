// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

#if UNITY_WSA
using UnityEngine.XR.WSA;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Tracking
{
    /// <summary>
    /// A service that detects when tracking is lost on WSA devices. 
    /// When tracking is lost, the service displays a visual indicator and sets the main camera's culling mask to hide all other objects.
    /// When tracking is restored, the camera mask is restored and the visual indicator is hidden.
    /// </summary>
    [MixedRealityExtensionService(SupportedPlatforms.WindowsUniversal)]
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

        public LostTrackingService(IMixedRealityServiceRegistrar registrar, string name, uint priority, BaseMixedRealityProfile profile) : base(registrar, name, priority, profile)
        {
            this.profile = (LostTrackingServiceProfile)profile;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
#if UNITY_WSA
            WorldManager.OnPositionalLocatorStateChanged += OnPositionalLocatorStateChanged;
#else
            Debug.LogWarning("This service is not supported on this platform.");
#endif
        }

#if UNITY_EDITOR
        /// <inheritdoc />
        public void EditorSetTrackingLost(bool trackingLost)
        {
            SetTrackingLost(trackingLost);
        }
#endif

        private void DisableTrackingLostVisual()
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

        private void EnableTrackingLostVisual()
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

        private void SetTrackingLost(bool trackingLost)
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

#if UNITY_WSA
        private void OnPositionalLocatorStateChanged(PositionalLocatorState oldState, PositionalLocatorState newState)
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
#endif
    }
}