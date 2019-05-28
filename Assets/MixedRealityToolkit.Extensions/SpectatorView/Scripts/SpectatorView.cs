// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

using System.Runtime.CompilerServices;
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

        /// <summary>
        /// User ip address
        /// </summary>
        [Tooltip("User ip address")]
        [SerializeField]
        private string userIpAddress = "127.0.0.1";

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
        private GameObjectHierarchyBroadcaster broadcastedContent = null;

        /// <summary>
        /// Parent of the main camera, spatial coordinate system transforms will be applied to this game object.
        /// </summary>
        [Tooltip("Parent of the main camera, spatial coordinate system transforms will be applied to this game object.")]
        [SerializeField]
        public GameObject parentOfMainCamera = null;

        private void Awake()
        {
            Debug.Log($"SpectatorView is running as: {Role.ToString()}. Expected User IPAddress: {userIpAddress}");

            if (stateSynchronizationSceneManager == null ||
                stateSynchronizationBroadcaster == null ||
                stateSynchronizationObserver == null ||
                broadcastedContent == null ||
                parentOfMainCamera == null)
            {
                Debug.LogError("StateSynchronization scene isn't configured correctly");
                return;
            }

            SpatialCoordinateSystemManager.Instance.transformedGameObject = parentOfMainCamera;

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
        }

        private void RunStateSynchronizationAsBroadcaster()
        {
            broadcastedContent.gameObject.SetActive(true);
            stateSynchronizationBroadcaster.gameObject.SetActive(true);
            stateSynchronizationObserver.gameObject.SetActive(false);

            // The StateSynchronizationSceneManager needs to be enabled after the broadcaster/observer
            stateSynchronizationSceneManager.gameObject.SetActive(true);
        }

        private void RunStateSynchronizationAsObserver()
        {
            // All content in the observer scene should be dynamically setup/created, so we hide scene content here
            broadcastedContent.gameObject.SetActive(false);

            stateSynchronizationBroadcaster.gameObject.SetActive(false);
            stateSynchronizationObserver.gameObject.SetActive(true);

            // The StateSynchronizationSceneManager needs to be enabled after the broadcaster/observer
            stateSynchronizationSceneManager.gameObject.SetActive(true);

            // Make sure the StateSynchronizationSceneManager is enabled prior to connecting the observer
            stateSynchronizationObserver.ConnectTo(userIpAddress);
        }
    }
}
