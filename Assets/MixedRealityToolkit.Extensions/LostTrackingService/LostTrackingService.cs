// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.XR.WSA;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Extensions.Tracking
{
    [MixedRealityExtensionService(SupportedPlatforms.WindowsUniversal)]
    public class LostTrackingService : BaseExtensionService, ILostTrackingService, IMixedRealityExtensionService
    {
        public bool TrackingLost { get; private set; } = false;
        public Action OnTrackingLost { get; set; }
        public Action OnTrackingRestored { get; set; }

        private LostTrackingServiceProfile profile;
        private ILostTrackingVisual visual;
        private int cullingMaskOnTrackingLost;
        private float timeScaleOnTrackingLost;

        public LostTrackingService(IMixedRealityServiceRegistrar registrar, string name, uint priority, BaseMixedRealityProfile profile) : base(registrar, name, priority, profile)
        {
            this.profile = (LostTrackingServiceProfile)profile;
        }

        public override void Initialize()
        {
#if UNITY_WSA
            UnityEngine.XR.WSA.WorldManager.OnPositionalLocatorStateChanged += OnPositionalLocatorStateChanged;
#else
            Debug.LogWarning("This service is not supported on this platform.");
#endif
        }

#if UNITY_EDITOR
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
                GameObject visualObject = GameObject.Instantiate(profile.TrackingLostVisualPrefab);
                visual = visualObject?.GetComponentInChildren<ILostTrackingVisual>();

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
            bool trackingLost = TrackingLost;
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