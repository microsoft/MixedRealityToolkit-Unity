// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

using System.Runtime.CompilerServices;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.ScreenRecording;

[assembly: InternalsVisibleToAttribute("Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Editor")]

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public enum Role
    {
        User,
        Spectator
    }

    /// <summary>
    /// Class that facilitates the Spectator View experience
    /// </summary>
    public class SpectatorView : MonoBehaviour
    {
        /// <summary>
        /// Role of the device in the spectator view experience.
        /// </summary>
        [Tooltip("Role of the device in the spectator view experience.")]
        [SerializeField]
        public Role Role;

        [Header("Networking")]
        /// <summary>
        /// User ip address
        /// </summary>
        [Tooltip("User ip address")]
        [SerializeField]
        private string userIpAddress = "127.0.0.1";

        [Header("State Synchronization")]
        /// <summary>
        /// StateSynchronizationSceneManager MonoBehaviour
        /// </summary>
        [Tooltip("StateSynchronizationSceneManager")]
        [SerializeField]
        private StateSynchronizationSceneManager stateSynchronizationSceneManager = null;

        /// <summary>
        /// StateSynchronizationBroadcaster MonoBehaviour
        /// </summary>
        [Tooltip("StateSynchronizationBroadcaster MonoBehaviour")]
        [SerializeField]
        private StateSynchronizationBroadcaster stateSynchronizationBroadcaster = null;

        /// <summary>
        /// StateSynchronizationObserver MonoBehaviour
        /// </summary>
        [Tooltip("StateSynchronizationObserver MonoBehaviour")]
        [SerializeField]
        private StateSynchronizationObserver stateSynchronizationObserver = null;

        [Header("Recording")]
        /// <summary>
        /// Check to enable the mobile recording service.
        /// </summary>
        [Tooltip("Check to enable the mobile recording service.")]
        [SerializeField]
        public bool enableMobileRecordingService = true;

        /// <summary>
        /// Prefab for creating a mobile recording service visual.
        /// </summary>
        [Tooltip("Prefab for creating a mobile recording service visual.")]
        [SerializeField]
        public GameObject mobileRecordingServiceVisualPrefab = null;

        private GameObject mobileRecordingServiceVisual = null;
        private IRecordingService recordingService = null;
        private IRecordingServiceVisual recordingServiceVisual = null;

        private void Awake()
        {
            Debug.Log($"SpectatorView is running as: {Role.ToString()}. Expected User IPAddress: {userIpAddress}");

            if (stateSynchronizationSceneManager == null ||
                stateSynchronizationBroadcaster == null ||
                stateSynchronizationObserver == null)
            {
                Debug.LogError("StateSynchronization scene isn't configured correctly");
                return;
            }

            switch (Role)
            {
                case Role.User:
                    {
                        RunStateSynchronizationAsBroadcaster();
                    }
                    break;
                case Role.Spectator:
                    {
                        RunStateSynchronizationAsObserver();
                    }
                    break;
            }

            SetupRecordingService();
        }

        private void RunStateSynchronizationAsBroadcaster()
        {
            stateSynchronizationBroadcaster.gameObject.SetActive(true);
            stateSynchronizationObserver.gameObject.SetActive(false);

            // The StateSynchronizationSceneManager needs to be enabled after the broadcaster/observer
            stateSynchronizationSceneManager.gameObject.SetActive(true);
        }

        private void RunStateSynchronizationAsObserver()
        {
            stateSynchronizationBroadcaster.gameObject.SetActive(false);
            stateSynchronizationObserver.gameObject.SetActive(true);

            // The StateSynchronizationSceneManager needs to be enabled after the broadcaster/observer
            stateSynchronizationSceneManager.gameObject.SetActive(true);

            // Make sure the StateSynchronizationSceneManager is enabled prior to connecting the observer
            stateSynchronizationObserver.ConnectTo(userIpAddress);
        }

        private void SetupRecordingService()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (enableMobileRecordingService &&
                mobileRecordingServiceVisualPrefab != null)
            {
                mobileRecordingServiceVisual = Instantiate(mobileRecordingServiceVisualPrefab);

                if (!TryCreateRecordingService(out recordingService))
                {
                    Debug.LogError("Failed to create a recording service for the current platform.");
                    return;
                }

                recordingServiceVisual = mobileRecordingServiceVisual.GetComponentInChildren<IRecordingServiceVisual>();
                if (recordingServiceVisual == null)
                {
                    Debug.LogError("Failed to find an IRecordingServiceVisual in the created mobileRecordingServiceVisualPrefab. Note: It's assumed that the IRecordingServiceVisual is enabled by default in the mobileRecordingServiceVisualPrefab.");
                    return;
                }

                recordingServiceVisual.SetRecordingService(recordingService);
            }
#endif
        }

        private bool TryCreateRecordingService(out IRecordingService recordingService)
        {
#if UNITY_ANDROID
            recordingService = new AndroidRecordingService();
            return true;
#elif UNITY_IOS
            recordingService = new iOSRecordingService();
            return true;
#else
            recordingService = null;
            return false;
#endif
        }
    }
}
