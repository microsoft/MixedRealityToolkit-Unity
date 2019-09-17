using Microsoft.MixedReality.Toolkit.Extensions.Tracking;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace Microsoft.MixedReality.Common
{
    public class TrackingLossHandler : MonoBehaviour
    {
        [SerializeField]
        private BasicLostTrackingVisual trackingLostVisualController = null;

        [SerializeField]
        private int cullingMask;

        [SerializeField]
        private bool haltTime = true;

        private bool configured = false;

        private GameObject trackingLostParent;

        void Start()
        {
            UnityEngine.XR.WSA.WorldManager.OnPositionalLocatorStateChanged += WorldManager_OnPositionalLocatorStateChanged;
            trackingLostParent = trackingLostVisualController.gameObject;
        }

        void Update()
        {
            //This must be done after Start, to allow for MRTK to set up the CameraCache.
            if (!configured)
            {
                configured = true;
                cullingMask = CameraCache.Main.cullingMask;
                cullingMask = cullingMask & ~(1 << trackingLostParent.layer);
                CameraCache.Main.cullingMask = cullingMask;
            }
        }

        private void DisableTrackingLostVisual()
        {
            CameraCache.Main.cullingMask = cullingMask;
            trackingLostParent.layer = 29;

            if (haltTime)
            {
                Time.timeScale = 1.0f;
            }

            AudioListener.pause = false;


            trackingLostParent.SetActive(false);
        }

        private void EnableTrackingLostVisual()
        {
            CameraCache.Main.cullingMask = (1 << trackingLostParent.layer);

            trackingLostParent.layer = 0;

            if (haltTime)
            {
                Time.timeScale = 0.0f;
            }

            AudioListener.pause = true;

            trackingLostVisualController.ResetVisual();

            trackingLostParent.SetActive(true);
        }

        private void WorldManager_OnPositionalLocatorStateChanged(PositionalLocatorState oldState, PositionalLocatorState newState)
        {
            if (newState == PositionalLocatorState.Active)
            {
                // Handle becoming active
                DisableTrackingLostVisual();
            }
            else if (newState == PositionalLocatorState.Inhibited)
            {
                // Handle becoming rotational only
                EnableTrackingLostVisual();
            }
            else
            {
                trackingLostParent.SetActive(false);
            }
        }
    }
}