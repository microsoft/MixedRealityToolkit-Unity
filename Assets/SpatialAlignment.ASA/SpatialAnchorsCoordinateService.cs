// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if SPATIALALIGNMENT_ASA
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.AzureSpatialAnchors
{
    /// <summary>
    /// Base class for the various platform implementation sof Azure Spatial Anchors.
    /// </summary>
    public abstract class SpatialAnchorsCoordinateService : SpatialCoordinateServiceUnityBase<string>
    {
        private readonly object lockObj = new object();
        private readonly SpatialAnchorsConfiguration spatialAnchorsConfiguration;
        private readonly SynchronizationContext gameThreadSynchronizationContext;

        protected readonly TaskCompletionSource<bool> readyForCreate = new TaskCompletionSource<bool>();
        protected readonly TaskCompletionSource<bool> recommendedForCreate = new TaskCompletionSource<bool>();

        private Task initializationTask = null;
        protected CloudSpatialAnchorSession session;
        private int requestsForSessionStart = 0;

        public SpatialAnchorsCoordinateService(SpatialAnchorsConfiguration spatialAnchorsConfiguration)
        {
            this.spatialAnchorsConfiguration = spatialAnchorsConfiguration;
            gameThreadSynchronizationContext = SynchronizationContext.Current;
        }

        /// <inheritdoc/>
        protected override void OnManagedDispose()
        {
            base.OnManagedDispose();

            session?.Stop();
            session?.Dispose();
            session = null;


        }

        /// <summary>
        /// This method must be pumped each frame from the Untiy Game loop.
        /// </summary>
        public void FrameUpdate()
        {
            ThrowIfDisposed();

            OnFrameUpdate();
        }

        /// <summary>
        /// Overridable function providing frame update pump.
        /// </summary>
        protected virtual void OnFrameUpdate() { }

        private void RequestSessionStart()
        {
            lock (session)
            {
                if (requestsForSessionStart == 0)
                {
                    session.Start();
                }

                requestsForSessionStart++;
            }
        }

        private void ReleaseSessionStartRequest()
        {
            lock (session)
            {
                if (requestsForSessionStart == 1)
                {
                    session.Stop();
                }

                requestsForSessionStart = Math.Max(requestsForSessionStart - 1, 0);
            }
        }

        /// <inheritdoc/>
        public override async Task<bool> TryDeleteCoordinateAsync(string id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            await EnsureInitializedAsync().Unless(cancellationToken);
            RequestSessionStart();

            try
            {
                SpatialAnchorsCoordinate spatialAnchorsCoordinate;
                if (!knownCoordinates.TryGetValue(id, out ISpatialCoordinate spatialCoordinate))
                {
                    return false;
                }

                spatialAnchorsCoordinate = (SpatialAnchorsCoordinate)spatialCoordinate;

                await session.DeleteAnchorAsync(spatialAnchorsCoordinate.CloudSpatialAnchor);

                // Dispose in case of success only
                using (spatialAnchorsCoordinate)
                {
                    OnRemoveCoordinate(id);
                    return true;
                }

            }
            finally
            {
                ReleaseSessionStartRequest();
            }
        }

        /// <summary>
        /// Creates a <see cref="GameObject"/> representing the anchor based on provided <see cref="AnchorLocatedEventArgs"/>.
        /// </summary>
        /// <param name="args">Args passed from AnchorLocated event.</param>
        /// <returns>The newly created <see cref="GameObject"/>.</returns>
        protected virtual GameObject CreateGameObjectFrom(AnchorLocatedEventArgs args)
        {
            Pose pose = args.Anchor.GetAnchorPose();
            Debug.Log($"ASA-Android: Creating an anchor at: {pose.position.ToString("G4")}, {pose.rotation.eulerAngles.ToString("G2")}");
            GameObject gameObject = SpawnGameObject(pose.position, pose.rotation);
            gameObject.AddARAnchor();
            return gameObject;
        }

        /// <inheritdoc/>
        protected override async Task OnDiscoverCoordinatesAsync(CancellationToken cancellationToken, string[] idsToLocate = null)
        {
            await EnsureInitializedAsync().Unless(cancellationToken);
            RequestSessionStart();

            try
            {
                AnchorLocateCriteria anchorLocateCriteria = new AnchorLocateCriteria();

                HashSet<string> ids = new HashSet<string>();
                if (idsToLocate?.Length > 0)
                {
                    anchorLocateCriteria.Identifiers = idsToLocate;
                    for (int i = 0; i < idsToLocate.Length; i++)
                    {
                        if (!knownCoordinates.ContainsKey(idsToLocate[i]))
                        {
                            ids.Add(idsToLocate[i]);
                        }
                    }

                    if (ids.Count == 0)
                    {
                        // We know already all of the coordintes
                        return;
                    }
                }

                using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    // Local handler
                    async void AnchorLocatedHandler(object sender, AnchorLocatedEventArgs args)
                    {
                        if (args.Status == LocateAnchorStatus.Located)
                        {
                            // Switch to UI thread for the rest here
                            await gameThreadSynchronizationContext;

                            GameObject gameObject = CreateGameObjectFrom(args);

                            SpatialAnchorsCoordinate coordinate = new SpatialAnchorsCoordinate(args.Anchor, gameObject);
                            OnNewCoordinate(coordinate.Id, coordinate);

                            lock (ids)
                            {
                                // If we succefully removed one and we are at 0, then stop. 
                                // If we never had to locate any, we would always be at 0 but never remove any.
                                if (ids.Remove(args.Identifier) && ids.Count == 0)
                                {
                                    // We found all requested, stop
                                    cts.Cancel();
                                }
                            }
                        }
                    }

                    session.AnchorLocated += AnchorLocatedHandler;
                    CloudSpatialAnchorWatcher watcher = session.CreateWatcher(anchorLocateCriteria);
                    try
                    {
                        await cts.Token.AsTask().IgnoreCancellation();
                    }
                    finally
                    {
                        session.AnchorLocated -= AnchorLocatedHandler;
                        watcher.Stop();
                    }
                }
            }
            finally
            {
                ReleaseSessionStartRequest();
            }
        }

        /// <summary>
        /// Simple helper method to spawn anchor <see cref="GameObject"/> provided position and rotation.
        /// </summary>
        /// <param name="worldPosition">Position of the anchor.</param>
        /// <param name="worldRotation">Rotation of the anchor.</param>
        /// <returns>The newly spawned <see cref="GameObject"/>.</returns>
        protected GameObject SpawnGameObject(Vector3 worldPosition, Quaternion worldRotation)
        {
            GameObject spawnedAnchorObject = new GameObject("Azure Spatial Anchor");

            spawnedAnchorObject.transform.position = worldPosition;
            spawnedAnchorObject.transform.rotation = worldRotation;

            return spawnedAnchorObject;
        }

        /// <inheritdoc/>
        protected override async Task<ISpatialCoordinate> TryCreateCoordinateAsync(Vector3 worldPosition, Quaternion worldRotation, CancellationToken cancellationToken)
        {
            await EnsureInitializedAsync().Unless(cancellationToken);

            RequestSessionStart();

            try
            {
                await recommendedForCreate.Task.Unless(cancellationToken);

                GameObject spawnedAnchorObject = SpawnGameObject(worldPosition, worldRotation);
                try
                {
                    spawnedAnchorObject.AddARAnchor();

                    // Let a frame pass to ensure any AR anchor is properly attached (WorldAnchors used to have issues with this)
                    await Task.Delay(100, cancellationToken);

                    CloudSpatialAnchor cloudSpatialAnchor = new CloudSpatialAnchor()
                    {
                        LocalAnchor = spawnedAnchorObject.GetNativeAnchorPointer(),
                        Expiration = DateTime.Now.AddDays(1)
                    };

                    if (cloudSpatialAnchor.LocalAnchor == IntPtr.Zero)
                    {
                        Debug.LogError($"{nameof(SpatialAnchorsCoordinateService)} failed to get native anchor pointer when creating anchor.");
                        return null;
                    }

                    await session.CreateAnchorAsync(cloudSpatialAnchor);
                    return new SpatialAnchorsCoordinate(cloudSpatialAnchor, spawnedAnchorObject);
                }
                catch
                {
                    UnityEngine.Object.Destroy(spawnedAnchorObject);
                    throw;
                }
            }
            finally
            {
                ReleaseSessionStartRequest();
            }
        }

        /// <inheritdoc/>
        protected override bool TryParse(string id, out string result)
        {
            result = id;
            return true;
        }

        /// <summary>
        /// Override this method in the implementation class to add additional initialization logic.
        /// </summary>
        protected virtual Task OnInitializeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Abstract method to be implemented by specific platform implentation that adds additional session configuration.
        /// </summary>
        /// <param name="session">The session that was just constructed and is being configured.</param>
        protected abstract void OnConfigureSession(CloudSpatialAnchorSession session);

        private Task EnsureInitializedAsync()
        {
            lock (lockObj)
            {
                if (initializationTask == null)
                {
                    initializationTask = InitializeAsync(disposedCTS.Token);
                }
            }

            return initializationTask;
        }

        private async Task InitializeAsync(CancellationToken cancellationToken)
        {
            await OnInitializeAsync();

            session = CreateSession();
        }

        private void SetValue(string configValue, Action<string> setter)
        {
            if (!string.IsNullOrWhiteSpace(configValue))
            {
                setter(configValue.Trim());
            }
        }

        private CloudSpatialAnchorSession CreateSession()
        {
            CloudSpatialAnchorSession toReturn = new CloudSpatialAnchorSession();

            SetValue(spatialAnchorsConfiguration.AccessToken, s => toReturn.Configuration.AccessToken = s);
            SetValue(spatialAnchorsConfiguration.AccountDomain, s => toReturn.Configuration.AccountDomain = s);
            SetValue(spatialAnchorsConfiguration.AccountId, s => toReturn.Configuration.AccountId = s);
            SetValue(spatialAnchorsConfiguration.AccountKey, s => toReturn.Configuration.AccountKey = s);
            SetValue(spatialAnchorsConfiguration.AuthenticationToken, s => toReturn.Configuration.AuthenticationToken = s);

            toReturn.LogLevel = SessionLogLevel.All;

            toReturn.Error += OnSessionError;
            toReturn.OnLogDebug += OnSessionLogDebug;
            toReturn.SessionUpdated += OnSessionUpdated;

            //TODO how to properly use this?
            //toReturn.LocateAnchorsCompleted += OnLocateAnchorsCompleted;

            OnConfigureSession(toReturn);

            return toReturn;
        }

        private void HandleSessionStatus(SessionStatus status)
        {
            if (status.ReadyForCreateProgress >= 1)
            {
                readyForCreate.TrySetResult(true);
            }

            if (status.RecommendedForCreateProgress >= 1)
            {
                recommendedForCreate.TrySetResult(true);
            }
        }

        private void OnSessionUpdated(object sender, SessionUpdatedEventArgs args)
        {
            HandleSessionStatus(args.Status);
        }

        private void OnSessionLogDebug(object sender, OnLogDebugEventArgs args)
        {
            Debug.Log(args.Message);
        }

        private void OnSessionError(object sender, SessionErrorEventArgs args)
        {
            Debug.LogError(args.ErrorMessage);
        }
    }
}
#endif