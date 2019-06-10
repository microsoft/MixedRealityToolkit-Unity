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
using UnityEngine.XR.WSA;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// The SpectatorView helper class for managing a participant in the spatial coordinate system
    /// </summary>
    internal class SpatialCoordinateSystemParticipant : MonoBehaviour
    {
        byte[] previousCoordinateStatusMessage = null;
        private ISpatialCoordinate coordinate;

        public SocketEndpoint SocketEndpoint { get; set; }

        public ISpatialCoordinate Coordinate
        {
            get => coordinate;
            set
            {
                if (coordinate != value)
                {
                    coordinate = value;
                }
            }
        }

        public bool IsLocatingSpatialCoordinate { get; set; }

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

        public string PersistentCoordinateId { get; internal set; }

        private void Update()
        {
            if (Coordinate != null && Coordinate.State == LocatedState.Tracking)
            {
                this.transform.position = Coordinate.CoordinateToWorldSpace(Vector3.zero);
                this.transform.rotation = Coordinate.CoordinateToWorldSpace(Quaternion.identity);
            }

            if (SocketEndpoint != null && SocketEndpoint.IsConnected)
            {
                SendCoordinateStateMessage();
            }
        }

        private void SendCoordinateStateMessage()
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(stream))
            {
                message.Write(SpatialCoordinateSystemManager.CoordinateStateMessageHeader);
                message.Write(WorldManager.state == PositionalLocatorState.Active);
                message.Write(Coordinate != null && Coordinate.State == LocatedState.Tracking);
                message.Write(IsLocatingSpatialCoordinate);
                message.Write(transform.position);
                message.Write(transform.rotation);

                byte[] newCoordinateStatusMessage = stream.ToArray();
                if (previousCoordinateStatusMessage == null || !previousCoordinateStatusMessage.SequenceEqual(newCoordinateStatusMessage))
                {
                    previousCoordinateStatusMessage = newCoordinateStatusMessage;
                    SocketEndpoint.Send(newCoordinateStatusMessage);
                }
            }
        }
    }
}
