// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.HolographicCamera
{
    [RequireComponent(typeof(TCPConnectionManager))]
    public class HolographicCameraNetworkListener : MonoBehaviour
    {
        [SerializeField]
        private int listeningPort = 7502;

        private TCPConnectionManager tcpConnectionManager;

        private void Awake()
        {
            tcpConnectionManager = GetComponent<TCPConnectionManager>();
            tcpConnectionManager.StartListening(listeningPort);
        }
    }
}