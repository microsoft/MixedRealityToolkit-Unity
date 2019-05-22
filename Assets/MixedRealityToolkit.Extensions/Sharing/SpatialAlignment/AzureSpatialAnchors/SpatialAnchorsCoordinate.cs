// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Azure.SpatialAnchors;
using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using UnityEngine;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.AzureSpatialAnchors
{
    internal class SpatialAnchorsCoordinate : SpatialCoordinateUnityBase<string>
    {
        private readonly GameObject anchorGO;

        private readonly Quaternion anchorRotation;
        private readonly Quaternion invertedAnchorRotation;

        public CloudSpatialAnchor CloudSpatialAnchor { get; }

        public override LocatedState State => base.State;

        public SpatialAnchorsCoordinate(CloudSpatialAnchor cloudSpatialAnchor, GameObject anchorGO)
            : base(cloudSpatialAnchor.Identifier)
        {
            this.CloudSpatialAnchor = cloudSpatialAnchor;
            this.anchorGO = anchorGO;
            anchorRotation = anchorGO.transform.rotation;
            invertedAnchorRotation = Quaternion.Inverse(anchorRotation);
        }

        protected override Vector3 CoordinateToWorldSpace(Vector3 vector)
        {
            return anchorGO.transform.TransformPoint(vector);
        }

        protected override Quaternion CoordinateToWorldSpace(Quaternion quaternion)
        {
            return quaternion * invertedAnchorRotation;
        }

        protected override Vector3 WorldToCoordinateSpace(Vector3 vector)
        {
            return anchorGO.transform.InverseTransformPoint(vector);
        }

        protected override Quaternion WorldToCoordinateSpace(Quaternion quaternion)
        {
            return quaternion * anchorRotation;
        }

        protected override void OnManagedDispose()
        {
            base.OnManagedDispose();

            UnityEngine.Object.Destroy(anchorGO);
        }
    }
}
