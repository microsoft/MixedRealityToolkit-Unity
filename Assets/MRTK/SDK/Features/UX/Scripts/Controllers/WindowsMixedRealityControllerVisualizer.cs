// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [AddComponentMenu("Scripts/MRTK/SDK/WindowsMixedRealityControllerVisualizer")]
    public class WindowsMixedRealityControllerVisualizer : MixedRealityControllerVisualizer
    {
        private readonly Quaternion inverseRotation = Quaternion.Euler(0f, 180f, 0f);

        /// <inheritdoc />
        public override void OnSourcePoseChanged(SourcePoseEventData<MixedRealityPose> eventData)
        {
            if (UseSourcePoseData &&
                eventData.SourceId == Controller?.InputSource.SourceId)
            {
                base.OnSourcePoseChanged(eventData);
                transform.localRotation *= inverseRotation;
            }
        }
    }
}