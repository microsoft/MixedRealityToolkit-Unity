// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class GameObjectHierarchyBroadcaster : MonoBehaviour
    {
        private TransformBroadcaster TransformBroadcaster;

        private void Start()
        {
            if (Broadcaster.IsInitialized)
            {
                Broadcaster.Instance.Connected += OnConnected;

                // If we have connted to other devices, make sure we immediately
                // add a new TransformBroadcaster.
                if (Broadcaster.Instance.HasConnections)
                {
                    OnConnected(null);
                }
            }
        }

        private void OnDestroy()
        {
            if (Broadcaster.IsInitialized && Broadcaster.Instance != null)
            {
                Broadcaster.Instance.Connected -= OnConnected;
            }
        }

        private void OnConnected(SocketEndpoint endpoint)
        {
            if (TransformBroadcaster != null)
            {
                Destroy(TransformBroadcaster);
            }
            TransformBroadcaster = this.gameObject.EnsureComponent<TransformBroadcaster>();
        }
    }
}