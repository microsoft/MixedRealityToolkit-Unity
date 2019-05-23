// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
    public abstract class SpatialAnchorsCoordinateService : SpatialCoordinateServiceUnityBase<string>
    {
        private readonly object lockObj = new object();
        private readonly SpatialAnchorsConfiguration spatialAnchorsConfiguration;

        private Task initializationTask = null;
        protected CloudSpatialAnchorSession session;

        public SpatialAnchorsCoordinateService(SpatialAnchorsConfiguration spatialAnchorsConfiguration)
        {
            this.spatialAnchorsConfiguration = spatialAnchorsConfiguration;
        }

        protected override void OnManagedDispose()
        {
            base.OnManagedDispose();

            session?.Dispose();
            session = null;
        }

        public override async Task<bool> TryDeleteCoordinateAsync(string id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            await EnsureInitializedAsync().Unless(cancellationToken);

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

        protected virtual void CreateCoordinateFrom(AnchorLocatedEventArgs args)
        {

        }

        protected override async Task OnDiscoverCoordinatesAsync(CancellationToken cancellationToken, string[] idsToLocate = null)
        {
            await EnsureInitializedAsync().Unless(cancellationToken);

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
                void AnchorLocatedHandler(object sender, AnchorLocatedEventArgs args)
                {
                    if (args.Status == LocateAnchorStatus.Located)
                    {
                        //TODO Create Anchor

                        // If we succefully removed one and we are at 0, then stop. 
                        // If we never had to locate any, we would always be at 0 but never remove any.
                        if (ids.Remove(args.Identifier) && ids.Count == 0)
                        {
                            // We found all requested, stop
                            cts.Cancel();
                        }
                    }
                }

                session.AnchorLocated += AnchorLocatedHandler;
                CloudSpatialAnchorWatcher watcher = session.CreateWatcher(anchorLocateCriteria);
                try
                {
                    await cts.Token.AsTask();
                }
                finally
                {
                    session.AnchorLocated -= AnchorLocatedHandler;
                    watcher.Stop();
                }
            }
        }

        protected GameObject SpawnGameObject(Vector3 worldPosition, Quaternion worldRotation)
        {
            GameObject spawnedAnchorObject = new GameObject("Azure Spatial Anchor");

            spawnedAnchorObject.transform.position = worldPosition;
            spawnedAnchorObject.transform.rotation = worldRotation;

            return spawnedAnchorObject;
        }

        protected override async Task<ISpatialCoordinate> TryCreateCoordinateAsync(Vector3 worldPosition, Quaternion worldRotation, CancellationToken cancellationToken)
        {
            await EnsureInitializedAsync().Unless(cancellationToken);

            GameObject spawnedAnchorObject = SpawnGameObject(worldPosition, worldRotation);
            try
            {
                spawnedAnchorObject.AddARAnchor();

                // Let a frame pass to ensure any AR anchor is properly attached (WorldAnchors used to have issues with this)
                await Task.Delay(1, cancellationToken);

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

        protected override bool TryParse(string id, out string result)
        {
            result = id;
            return true;
        }

        protected virtual Task OnInitializeAsync()
        {
            return Task.CompletedTask;
        }

        protected abstract void OnConfigureSession(CloudSpatialAnchorSession session);

        private Task EnsureInitializedAsync()
        {
            lock (lockObj)
            {
                if (initializationTask == null)
                {
                    initializationTask = InitializeAsync();
                }
            }

            return initializationTask;
        }

        private async Task InitializeAsync()
        {
            await OnInitializeAsync();

            session = CreateSession();
        }

        private CloudSpatialAnchorSession CreateSession()
        {
            CloudSpatialAnchorSession toReturn = new CloudSpatialAnchorSession();
            toReturn.Configuration.AccessToken = spatialAnchorsConfiguration.AccessToken;
            toReturn.Configuration.AccountDomain = spatialAnchorsConfiguration.AccountDomain;
            toReturn.Configuration.AccountId = spatialAnchorsConfiguration.AccountId;
            toReturn.Configuration.AccountKey = spatialAnchorsConfiguration.AccountKey;
            toReturn.Configuration.AuthenticationToken = spatialAnchorsConfiguration.AuthenticationToken;

            toReturn.Error += OnSessionError;
            toReturn.OnLogDebug += OnSessionLogDebug;
            toReturn.SessionUpdated += OnSessionUpdated;

            //TODO how to properly use this?
            //toReturn.LocateAnchorsCompleted += OnLocateAnchorsCompleted;

            OnConfigureSession(toReturn);

            return toReturn;
        }

        private void OnSessionUpdated(object sender, SessionUpdatedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void OnSessionLogDebug(object sender, OnLogDebugEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void OnSessionError(object sender, SessionErrorEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}