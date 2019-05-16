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

        /// <summary>
        /// Broadcaster ip address
        /// </summary>
        [Tooltip("Broadcaster ip address")]
        [SerializeField]
        protected string broadcasterIpAddress = "127.0.0.1";

        /// <summary>
        /// StateSynchronizationSceneManager MonoBehaviour
        /// </summary>
        [Tooltip("StateSynchronizationSceneManager")]
        [SerializeField]
        protected StateSynchronizationSceneManager StateSynchronizationSceneManager;

        /// <summary>
        /// StateSynchronizationBroadcaster MonoBehaviour
        /// </summary>
        [Tooltip("StateSynchronizationBroadcaster MonoBehaviour")]
        [SerializeField]
        protected StateSynchronizationBroadcaster stateSynchronizationBroadcaster;

        /// <summary>
        /// StateSynchronizationObserver MonoBehaviour
        /// </summary>
        [Tooltip("StateSynchronizationObserver MonoBehaviour")]
        [SerializeField]
        protected StateSynchronizationObserver stateSynchronizationObserver;

        /// <summary>
        /// Content to enable in the broadcaster application
        /// </summary>
        [Tooltip("Content to enable in the broadcaster application")]
        [SerializeField]
        protected GameObject broadcastedContent;

        private void Start()
        {
            if (StateSynchronizationSceneManager == null ||
                stateSynchronizationBroadcaster == null ||
                stateSynchronizationObserver == null)
            {
                Debug.LogError("StateSynchronization scene isn't configured correctly");
                return;
            }

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
            StateSynchronizationSceneManager.gameObject.SetActive(true);
        }

        private void RunAsObserver()
        {
            // All content in the observer scene should be dynamically setup/created, so we hide scene content here
            broadcastedContent.SetActive(false);

            stateSynchronizationBroadcaster.gameObject.SetActive(false);
            stateSynchronizationObserver.gameObject.SetActive(true);

            // The StateSynchronizationSceneManager needs to be enabled after the broadcaster/observer
            StateSynchronizationSceneManager.gameObject.SetActive(true);

            // Make sure the StateSynchronizationSceneManager is enabled prior to connecting the observer
            stateSynchronizationObserver.ConnectTo(broadcasterIpAddress);
        }
    }
}
