// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Assets.MRTK.MixedRealityToolkit.Extensions.Sharing.SpatialAlignment.AzureSpatialAnchors;
using Microsoft.Azure.SpatialAnchors;
using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.AzureSpatialAnchors
{
    public abstract class SpatialAnchorsCoordinateService : SpatialCoordinateServiceBase<Guid>
    {
        private readonly object lockObj = new object();
        private readonly SpatialAnchorsConfiguration spatialAnchorsConfiguration;
        private Task initializationTask = null;

        public SpatialAnchorsCoordinateService(SpatialAnchorsConfiguration spatialAnchorsConfiguration)
        {
            this.spatialAnchorsConfiguration = spatialAnchorsConfiguration;
        }

        protected override async Task OnDiscoverCoordinatesAsync(CancellationToken cancellationToken, Guid[] idsToLocate = null)
        {
            await EnsureInitializedAsync();

            using (CloudSpatialAnchorSession session = CreateSession())
            {

            }
        }

        protected override bool TryParse(string id, out Guid result) => Guid.TryParse(id, out result);

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

        protected abstract Task InitializeAsync();

        protected abstract void OnConfigureSession(CloudSpatialAnchorSession session);

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