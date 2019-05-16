// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
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
        private const int markerOriginId = 0;
        private const string AnchorName = "WorldRoot";
        private WorldAnchorManager worldAnchorManager;
        private TCPConnectionManager tcpConnectionManager;
        private IMarkerDetector markerDetector;
        private bool isDetectingMarker;

        public bool IsDetectingMarker => isDetectingMarker;

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

            markerDetector = GetComponent<IMarkerDetector>();
            markerDetector.MarkersUpdated += MarkerDetector_MarkersUpdated;
        }

        private void MarkerDetector_MarkersUpdated(Dictionary<int, Marker> markers)
        {
            Marker originMarker;
            if (markers.TryGetValue(markerOriginId, out originMarker))
            {
                isDetectingMarker = false;
                markerDetector.StopDetecting();

                transform.position = originMarker.Position;
                transform.rotation = originMarker.Rotation;
                worldAnchorManager.AttachAnchor(gameObject, AnchorName);
            }
        }

        private void OnDestroy()
        {
            tcpConnectionManager.OnReceive -= TcpConnectionManager_OnReceive;
            markerDetector.MarkersUpdated -= MarkerDetector_MarkersUpdated;

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
                            float markerDistance = reader.ReadSingle();

                            if (!isDetectingMarker)
                            {
                                isDetectingMarker = true;
                                worldAnchorManager.RemoveAnchor(gameObject);

                                markerDetector.SetMarkerSize(markerDistance);
                                markerDetector.StartDetecting();
                            }
                        }
                        break;
                }
            }
        }
#endif
    }
}