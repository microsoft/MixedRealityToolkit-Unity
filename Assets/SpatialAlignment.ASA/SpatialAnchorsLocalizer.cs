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
    public class SpatialAnchorsLocalizer : SpatialLocalizerBase
    {
        private SpatialAnchorsCoordinateService coordinateService;

        [SerializeField]
        private SpatialAnchorsConfiguration configuration = null;

        private void Awake()
        {
#if UNITY_WSA
            spatialCoordinateService = coordinateService = new SpatialAnchorsUWPCoordinateService(configuration);
#elif UNITY_ANDROID
            spatialCoordinateService = coordinateService = new SpatialAnchorsAndroidCoordinateService(configuration);
#elif UNITY_IOS
#endif
        }

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

        protected override async Task<ISpatialCoordinate> GetHostCoordinateAsync(Guid token)
        {
            return await coordinateService.TryCreateCoordinateAsync(new Vector3(0f, -0.25f, 0.25f), Quaternion.identity, CancellationToken.None);
        }
    }
}
