// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// The SpectatorView helper class for managing a participant in the spatial coordinate system
    /// </summary>
    internal class SpatialCoordinateSystemParticipant : DisposableBase
    {
        private const string CoordinateStateMessageHeader = "COORDSTATE";
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationToken cancellationToken;

        private readonly bool isHost;
        private readonly SocketEndpoint socketEndpoint;
        private readonly Func<GameObject> createSpatialCoordinateGO;

        private bool validLocalOrigin = false;
        private Vector3 localOriginPositionInCoordinateSystem = Vector3.zero;
        private Quaternion localOriginRotationInCoordinateSystem = Quaternion.identity;
        private bool validRemoteOrigin = false;
        private Vector3 remoteOriginPositionInCoordinateSystem = Vector3.zero;
        private Quaternion remoteOriginRotationInCoordinateSystem = Quaternion.identity;

        private readonly bool debugLogging;
        private bool showDebugVisuals = false;
        private GameObject debugVisual = null;
        private float debugVisualScale = 1.0f;

        private Action<SocketEndpoint, Action<BinaryWriter>> writeAndSend = null;
        private Action<BinaryReader> processIncomingMessages = null;
        private GameObject spatialCoordinateGO = null;

        /// <summary>
        /// Instantiates a new <see cref="SpatialCoordinateSystemParticipant"/>.
        /// </summary>
        /// <param name="actAsHost">If true, this instance will assume that it is responsible for hosting the spatial coordinate system.</param>
        /// <param name="socketEndpoint">The endpoint of the other entity.</param>
        /// <param name="createSpatialCoordinateGO">The function that creates a spatial coordinate game object on detection<see cref="GameObject"/>.</param>
        /// <param name="debugLogging">Flag for enabling troubleshooting logging.</param>
        public SpatialCoordinateSystemParticipant(bool actAsHost, SocketEndpoint socketEndpoint, Action<SocketEndpoint, Action<BinaryWriter>> writeAndSend, Func<GameObject> createSpatialCoordinateGO, bool debugLogging, bool showDebugVisuals = false, GameObject debugVisual = null, float debugVisualScale = 1.0f)
        {
            cancellationToken = cancellationTokenSource.Token;

            isHost = actAsHost;
            this.socketEndpoint = socketEndpoint;
            this.writeAndSend = writeAndSend;
            this.createSpatialCoordinateGO = createSpatialCoordinateGO;
            this.debugLogging = debugLogging;
            this.showDebugVisuals = showDebugVisuals;
            this.debugVisual = debugVisual;
            this.debugVisualScale = debugVisualScale;
        }

        private void DebugLog(string message)
        {
            if (debugLogging)
            {
                Debug.Log($"SpatialCoordinateSystemParticipant [isHost: {isHost} - Connection: {socketEndpoint.Address}]: {message}");
            }
        }

        /// <summary>
        /// Localizes this observer with the connected party using the given mechanism.
        /// </summary>
        /// <param name="localizationMechanism">The mechanism to use for localization.</param>
        public async Task LocalizeAsync(SpatialLocalizer spatialLocalizer)
        {
            DebugLog("Started LocalizeAsync");

            DebugLog("Initializing with LocalizationMechanism.");
            // Tell the localization mechanism to initialize, this could create anchor if need be
            Guid token = await spatialLocalizer.InitializeAsync(isHost, cancellationToken);
            DebugLog("Initialized with LocalizationMechanism");

            try
            {
                lock (cancellationTokenSource)
                {
                    processIncomingMessages = (reader) =>
                    {
                        DebugLog("Passing on incoming message");
                        string command = reader.ReadString();
                        if (!TryProcessIncomingMessage(command, reader))
                        {
                            spatialLocalizer.ProcessIncomingMessage(isHost, token, command, reader);
                        }
                    };
                }

                DebugLog("Telling LocalizationMechanims to begin localizng");
                ISpatialCoordinate coordinate = await spatialLocalizer.LocalizeAsync(isHost, token, LocalizerWriteAndSend, cancellationToken);

                if (coordinate == null)
                {
                    Debug.LogError($"Failed to localize for spectator: {socketEndpoint.Address}");
                }
                else
                {
                    DebugLog("Creating Visual for spatial coordinate");
                    lock (cancellationTokenSource)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            validLocalOrigin = true;
                            localOriginPositionInCoordinateSystem = coordinate.WorldToCoordinateSpace(Vector3.zero);
                            localOriginRotationInCoordinateSystem = coordinate.WorldToCoordinateSpace(Quaternion.identity);

                            spatialCoordinateGO = createSpatialCoordinateGO();
                            var spatialCoordinateLocalizer = spatialCoordinateGO.AddComponent<SpatialCoordinateLocalizer>();
                            spatialCoordinateLocalizer.debugLogging = debugLogging;
                            spatialCoordinateLocalizer.showDebugVisuals = showDebugVisuals;
                            spatialCoordinateLocalizer.debugVisual = debugVisual;
                            spatialCoordinateLocalizer.debugVisualScale = debugVisualScale;
                            spatialCoordinateLocalizer.Coordinate = coordinate;
                            DebugLog("Spatial coordinate created, coordinate set");
                        }
                    }
                }
            }
            finally
            {
                lock (cancellationTokenSource)
                {
                    processIncomingMessages = null;
                }

                DebugLog("Uninitializing.");
                spatialLocalizer.Uninitialize(isHost, token);
                DebugLog("Uninitialized.");
            }
        }

        /// <summary>
        /// Handles messages received from the network.
        /// </summary>
        /// <param name="reader">The reader to access the contents of the message.</param>
        public void ReceiveMessage(BinaryReader reader)
        {
            lock (cancellationTokenSource)
            {
                processIncomingMessages?.Invoke(reader);
            }
        }

        public bool TryTransformToHostWorldSpace(Vector3 localWorldPosition, out Vector3 hostWorldPosition)
        {
            hostWorldPosition = Vector3.zero;
            if (!validLocalOrigin || !validRemoteOrigin)
            {
                return false;
            }

            if (isHost)
            {
                hostWorldPosition = localWorldPosition;
            }
            else
            {
                hostWorldPosition = localWorldPosition + localOriginPositionInCoordinateSystem - remoteOriginPositionInCoordinateSystem;
            }

            return true;
        }

        public bool TryTransformToHostWorldSpace(Quaternion localWorldRotation, out Quaternion hostWorldRotation)
        {
            hostWorldRotation = Quaternion.identity;
            if (!validLocalOrigin || !validRemoteOrigin)
            {
                return false;
            }

            if (isHost)
            {
                hostWorldRotation = localWorldRotation;
            }
            else
            {
                hostWorldRotation = localWorldRotation * localOriginRotationInCoordinateSystem * Quaternion.Inverse(remoteOriginRotationInCoordinateSystem);
            }

            return true;
        }

        protected override void OnManagedDispose()
        {
            base.OnManagedDispose();

            DebugLog("Disposed");

            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();

            lock (cancellationTokenSource)
            {
                UnityEngine.Object.Destroy(spatialCoordinateGO);
            }
        }

        private bool TryProcessIncomingMessage(string command, BinaryReader reader)
        {
            if (command == CoordinateStateMessageHeader)
            {
                bool isRemoteHost = reader.ReadBoolean();
                if (isHost == isRemoteHost)
                {
                    Debug.LogError("This spatial coordinate system participant is acting as hosts on multiple devices, which is an unexpected scenario");
                }
                else
                {
                    validRemoteOrigin = reader.ReadBoolean();
                    remoteOriginPositionInCoordinateSystem = reader.ReadVector3();
                    remoteOriginRotationInCoordinateSystem = reader.ReadQuaternion();
                    DebugLog($"Updated remote origin information: Valid: {validRemoteOrigin}, Position: {remoteOriginPositionInCoordinateSystem.ToString("G4")}, Rotation: {remoteOriginRotationInCoordinateSystem.ToString("G4")}");
                }

                // We return true even if an error occurs so that no other classes attempt to process this message.
                return true;
            }

            return false;
        }

        private void SendCoordinateState()
        {
            if (writeAndSend == null)
            {
                Debug.LogWarning("Write and send message was not populated for SpatialCoordinateSystemParticipant");
                return;
            }

            writeAndSend(socketEndpoint, writer =>
            {
                writer.Write(CoordinateStateMessageHeader);
                writer.Write(isHost);
                writer.Write(validLocalOrigin);
                writer.Write(localOriginPositionInCoordinateSystem);
                writer.Write(localOriginRotationInCoordinateSystem);
            });
        }

        private void LocalizerWriteAndSend(Action<BinaryWriter> callToWrite)
        {
            if (writeAndSend == null)
            {
                Debug.LogWarning("Write and send message was not populated for SpatialCoordinateSystemParticipant");
                return;
            }

            writeAndSend(socketEndpoint, callToWrite);
        }
    }
}
