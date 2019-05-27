// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_WSA && SPATIALALIGNMENT_ASA
using Microsoft.Azure.SpatialAnchors;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.AzureSpatialAnchors
{
    /// <summary>
    /// UWP implementation of the Azure Spatial Anchors coordinate service.
    /// </summary>
    internal class SpatialAnchorsUWPCoordinateService : SpatialAnchorsCoordinateService
    {
        public SpatialAnchorsUWPCoordinateService(SpatialAnchorsConfiguration spatialAnchorsConfiguration)
            : base(spatialAnchorsConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override GameObject CreateGameObjectFrom(AnchorLocatedEventArgs args)
        {
            GameObject gameObject = SpawnGameObject(Vector3.zero, Quaternion.identity);
            gameObject.AddComponent<WorldAnchor>();

            // On HoloLens, if we do not have a cloudAnchor already, we will have already positioned the
            // object based on the passed in worldPos/worldRot and attached a new world anchor,
            // so we are ready to commit the anchor to the cloud if requested.
            // If we do have a cloudAnchor, we will use it's pointer to setup the world anchor,
            // which will position the object automatically.
            if (args.Anchor != null)
            {
                gameObject.GetComponent<WorldAnchor>().SetNativeSpatialAnchorPtr(args.Anchor.LocalAnchor);
            }

            return gameObject;
        }

        /// <inheritdoc/>
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