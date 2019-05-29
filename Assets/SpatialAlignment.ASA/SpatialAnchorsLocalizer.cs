// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.AzureSpatialAnchors
{
    /// <summary>
    /// This is the localization mechanism for enabling anchor exchange/localization through Azure Spatial Anchors.
    /// </summary>
    public class SpatialAnchorsLocalizer : SpatialLocalizerBase
    {
#if SPATIALALIGNMENT_ASA
        private SpatialAnchorsCoordinateService coordinateService;
#endif

        /// <summary>
        /// Location of the anchor used for localization.
        /// </summary>
        [SerializeField]
        [Tooltip("Rotation of the anchor used for localization.")]
        private Vector3 anchorPosition = Vector3.zero;

        /// <summary>
        /// Location of the anchor used for localization.
        /// </summary>
        [SerializeField]
        [Tooltip("Rotation of the anchor used for localization.")]
        private Vector3 anchorRotation = Vector3.zero;

        /// <summary>
        /// Configuration for the Azure Spatial Anchors service.
        /// </summary>
        [SerializeField]
        [Tooltip("Configuration for the Azure Spatial Anchors service.")]
        private SpatialAnchorsConfiguration configuration = null;

        private void Awake()
        {
#if !SPATIALALIGNMENT_ASA
            Debug.LogError("Attempting to use SpatialAnchorLocalizer but ASA is not enabled for this build");
#elif UNITY_WSA && SPATIALALIGNMENT_ASA
            spatialCoordinateService = coordinateService = new SpatialAnchorsUWPCoordinateService(configuration);
#elif UNITY_ANDROID && SPATIALALIGNMENT_ASA
            spatialCoordinateService = coordinateService = new SpatialAnchorsAndroidCoordinateService(configuration);
#elif UNITY_IOS && SPATIALALIGNMENT_ASA
            Debug.LogError("SpatialAnchorLocalizer does not yet support iOS");
#endif

            if ((string.IsNullOrWhiteSpace(configuration.AccountId) || string.IsNullOrWhiteSpace(configuration.AccountKey)) && string.IsNullOrWhiteSpace(configuration.AuthenticationToken) && string.IsNullOrWhiteSpace(configuration.AccessToken))
            {
                Debug.LogError("Authentication method not configured for Azure Spatial Anchors, ensure you configured AccountID and AccountKey, or Authentication Token, or Access Token.", this);
            }
        }

#if SPATIALALIGNMENT_ASA
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
            return await coordinateService.TryCreateCoordinateAsync(anchorPosition, Quaternion.Euler(anchorRotation), CancellationToken.None);
        }
#else
        protected override Task<ISpatialCoordinate> GetHostCoordinateAsync(Guid token)
        {
            return Task.FromResult<ISpatialCoordinate>(null);
        }
#endif
    }
}
