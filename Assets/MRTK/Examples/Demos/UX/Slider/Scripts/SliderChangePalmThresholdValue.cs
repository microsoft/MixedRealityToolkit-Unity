//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class SliderChangePalmThresholdValue : MonoBehaviour
    {

        public void ChangeFacingCameraTrackingThreshold(SliderEventData eventData)
        {
            GetComponent<HandConstraintPalmUp>().FacingCameraTrackingThreshold = eventData.NewValue * 90.0f;
        }
    }
}
