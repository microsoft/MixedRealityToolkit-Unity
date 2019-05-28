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
    internal class SpatialCoordinateSystemMember : DisposableBase
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationToken cancellationToken;

        private readonly bool isHost;
        private readonly SocketEndpoint socketEndpoint;
        private readonly Func<GameObject> createSpatialCoordinateGO;
        private readonly bool debugLogging;
        private bool showDebugVisuals = false;
        private GameObject debugVisual = null;
        private float debugVisualScale = 1.0f;

        private Action<string, BinaryReader> processIncomingMessages = null;
        private GameObject spatialCoordinateGO = null;

        /// <summary>
        /// Instantiates a new <see cref="SpatialCoordinateSystemMember"/>.
        /// </summary>
        /// <param name="actAsHost">If True, this instance is in charge of hosting the spatial coordinate system.</param>
        /// <param name="socketEndpoint">The endpoint of the other entity.</param>
        /// <param name="createSpatialCoordinateGO">The function that creates a spatial coordinate game object on detection<see cref="GameObject"/>.</param>
        /// <param name="debugLogging">Flag for enabling troubleshooting logging.</param>
        public SpatialCoordinateSystemMember(bool actAsHost, SocketEndpoint socketEndpoint, Func<GameObject> createSpatialCoordinateGO, bool debugLogging, bool showDebugVisuals = false, GameObject debugVisual = null, float debugVisualScale = 1.0f)
        {
            cancellationToken = cancellationTokenSource.Token;

            this.isHost = actAsHost;
            this.socketEndpoint = socketEndpoint;
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
                Debug.Log($"SpatialCoordinateSystemMember [isHost: {isHost} - Connection: {socketEndpoint.Address}]: {message}");
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
                    processIncomingMessages = (command, reader) =>
                    {
                        DebugLog("Passing on incoming message");
                        if(!TryProcessIncomingMessage(command, reader))
                        {
                            spatialLocalizer.ProcessIncomingMessage(isHost, token, command, reader);
                        }
                    };
                }

                DebugLog("Telling LocalizationMechanims to begin localizng");
                ISpatialCoordinate coordinate = await spatialLocalizer.LocalizeAsync(isHost, token, WriteAndSendMessage, cancellationToken);

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
        public void ReceiveMessage(string command, BinaryReader reader)
        {
            lock (cancellationTokenSource)
            {
                processIncomingMessages?.Invoke(command, reader);
            }
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
            return false;
        }

        private void WriteAndSendMessage(Action<BinaryWriter> callToWrite)
        {
            DebugLog("Sending message to connected client");
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                // Allow the spatialLocalizer to write its own content to the binary writer with this function
                callToWrite(writer);

                socketEndpoint.Send(memoryStream.ToArray());
                DebugLog("Sent Message");
            }
        }
    }
}
