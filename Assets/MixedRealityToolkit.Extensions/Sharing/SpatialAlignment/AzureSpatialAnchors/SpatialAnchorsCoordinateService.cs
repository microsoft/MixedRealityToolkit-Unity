// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Assets.MRTK.MixedRealityToolkit.Extensions.Sharing.SpatialAlignment.AzureSpatialAnchors;
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using System;
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
        private CloudSpatialAnchorSession session;

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

        protected override async Task OnDiscoverCoordinatesAsync(CancellationToken cancellationToken, string[] idsToLocate = null)
        {
            await EnsureInitializedAsync().Unless(cancellationToken);

            AnchorLocateCriteria anchorLocateCriteria = new AnchorLocateCriteria();

            if (idsToLocate?.Length > 0)
            {
                anchorLocateCriteria.Identifiers = idsToLocate;
            }

            CloudSpatialAnchorWatcher watcher = session.CreateWatcher(anchorLocateCriteria);
            try
            {
                await cancellationToken.AsTask();
            }
            finally
            {
                watcher.Stop();
            }
        }

        protected override async Task<ISpatialCoordinate> TryCreateCoordinateAsync(Vector3 worldPosition, Quaternion worldRotation, CancellationToken cancellationToken)
        {
            await EnsureInitializedAsync().Unless(cancellationToken);

            GameObject spawnedAnchorObject = new GameObject("Azure Spatial Anchor");
            try
            {
                spawnedAnchorObject.transform.position = worldPosition;
                spawnedAnchorObject.transform.rotation = worldRotation;

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

            OnConfigureSession(toReturn);

            return toReturn;
        }
    }
}