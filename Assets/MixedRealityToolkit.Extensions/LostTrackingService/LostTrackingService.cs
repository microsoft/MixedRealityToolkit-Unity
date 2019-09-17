using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Experimental.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace Microsoft.MixedReality.Toolkit.Extensions.Tracking
{
	[MixedRealityExtensionService(SupportedPlatforms.WindowsStandalone|SupportedPlatforms.MacStandalone|SupportedPlatforms.LinuxStandalone|SupportedPlatforms.WindowsUniversal)]
	public class LostTrackingService : BaseExtensionService, ILostTrackingService, IMixedRealityExtensionService
	{
        public bool TrackingLost { get; private set; } = false;

		private LostTrackingServiceProfile profile;
        private ILostTrackingVisual visual;
        private int cullingMaskOnTrackingLost;
        private float timeScaleOnTrackingLost;

		public LostTrackingService(IMixedRealityServiceRegistrar registrar,  string name,  uint priority,  BaseMixedRealityProfile profile) : base(registrar, name, priority, profile) 
		{
			this.profile = (LostTrackingServiceProfile)profile;
		}

		public override void Initialize()
		{
            UnityEngine.XR.WSA.WorldManager.OnPositionalLocatorStateChanged += OnPositionalLocatorStateChanged;
        }

        public override void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha0))
            {
                Debug.Log("Toggling tracking lost...");
                if (!TrackingLost)
                {
                    TrackingLost = true;
                    EnableTrackingLostVisual();
                }
                else
                {
                    TrackingLost = false;
                    DisableTrackingLostVisual();
                }
            }
        }

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

        private void OnPositionalLocatorStateChanged(PositionalLocatorState oldState, PositionalLocatorState newState)
        {
            switch (newState)
            {
                case PositionalLocatorState.Inhibited:
                    TrackingLost = true;
                    EnableTrackingLostVisual();
                    break;

                default:
                    TrackingLost = false;
                    DisableTrackingLostVisual();
                    break;
            }
        }
    }
}