// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.AzureSpatialAnchors
{
    /// <summary>
    /// This is the localization mechanism for enabling anchor exchange/localization through Azure Spatial Anchors.
    /// </summary>
    public class SpatialAnchorsLocalizer : SpatialLocalizerBase
    {
#if ASA_LOCALIZATION
        private SpatialAnchorsCoordinateService coordinateService;
#endif

        /// <summary>
        /// Configuration for the Azure Spatial Anchors service.
        /// </summary>
        [SerializeField]
        [Tooltip("Configuration for the Azure Spatial Anchors service.")]
        private SpatialAnchorsConfiguration configuration = null;

        private void Awake()
        {
#if UNITY_WSA && ASA_LOCALIZATION
            spatialCoordinateService = coordinateService = new SpatialAnchorsUWPCoordinateService(configuration);
#elif UNITY_ANDROID && ASA_LOCALIZATION
            spatialCoordinateService = coordinateService = new SpatialAnchorsAndroidCoordinateService(configuration);
#elif UNITY_IOS && ASA_LOCALIZATION
#endif

            if ((string.IsNullOrWhiteSpace(configuration.AccountId) || string.IsNullOrWhiteSpace(configuration.AccountKey)) && string.IsNullOrWhiteSpace(configuration.AuthenticationToken) && string.IsNullOrWhiteSpace(configuration.AccessToken))
            {
                Debug.LogError("Authentication method not configured for Azure Spatial Anchors, ensure you configured AccountID and AccountKey, or Authentication Token, or Access Token.", this);
            }
        }

#if ASA_LOCALIZATION
        private void Update()
        {
            coordinateService.FrameUpdate();
        }

        private void OnDestroy()
        {
            coordinateService.Dispose();
            coordinateService = null;
            spatialCoordinateService = null;
        }

        /// <inheritdoc/>
        protected override async Task<ISpatialCoordinate> GetHostCoordinateAsync(Guid token)
        {
            return await coordinateService.TryCreateCoordinateAsync(new Vector3(0f, -0.25f, 0.25f), Quaternion.identity, CancellationToken.None);
        }
#else
        protected override Task<ISpatialCoordinate> GetHostCoordinateAsync(Guid token)
        {
            return Task.FromResult<ISpatialCoordinate>(null);
        }
#endif
    }
}
