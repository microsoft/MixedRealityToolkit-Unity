// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_IOS
using Microsoft.Azure.SpatialAnchors;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.AzureSpatialAnchors
{
    // TODO anborod make this compile for iOS
    internal class SpatialAnchorsIOSCoordinateService : SpatialAnchorsCoordinateService
    {
        public SpatialAnchorsIOSCoordinateService(SpatialAnchorsConfiguration spatialAnchorsConfiguration)
            : base(spatialAnchorsConfiguration)
        {
        }

        protected override Task OnInitializeAsync()
        {
            return Task.CompletedTask;
        }

        protected override void OnFrameUpdate()
        {
            base.OnFrameUpdate();
        }

        protected override void OnConfigureSession(CloudSpatialAnchorSession session)
        {
           
        }
    }
}
#endif