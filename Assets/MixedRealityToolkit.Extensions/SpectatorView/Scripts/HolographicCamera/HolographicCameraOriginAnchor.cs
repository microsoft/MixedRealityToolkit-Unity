// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.WorldAnchors;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_WSA
using UnityEngine.XR.WSA;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.HolographicCamera
{
    [RequireComponent(typeof(WorldAnchorManager))]
    [RequireComponent(typeof(TCPConnectionManager))]
    public class HolographicCameraOriginAnchor : MonoBehaviour
    {
#if UNITY_WSA
        private const string AnchorName = "WorldRoot";
        private WorldAnchorManager worldAnchorManager;
        private TCPConnectionManager tcpConnectionManager;

        public bool IsAnchorLocated
        {
            get
            {
                WorldAnchor anchor = GetComponent<WorldAnchor>();
                return anchor != null && anchor.isLocated;
            }
        }

        private void Awake()
        {
            worldAnchorManager = GetComponent<WorldAnchorManager>();
            worldAnchorManager.AttachAnchor(gameObject, AnchorName);

            tcpConnectionManager = GetComponent<TCPConnectionManager>();
            tcpConnectionManager.OnReceive += TcpConnectionManager_OnReceive;
        }

        private void OnDestroy()
        {
            tcpConnectionManager.OnReceive -= TcpConnectionManager_OnReceive;
        }

        private void TcpConnectionManager_OnReceive(IncomingMessage data)
        {
            using (MemoryStream stream = new MemoryStream(data.Data))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                string command = reader.ReadString();

                switch (command)
                {
                    case "CreateSharedAnchor":
                        {
                        }
                        break;
                }
            }
        }
#endif
    }
}