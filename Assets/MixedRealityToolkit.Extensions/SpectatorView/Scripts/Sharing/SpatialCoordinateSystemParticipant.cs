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

        public SocketEndpoint SocketEndpoint { get; set; }

        public ISpatialCoordinate Coordinate { get; set; }

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

        //private const string CoordinateStateMessageHeader = "COORDSTATE";
        //private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        //private readonly CancellationToken cancellationToken;

        //private readonly SocketEndpoint socketEndpoint;
        //private readonly Func<GameObject> createSpatialCoordinateGO;

        //private bool validLocalOrigin = false;
        //private Vector3 localOriginPositionInCoordinateSystem = Vector3.zero;
        //private Quaternion localOriginRotationInCoordinateSystem = Quaternion.identity;
        //private bool validRemoteOrigin = false;
        //private Vector3 remoteOriginPositionInCoordinateSystem = Vector3.zero;
        //private Quaternion remoteOriginRotationInCoordinateSystem = Quaternion.identity;

        //private readonly bool debugLogging;
        //private bool showDebugVisuals = false;
        //private GameObject debugVisual = null;
        //private float debugVisualScale = 1.0f;

        //private Action<SocketEndpoint, Action<BinaryWriter>> writeAndSend = null;
        //private Action<BinaryReader> processIncomingMessages = null;
        //private GameObject spatialCoordinateGO = null;

        ///// <summary>
        ///// Instantiates a new <see cref="SpatialCoordinateSystemParticipant"/>.
        ///// </summary>
        ///// <param name="socketEndpoint">The endpoint of the other entity.</param>
        ///// <param name="createSpatialCoordinateGO">The function that creates a spatial coordinate game object on detection<see cref="GameObject"/>.</param>
        ///// <param name="debugLogging">Flag for enabling troubleshooting logging.</param>
        //public SpatialCoordinateSystemParticipant(SocketEndpoint socketEndpoint, Action<SocketEndpoint, Action<BinaryWriter>> writeAndSend, Func<GameObject> createSpatialCoordinateGO, bool debugLogging, bool showDebugVisuals = false, GameObject debugVisual = null, float debugVisualScale = 1.0f)
        //{
        //    cancellationToken = cancellationTokenSource.Token;

        //    this.socketEndpoint = socketEndpoint;
        //    this.writeAndSend = writeAndSend;
        //    this.createSpatialCoordinateGO = createSpatialCoordinateGO;
        //    this.debugLogging = debugLogging;
        //    this.showDebugVisuals = showDebugVisuals;
        //    this.debugVisual = debugVisual;
        //    this.debugVisualScale = debugVisualScale;
        //}

        //private void DebugLog(string message)
        //{
        //    if (debugLogging)
        //    {
        //        Debug.Log($"SpatialCoordinateSystemParticipant [Connection: {socketEndpoint.Address}]: {message}");
        //    }
        //}

        ///// <summary>
        ///// Localizes this observer with the connected party using the given mechanism.
        ///// </summary>
        ///// <param name="localizationMechanism">The mechanism to use for localization.</param>
        //public async Task LocalizeAsync(SpatialLocalizer spatialLocalizer)
        //{
        //    DebugLog("Started LocalizeAsync");

        //    DebugLog("Initializing with LocalizationMechanism.");
        //    // Tell the localization mechanism to initialize, this could create anchor if need be
        //    Guid token = await spatialLocalizer.InitializeAsync(isHost, cancellationToken);
        //    DebugLog("Initialized with LocalizationMechanism");

        //    try
        //    {
        //        lock (cancellationTokenSource)
        //        {
        //            processIncomingMessages = (reader) =>
        //            {
        //                DebugLog("Passing on incoming message");
        //                string command = reader.ReadString();
        //                if (!TryProcessIncomingMessage(command, reader))
        //                {
        //                    spatialLocalizer.ProcessIncomingMessage(isHost, token, command, reader);
        //                }
        //            };
        //        }

        //        DebugLog("Telling LocalizationMechanims to begin localizng");
        //        ISpatialCoordinate coordinate = await spatialLocalizer.LocalizeAsync(isHost, token, LocalizerWriteAndSend, cancellationToken);

        //        if (coordinate == null)
        //        {
        //            Debug.LogError($"Failed to localize for spectator: {socketEndpoint.Address}");
        //        }
        //        else
        //        {
        //            DebugLog("Creating Visual for spatial coordinate");
        //            lock (cancellationTokenSource)
        //            {
        //                if (!cancellationToken.IsCancellationRequested)
        //                {
        //                    validLocalOrigin = true;
        //                    localOriginPositionInCoordinateSystem = coordinate.WorldToCoordinateSpace(Vector3.zero);
        //                    localOriginRotationInCoordinateSystem = coordinate.WorldToCoordinateSpace(Quaternion.identity);

        //                    SendCoordinateState();

        //                    if (showDebugVisuals)
        //                    {
        //                        spatialCoordinateGO = createSpatialCoordinateGO();
        //                        var spatialCoordinateLocalizer = spatialCoordinateGO.AddComponent<SpatialCoordinateLocalizer>();
        //                        spatialCoordinateLocalizer.debugLogging = debugLogging;
        //                        spatialCoordinateLocalizer.showDebugVisuals = showDebugVisuals;
        //                        spatialCoordinateLocalizer.debugVisual = debugVisual;
        //                        spatialCoordinateLocalizer.debugVisualScale = debugVisualScale;
        //                        spatialCoordinateLocalizer.Coordinate = coordinate;
        //                        DebugLog("Spatial coordinate created, coordinate set");
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        lock (cancellationTokenSource)
        //        {
        //            processIncomingMessages = null;
        //        }

        //        DebugLog("Uninitializing.");
        //        spatialLocalizer.Uninitialize(isHost, token);
        //        DebugLog("Uninitialized.");
        //    }
        //}

        ///// <summary>
        ///// Handles messages received from the network.
        ///// </summary>
        ///// <param name="reader">The reader to access the contents of the message.</param>
        //public void ReceiveMessage(BinaryReader reader)
        //{
        //    lock (cancellationTokenSource)
        //    {
        //        processIncomingMessages?.Invoke(reader);
        //    }
        //}

        //public bool TryTransformToHostWorldSpace(Vector3 localWorldPosition, out Vector3 hostWorldPosition)
        //{
        //    hostWorldPosition = Vector3.zero;
        //    if (!validLocalOrigin || !validRemoteOrigin)
        //    {
        //        return false;
        //    }

        //    if (isHost)
        //    {
        //        hostWorldPosition = localWorldPosition;
        //    }
        //    else
        //    {
        //        DebugLog($"Local Origin Position: {localOriginPositionInCoordinateSystem.ToString("G4")}, Remote Origin Position: {remoteOriginPositionInCoordinateSystem.ToString("G4")}");
        //        hostWorldPosition = localWorldPosition + localOriginPositionInCoordinateSystem - remoteOriginPositionInCoordinateSystem;
        //    }

        //    return true;
        //}

        //public bool TryTransformToHostWorldSpace(Quaternion localWorldRotation, out Quaternion hostWorldRotation)
        //{
        //    hostWorldRotation = Quaternion.identity;
        //    if (!validLocalOrigin || !validRemoteOrigin)
        //    {
        //        return false;
        //    }

        //    if (isHost)
        //    {
        //        hostWorldRotation = localWorldRotation;
        //    }
        //    else
        //    {
        //        DebugLog($"Local Origin Rotation: {localOriginRotationInCoordinateSystem.ToString("G4")}, Remote Origin Rotation: {remoteOriginRotationInCoordinateSystem.ToString("G4")}");
        //        hostWorldRotation = Quaternion.Inverse(remoteOriginRotationInCoordinateSystem) * localOriginRotationInCoordinateSystem * localWorldRotation;
        //    }

        //    return true;
        //}

        //protected override void OnManagedDispose()
        //{
        //    base.OnManagedDispose();

        //    DebugLog("Disposed");

        //    cancellationTokenSource.Cancel();
        //    cancellationTokenSource.Dispose();

        //    lock (cancellationTokenSource)
        //    {
        //        UnityEngine.Object.Destroy(spatialCoordinateGO);
        //    }
        //}

        //private bool TryProcessIncomingMessage(string command, BinaryReader reader)
        //{
        //    if (command == CoordinateStateMessageHeader)
        //    {
        //        bool isRemoteHost = reader.ReadBoolean();
        //        if (isHost == isRemoteHost)
        //        {
        //            Debug.LogError("This spatial coordinate system participant is acting as hosts on multiple devices, which is an unexpected scenario");
        //        }
        //        else
        //        {
        //            validRemoteOrigin = reader.ReadBoolean();
        //            remoteOriginPositionInCoordinateSystem = reader.ReadVector3();
        //            remoteOriginRotationInCoordinateSystem = reader.ReadQuaternion();
        //            DebugLog($"Updated remote origin information: Valid: {validRemoteOrigin}, Position: {remoteOriginPositionInCoordinateSystem.ToString("G4")}, Rotation: {remoteOriginRotationInCoordinateSystem.ToString("G4")}");
        //        }

        //        // We return true even if an error occurs so that no other classes attempt to process this message.
        //        return true;
        //    }

        //    return false;
        //}

        //private void SendCoordinateState()
        //{
        //    if (writeAndSend == null)
        //    {
        //        Debug.LogWarning("Write and send message was not populated for SpatialCoordinateSystemParticipant");
        //        return;
        //    }

        //    writeAndSend(socketEndpoint, writer =>
        //    {
        //        DebugLog($"Sending coordinate state: isHost: {isHost}, validLocalOrigin: {validLocalOrigin}, Position: {localOriginPositionInCoordinateSystem.ToString("G4")}, Rotation: {localOriginRotationInCoordinateSystem.ToString("G4")}");
        //        writer.Write(CoordinateStateMessageHeader);
        //        writer.Write(isHost);
        //        writer.Write(validLocalOrigin);
        //        writer.Write(localOriginPositionInCoordinateSystem);
        //        writer.Write(localOriginRotationInCoordinateSystem);
        //    });
        //}

        //private void LocalizerWriteAndSend(Action<BinaryWriter> callToWrite)
        //{
        //    if (writeAndSend == null)
        //    {
        //        Debug.LogWarning("Write and send message was not populated for SpatialCoordinateSystemParticipant");
        //        return;
        //    }

        //    writeAndSend(socketEndpoint, callToWrite);
        //}
    }
}
