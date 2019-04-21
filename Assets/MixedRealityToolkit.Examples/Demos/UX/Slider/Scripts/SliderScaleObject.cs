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
    public class SliderScaleObject : MonoBehaviour
    {
        [SerializeField]
        private Transform targetObjectTransform = null;

        public void OnSliderUpdated(SliderEventData eventData)
        {
            if (targetObjectTransform != null)
            {
                // Scale the target object using Slider's eventData.NewValue (Scale between 0.5 and 1.5)
                float newScale = eventData.NewValue + 0.5f;
                targetObjectTransform.localScale = new Vector3(newScale, newScale, newScale);
            }
        }
    }
}
