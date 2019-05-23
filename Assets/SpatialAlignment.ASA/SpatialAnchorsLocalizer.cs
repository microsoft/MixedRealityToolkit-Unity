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
            coordinateService = new SpatialAnchorsUWPCoordinateService(configuration);
#elif UNITY_ANDROID
#elif UNITY_IOS
#endif
        }

        protected override async Task<ISpatialCoordinate> GetHostCoordinateAsync(Guid token)
        {
            return await coordinateService.TryCreateCoordinateAsync(Vector3.zero, Quaternion.identity, CancellationToken.None);
        }
    }
}
