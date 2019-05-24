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

        private readonly Quaternion worldToCoordinateRotation;
        private readonly Quaternion coordinateToWorldRotation;

        public CloudSpatialAnchor CloudSpatialAnchor { get; }

        // TODO anborod this should be updated from the cloud session, but in our case while it's created it's technically located
        public override LocatedState State => LocatedState.Tracking;

        public SpatialAnchorsCoordinate(CloudSpatialAnchor cloudSpatialAnchor, GameObject anchorGO)
            : base(cloudSpatialAnchor.Identifier)
        {
            this.CloudSpatialAnchor = cloudSpatialAnchor;
            this.anchorGO = anchorGO;
            coordinateToWorldRotation = anchorGO.transform.rotation;
            worldToCoordinateRotation = Quaternion.Inverse(worldToCoordinateRotation);
        }

        protected override Vector3 CoordinateToWorldSpace(Vector3 vector)
        {
            return anchorGO.transform.TransformPoint(vector);
        }

        protected override Quaternion CoordinateToWorldSpace(Quaternion quaternion)
        {
            return coordinateToWorldRotation * quaternion;
        }

        protected override Vector3 WorldToCoordinateSpace(Vector3 vector)
        {
            return anchorGO.transform.InverseTransformPoint(vector);
        }

        protected override Quaternion WorldToCoordinateSpace(Quaternion quaternion)
        {
            return worldToCoordinateRotation * quaternion;
        }

        protected override void OnManagedDispose()
        {
            base.OnManagedDispose();

            UnityEngine.Object.Destroy(anchorGO);
        }
    }
}
