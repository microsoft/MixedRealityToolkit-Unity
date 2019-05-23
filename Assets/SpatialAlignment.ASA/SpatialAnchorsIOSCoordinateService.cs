// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_WSA
using Assets.MRTK.MixedRealityToolkit.Extensions.Sharing.SpatialAlignment.AzureSpatialAnchors;
using Microsoft.Azure.SpatialAnchors;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.AzureSpatialAnchors
{
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

        protected override void OnConfigureSession(CloudSpatialAnchorSession session)
        {
            // TODO anborod: This is a copy from DemoWrapper (it was commented), figure out what should be done here

            // AAD user token scenario to get an authentication token
            //cloudSpatialAnchorSession.TokenRequired += async (object sender, SpatialServices.TokenRequiredEventArgs args) =>
            //{
            //    CloudSpatialAnchorSessionDeferral deferral = args.GetDeferral();
            //    // AAD user token scenario to get an authentication token
            //    args.AuthenticationToken = await AuthenticationHelper.GetAuthenticationTokenAsync();
            //    deferral.Complete();
            //};
        }
    }
}
#endif