// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    [AddComponentMenu("Scripts/MRTK/Examples/HandInteractionTouchRotate")]
    public class HandInteractionTouchRotate : HandInteractionTouch, IMixedRealityTouchHandler
    {
        [SerializeField]
        [FormerlySerializedAs("TargetObjectTransform")]
        private Transform targetObjectTransform = null;

        [SerializeField]
        private float rotateSpeed = 300.0f;

        void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
        {
            if (targetObjectTransform != null)
            {
                targetObjectTransform.Rotate(Vector3.up * (rotateSpeed * Time.deltaTime));
            }
        }
    }
}