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

        public readonly LocalizerRole Role;
        public readonly SocketEndpoint SocketEndpoint;
        private readonly Func<GameObject> createSpatialCoordinateGO;
        private readonly bool debugLogging;
        private bool showDebugVisuals = false;
        private GameObject debugVisual = null;
        private float debugVisualScale = 1.0f;

        private GameObject spatialCoordinateGO = null;

        /// <summary>
        /// Instantiates a new <see cref="SpatialCoordinateSystemMember"/>.
        /// </summary>
        /// <param name="role">The role of the current device (is it a Broadcaster adding a connected observer to it's list).</param>
        /// <param name="socketEndpoint">The endpoint of the other entity.</param>
        /// <param name="createSpatialCoordinateGO">The function that creates a spatial coordinate game object on detection<see cref="GameObject"/>.</param>
        /// <param name="debugLogging">Flag for enabling troubleshooting logging.</param>
        public SpatialCoordinateSystemMember(LocalizerRole role, SocketEndpoint socketEndpoint, Func<GameObject> createSpatialCoordinateGO, bool debugLogging, bool showDebugVisuals = false, GameObject debugVisual = null, float debugVisualScale = 1.0f)
        {
            cancellationToken = cancellationTokenSource.Token;

            Role = role;
            SocketEndpoint = socketEndpoint;
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
                Debug.Log($"SpatialCoordinateSystemMember [{Role} - Connection: {SocketEndpoint.Address}]: {message}");
            }
        }

        /// <summary>
        /// Localizes this observer with the connected party using the given mechanism.
        /// </summary>
        /// <param name="localizationMechanism">The mechanism to use for localization.</param>
        public async Task LocalizeAsync(SpatialLocalizer spatialLocalizer, Action<SpatialCoordinateSystemMember, ISpatialCoordinate> onCoordinateLocalized)
        {
            DebugLog("Started LocalizeAsync");

            DebugLog("Initializing with LocalizationMechanism.");
            // Tell the localization mechanism to initialize, this could create anchor if need be
            Guid token = await spatialLocalizer.InitializeAsync(Role, cancellationToken);
            DebugLog("Initialized with LocalizationMechanism");

            try
            {
                DebugLog("Telling LocalizationMechanims to begin localizng");
                ISpatialCoordinate coordinate = await spatialLocalizer.LocalizeAsync(Role, token, cancellationToken);

                if (coordinate == null)
                {
                    Debug.LogError($"Failed to localize for spectator: {SocketEndpoint.Address}");
                }
                else
                {
                    DebugLog("Creating Visual for spatial coordinate");
                    lock (cancellationTokenSource)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            // Firing the coordinate localized callback if it exists
                            onCoordinateLocalized?.Invoke(this, coordinate);

                            spatialCoordinateGO = createSpatialCoordinateGO();
                            if (spatialCoordinateGO != null)
                            {
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
            }
            finally
            {
                DebugLog("Uninitializing.");
                spatialLocalizer.Uninitialize(Role, token);
                DebugLog("Uninitialized.");
            }
        }

        /// <summary>
        /// Handles messages received from the network.
        /// </summary>
        /// <param name="reader">The reader to access the contents of the message.</param>
        public bool TrySetCoordinateIdForLocalizer(SpatialLocalizer spatialLocalizer, string coordinateId)
        {
            return spatialLocalizer.TrySetCoordinateId(Role, Guid.Empty, coordinateId);
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
    }
}
