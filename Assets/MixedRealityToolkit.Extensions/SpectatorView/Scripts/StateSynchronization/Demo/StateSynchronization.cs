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
    internal class StateSynchronization : Singleton<StateSynchronization>
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

        [SerializeField]
        protected SynchronizedSceneManager synchronizedSceneManager;

        [SerializeField]
        protected SynchronizedClient userClient;

        [SerializeField]
        protected RemoteClient observerClient;

        void Start()
        {
            if (synchronizedSceneManager == null ||
                userClient == null ||
                observerClient == null)
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

        private void RunAsBroadcaster()
        {
            userClient.gameObject.SetActive(true);
            observerClient.gameObject.SetActive(false);

            synchronizedSceneManager.gameObject.SetActive(true);
        }

        private void RunAsObserver()
        {
            userClient.gameObject.SetActive(false);
            observerClient.gameObject.SetActive(true);

            synchronizedSceneManager.gameObject.SetActive(true);

            observerClient.ConnectTo(broadcasterIpAddress);
        }
    }
}
