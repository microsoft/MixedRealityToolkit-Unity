// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor;
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
    /// <summary>
    /// Represents an object that can detect an ArUco marker and persist and restore a WorldAnchor
    /// to persist the location of that marker across sessions.
    /// </summary>
    public class ArUcoMarkerAnchor : MonoBehaviour
    {
        [SerializeField]
        private GameObject networkManagerGameObject = null;

#if UNITY_WSA
        private const int markerOriginId = 0;
        private const string AnchorName = "WorldRoot";
        private INetworkManager networkManager;
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

        public void HandleCreateSharedSpatialCoordinateCommand(SocketEndpoint endpoint, string command, BinaryReader reader, int remainingDataSize)
        {
            float markerDistance = reader.ReadSingle();

            if (!isDetectingMarker)
            {
                isDetectingMarker = true;
                WorldAnchorManager.Instance.RemoveAnchor(gameObject);

                markerDetector.SetMarkerSize(markerDistance);
                markerDetector.StartDetecting();
            }
        }

        private void Awake()
        {
            networkManager = networkManagerGameObject.GetComponent<INetworkManager>();
            networkManager.RegisterCommandHandler(LocatableDeviceObserver.CreateSharedSpatialCoordinateCommand, HandleCreateSharedSpatialCoordinateCommand);

            markerDetector = GetComponent<IMarkerDetector>();
            markerDetector.MarkersUpdated += MarkerDetector_MarkersUpdated;
        }

        private void Start()
        {
            if (!WorldAnchorManager.IsInitialized)
            {
                Debug.LogError("No WorldAnchorManager is available for persisting and restoring anchors");
                return;
            }

            WorldAnchorManager.Instance.AttachAnchor(gameObject, AnchorName);
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
                WorldAnchorManager.Instance.AttachAnchor(gameObject, AnchorName);
            }
        }

        private void OnDestroy()
        {
            networkManager.UnregisterCommandHandler(LocatableDeviceObserver.CreateSharedSpatialCoordinateCommand, HandleCreateSharedSpatialCoordinateCommand);
            markerDetector.MarkersUpdated -= MarkerDetector_MarkersUpdated;

        }
#endif
    }
}