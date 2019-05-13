// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class SynchronizedGameObjectHierarchy : MonoBehaviour
    {
        private SynchronizedTransform synchronizedTransform;

        private void Start()
        {
            if (SynchronizedClient.IsInitialized)
            {
                SynchronizedClient.Instance.Connected += OnConnected;

                // If we have connted to other devices, make sure we immediately
                // add a new SynchronizedTransform.
                if (SynchronizedClient.Instance.HasConnections)
                {
                    OnConnected(null);
                }
            }
        }

        private void OnDestroy()
        {
            if (SynchronizedClient.IsInitialized && SynchronizedClient.Instance != null)
            {
                SynchronizedClient.Instance.Connected -= OnConnected;
            }
        }

        private void OnConnected(SocketEndpoint endpoint)
        {
            if (synchronizedTransform != null)
            {
                Destroy(synchronizedTransform);
            }
            synchronizedTransform = this.gameObject.EnsureComponent<SynchronizedTransform>();
        }
    }
}