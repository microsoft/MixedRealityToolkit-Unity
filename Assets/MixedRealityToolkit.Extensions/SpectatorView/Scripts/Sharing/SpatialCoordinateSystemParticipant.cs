// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// The SpectatorView helper class for managing a participant in the spatial coordinate system
    /// </summary>
    public class SpatialCoordinateSystemParticipant : DisposableBase, IPeerConnection
    {
        internal const string LocalizationDataExchangeCommand = "LocalizationDataExchange";
        private readonly GameObject debugVisualPrefab;
        private readonly float debugVisualScale;
        private byte[] previousCoordinateStatusMessage = null;
        private ISpatialCoordinate coordinate;
        private GameObject debugVisual;
        private SpatialCoordinateLocalizer debugCoordinateLocalizer;
        private bool showDebugVisuals;

        public SocketEndpoint SocketEndpoint { get; }

        public SpatialCoordinateSystemParticipant(SocketEndpoint endpoint, GameObject debugVisualPrefab, float debugVisualScale)
        {
            this.debugVisualPrefab = debugVisualPrefab;
            this.debugVisualScale = debugVisualScale;
            SocketEndpoint = endpoint;
        }

        public ISpatialCoordinate Coordinate
        {
            get => coordinate;
            set
            {
                if (coordinate != value)
                {
                    coordinate = value;

                    if (debugCoordinateLocalizer != null)
                    {
                        debugCoordinateLocalizer.Coordinate = coordinate;
                    }
                }
            }
        }

        public bool ShowDebugVisuals
        {
            get { return showDebugVisuals; }
            set
            {
                if (showDebugVisuals != value)
                {
                    showDebugVisuals = value;

                    if (debugVisual == null)
                    {
                        debugVisual = GameObject.Instantiate(debugVisualPrefab);
                        debugVisual.transform.localScale = Vector3.one * debugVisualScale;
                        debugCoordinateLocalizer = debugVisual.AddComponent<SpatialCoordinateLocalizer>();
                        debugCoordinateLocalizer.Coordinate = Coordinate;
                    }

                    debugVisual.SetActive(showDebugVisuals);
                }
            }
        }

        public bool IsLocatingSpatialCoordinate => CurrentLocalizationSession != null;

        /// <summary>
        /// Gets the last-reported tracking status of the peer device.
        /// </summary>
        public bool PeerDeviceHasTracking { get; internal set; }

        /// <summary>
        /// Gets the last-reported status of whether or not the peer's spatial coordinate is located and tracking.
        /// </summary>
        public bool PeerSpatialCoordinateIsLocated { get; internal set; }

        /// <summary>
        /// Gets whether or not the peer device is actively attempting to locate the shared spatial coordinate.
        /// </summary>
        public bool PeerIsLocatingSpatialCoordinate { get; internal set; }

        /// <summary>
        /// Gets the position of the shared spatial coordinate in the peer device's world space.
        /// </summary>
        public Vector3 PeerSpatialCoordinateWorldPosition { get; internal set; }

        /// <summary>
        /// Gets the rotation of the shared spatial coordinate in the peer device's world space.
        /// </summary>
        public Quaternion PeerSpatialCoordinateWorldRotation { get; internal set; }

        /// <summary>
        /// Gets the currently-running localization session for this participant;
        /// </summary>
        public ISpatialLocalizationSession CurrentLocalizationSession { get; internal set; }

        public void CheckForStateChanges()
        {
            if (SocketEndpoint != null && SocketEndpoint.IsConnected)
            {
                SendCoordinateStateMessage();
            }
        }

        protected override void OnManagedDispose()
        {
            base.OnManagedDispose();

            if (debugVisual != null)
            {
                GameObject.Destroy(debugVisual);
            }
        }

        private void SendCoordinateStateMessage()
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(stream))
            {
                message.Write(SpatialCoordinateSystemManager.CoordinateStateMessageHeader);
#if UNITY_WSA
                bool isTracking = UnityEngine.XR.WSA.WorldManager.state == UnityEngine.XR.WSA.PositionalLocatorState.Active;
#else
                // For Android, this should refer to GoogleARCore.Session.Status == GoogleARCore.SessionStatus.Tracking, but that requires
                // an eventual dependency between SpectatorView and GoogleARCore. For now, always report that tracking is enabled
                // for other platforms.
                bool isTracking = true;
#endif
                message.Write(isTracking);
                message.Write(Coordinate != null && (Coordinate.State == LocatedState.Tracking || Coordinate.State == LocatedState.Resolved));
                message.Write(IsLocatingSpatialCoordinate);

                Vector3 position = Vector3.zero;
                Quaternion rotation = Quaternion.identity;
                if (Coordinate != null)
                {
                    position = Coordinate.CoordinateToWorldSpace(Vector3.zero);
                    rotation = Coordinate.CoordinateToWorldSpace(Quaternion.identity);
                }

                message.Write(position);
                message.Write(rotation);

                byte[] newCoordinateStatusMessage = stream.ToArray();
                if (previousCoordinateStatusMessage == null || !previousCoordinateStatusMessage.SequenceEqual(newCoordinateStatusMessage))
                {
                    previousCoordinateStatusMessage = newCoordinateStatusMessage;
                    SocketEndpoint.Send(newCoordinateStatusMessage);
                }
            }
        }

        public void SendData(Action<BinaryWriter> writeCallback)
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(LocalizationDataExchangeCommand);
                writeCallback(writer);

                SocketEndpoint.Send(stream.ToArray());
            }
        }

        internal void ReadCoordinateStateMessage(BinaryReader reader)
        {
            PeerDeviceHasTracking = reader.ReadBoolean();
            PeerSpatialCoordinateIsLocated = reader.ReadBoolean();
            PeerIsLocatingSpatialCoordinate = reader.ReadBoolean();
            PeerSpatialCoordinateWorldPosition = reader.ReadVector3();
            PeerSpatialCoordinateWorldRotation = reader.ReadQuaternion();
        }
    }
}
