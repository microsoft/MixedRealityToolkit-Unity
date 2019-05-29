// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if SPATIALALIGNMENT_ASA
using Microsoft.Azure.SpatialAnchors;
using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using UnityEngine;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.AzureSpatialAnchors
{
    /// <summary>
    /// The Spatial Coordinate based on Azure Spatial Anchors service.
    /// </summary>
    internal class SpatialAnchorsCoordinate : SpatialCoordinateUnityBase<string>
    {
        private readonly GameObject anchorGO;

        /// <summary>
        /// The associated <see cref="CloudSpatialAnchor"/>.
        /// </summary>
        public CloudSpatialAnchor CloudSpatialAnchor { get; }

        // TODO anborod this should be updated from the cloud session, but in our case while it's created it's technically located
        /// <inheritdoc/>
        public override LocatedState State => LocatedState.Tracking;

        /// <summary>
        /// Creates a new instance of <see cref="SpatialAnchorsCoordinate"/>.
        /// </summary>
        /// <param name="cloudSpatialAnchor">The associated <see cref="CloudSpatialAnchor"/> to use for creation.</param>
        /// <param name="anchorGO">The <see cref="GameObject"/> representing this anchor.</param>
        public SpatialAnchorsCoordinate(CloudSpatialAnchor cloudSpatialAnchor, GameObject anchorGO)
            : base(cloudSpatialAnchor.Identifier)
        {
            this.CloudSpatialAnchor = cloudSpatialAnchor;
            this.anchorGO = anchorGO;
        }

        /// <inheritdoc/>
        protected override Vector3 CoordinateToWorldSpace(Vector3 vector)
        {
            return anchorGO.transform.TransformPoint(vector);
        }

        /// <inheritdoc/>
        protected override Quaternion CoordinateToWorldSpace(Quaternion quaternion)
        {
            return anchorGO.transform.rotation * quaternion;
        }

        /// <inheritdoc/>
        protected override Vector3 WorldToCoordinateSpace(Vector3 vector)
        {
            return anchorGO.transform.InverseTransformPoint(vector);
        }

        /// <inheritdoc/>
        protected override Quaternion WorldToCoordinateSpace(Quaternion quaternion)
        {
            return Quaternion.Inverse(anchorGO.transform.rotation) * quaternion;
        }

        /// <inheritdoc/>
        protected override void OnManagedDispose()
        {
            base.OnManagedDispose();

            UnityEngine.Object.Destroy(anchorGO);
        }
    }
}
#endif