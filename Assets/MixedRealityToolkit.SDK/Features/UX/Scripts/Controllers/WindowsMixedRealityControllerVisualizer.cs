// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class WindowsMixedRealityControllerVisualizer : MixedRealityControllerVisualizer
    {
        private readonly Quaternion inverseRotation = Quaternion.Euler(0f, 180f, 0f);

        public override void OnSourcePoseChanged(SourcePoseEventData<MixedRealityPose> eventData)
        {
            if (UseSourcePoseData &&
                eventData.SourceId == Controller?.InputSource.SourceId)
            {
                TrackingState = TrackingState.Tracked;
                transform.localPosition = eventData.SourceData.Position;
                transform.localRotation = eventData.SourceData.Rotation * inverseRotation;
            }
        }
    }
}