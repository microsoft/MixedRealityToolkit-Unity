// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal enum Role
    {
        Broadcaster,
        Observer
    }

    /// <summary>
    /// Class for demoing state synchronization
    /// </summary>
    internal class StateSynchronizationDemo : Singleton<StateSynchronizationDemo>
    {
        /// <summary>
        /// Current role of the application
        /// </summary>
        [Tooltip("Current role of the application")]
        [SerializeField]
        public Role Role = Role.Broadcaster;

        [Tooltip("Localization mechanism to use for sptially localizing the SpectatorView participants.")]
        [SerializeField]
        private LocalizationMechanismBase localizationMechanism = null;

        /// <summary>
        /// Broadcaster ip address
        /// </summary>
        [Tooltip("Broadcaster ip address")]
        [SerializeField]
        private string broadcasterIpAddress = "127.0.0.1";

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

        /// <summary>
        /// Content to enable in the broadcaster application
        /// </summary>
        [Tooltip("Content to enable in the broadcaster application")]
        [SerializeField]
        private GameObject broadcastedContent = null;

        private void Start()
        {
            if (stateSynchronizationSceneManager == null ||
                stateSynchronizationBroadcaster == null ||
                stateSynchronizationObserver == null)
            {
                Debug.LogError("StateSynchronization scene isn't configured correctly");
                return;
            }

            stateSynchronizationBroadcaster.LocalizationMechanism = localizationMechanism;
            stateSynchronizationObserver.LocalizationMechanism = localizationMechanism;

            switch (Role)
            {
                case Role.Broadcaster:
                    RunAsBroadcaster();
                    break;
                case Role.Observer:
                    RunAsObserver();
                    break;
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Role == Role.Observer)
            {
                Camera.main.transform.localPosition = StateSynchronizationObserver.Instance.transform.position;
                Camera.main.transform.localRotation = StateSynchronizationObserver.Instance.transform.rotation;
            }
        }
#endif

        private void RunAsBroadcaster()
        {
            broadcastedContent.SetActive(true);
            stateSynchronizationBroadcaster.gameObject.SetActive(true);
            stateSynchronizationObserver.gameObject.SetActive(false);

            // The StateSynchronizationSceneManager needs to be enabled after the broadcaster/observer
            stateSynchronizationSceneManager.gameObject.SetActive(true);
        }

        private void RunAsObserver()
        {
            // All content in the observer scene should be dynamically setup/created, so we hide scene content here
            broadcastedContent.SetActive(false);

            stateSynchronizationBroadcaster.gameObject.SetActive(false);
            stateSynchronizationObserver.gameObject.SetActive(true);

            // The StateSynchronizationSceneManager needs to be enabled after the broadcaster/observer
            stateSynchronizationSceneManager.gameObject.SetActive(true);

            // Make sure the StateSynchronizationSceneManager is enabled prior to connecting the observer
            stateSynchronizationObserver.ConnectTo(broadcasterIpAddress);
        }
    }
}
