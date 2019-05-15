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

        /// <summary>
        /// SynchronizedSceneManager MonoBehaviour
        /// </summary>
        [Tooltip("SynchronizedSceneManager")]
        [SerializeField]
        protected SynchronizedSceneManager synchronizedSceneManager;

        /// <summary>
        /// Broadcaster MonoBehaviour
        /// </summary>
        [Tooltip("Broadcaster MonoBehaviour")]
        [SerializeField]
        protected Broadcaster broadcaster;

        /// <summary>
        /// Observer MonoBehaviour
        /// </summary>
        [Tooltip("Observer MonoBehaviour")]
        [SerializeField]
        protected Observer observer;

        private void Start()
        {
            if (synchronizedSceneManager == null ||
                broadcaster == null ||
                observer == null)
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
                Camera.main.transform.localPosition = Observer.Instance.transform.position;
                Camera.main.transform.localRotation = Observer.Instance.transform.rotation;
            }
        }
#endif

        private void RunAsBroadcaster()
        {
            broadcaster.gameObject.SetActive(true);
            observer.gameObject.SetActive(false);

            // The SynchronizedSceneManager needs to be enabled after the broadcaster/observer
            synchronizedSceneManager.gameObject.SetActive(true);
        }

        private void RunAsObserver()
        {
            broadcaster.gameObject.SetActive(false);
            observer.gameObject.SetActive(true);

            // The SynchronizedSceneManager needs to be enabled after the broadcaster/observer
            synchronizedSceneManager.gameObject.SetActive(true);

            // Make sure the SynchronizedSceneManger is enabled prior to connecting the observer
            observer.ConnectTo(broadcasterIpAddress);
        }
    }
}
