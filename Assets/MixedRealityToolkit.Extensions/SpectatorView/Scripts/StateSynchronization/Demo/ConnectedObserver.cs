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
    /// The SpectatorView helper class for managing the connected observer.
    /// </summary>
    internal class ConnectedObserver : DisposableBase
    {
        /// <summary>
        /// Const string to be used for sending networking messages.
        /// </summary>
        public const string SpatialLocalizationMessageHeader = "LOCALIZE";

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationToken cancellationToken;

        private readonly Role role;
        private readonly SocketEndpoint socketEndpoint;
        private readonly Func<GameObject> createAnchor;
        private readonly bool debugLogging;

        private Action<BinaryReader> processIncomingMessages = null;
        private GameObject observerGO = null;

        /// <summary>
        /// Instantiates a new <see cref="ConnectedObserver"/>.
        /// </summary>
        /// <param name="role">The role of the current device (is it a Broadcaster adding a connected observer to it's list).</param>
        /// <param name="socketEndpoint">The endpoint of the other entity.</param>
        /// <param name="createAnchor">The function to create the anchor <see cref="GameObject"/>.</param>
        /// <param name="debugLogging">Flag for enabling troubleshooting logging.</param>
        public ConnectedObserver(Role role, SocketEndpoint socketEndpoint, Func<GameObject> createAnchor, bool debugLogging)
        {
            cancellationToken = cancellationTokenSource.Token;

            this.role = role;
            this.socketEndpoint = socketEndpoint;
            this.createAnchor = createAnchor;
            this.debugLogging = debugLogging;
        }

        private void DebugLog(string message)
        {
            if (debugLogging)
            {
                Debug.Log($"ConnectedObserver [{role} - Connection: {socketEndpoint.Address}]: {message}");
            }
        }

        /// <summary>
        /// Localizes this observer with the connected party using the given mechanism.
        /// </summary>
        /// <param name="localizationMechanism">The mechanism to use for localization.</param>
        public async Task LocalizeAsync(SpatialLocalizationMechanismBase localizationMechanism)
        {
            DebugLog("Started LocalizeAsync");

            DebugLog("Initializing with LocalizationMechanism.");
            // Tell the localization mechanism to initialize, this could create anchor if need be
            Guid token = await localizationMechanism.InitializeAsync(role, cancellationToken);
            DebugLog("Initialized with LocalizationMechanism");

            try
            {
                lock (cancellationTokenSource)
                {
                    processIncomingMessages = r =>
                    {
                        DebugLog("Passing on incoming message");
                        localizationMechanism.ProcessIncomingMessage(role, token, r);
                    };
                }

                DebugLog("Telling LocalizationMechanims to begin localizng");
                ISpatialCoordinate coordinate = await localizationMechanism.LocalizeAsync(role, token, (callback) => // Allows localization mechanism to tell spectator what to do
                {
                    DebugLog("Sending message to connected client");
                    using (MemoryStream memoryStream = new MemoryStream())
                    using (BinaryWriter writer = new BinaryWriter(memoryStream))
                    {
                        writer.Write(SpatialLocalizationMessageHeader);

                        callback(writer);
                        socketEndpoint.Send(memoryStream.ToArray());
                        DebugLog("Sent Message");
                    }
                }, cancellationToken);

                if (coordinate == null)
                {
                    Debug.LogError($"Failed to localize for spectator: {socketEndpoint.Address}");
                }
                else
                {
                    DebugLog("Creating Visual Anchor");
                    lock (cancellationTokenSource)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            observerGO = createAnchor();
                            observerGO.AddComponent<SpatialCoordinateLocalizer>().Coordinate = coordinate;
                            DebugLog("Anchor created, coordinate set");
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
                localizationMechanism.Uninitialize(role, token);
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

        protected override void OnManagedDispose()
        {
            base.OnManagedDispose();

            DebugLog("Disposed");

            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();


            lock (cancellationTokenSource)
            {
                UnityEngine.Object.Destroy(observerGO);
            }
        }
    }
}
