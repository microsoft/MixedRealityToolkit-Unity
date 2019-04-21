//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class SliderRotateObject : MonoBehaviour
    {
        [SerializeField]
        private Transform targetObjectTransform = null;

        public void OnSliderUpdated(SliderEventData eventData)
        {
            if (targetObjectTransform != null)
            {
                // Rotate the target object using Slider's eventData.NewValue
                targetObjectTransform.localRotation = Quaternion.Euler(targetObjectTransform.eulerAngles.x, eventData.NewValue * -360, targetObjectTransform.eulerAngles.z);
            }
        }
    }
}
