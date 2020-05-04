// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Detects if the Leap Motion Data Provider can be used given the current unity project configuration and displays a message 
    /// to the LeapMotionHandTrackingExample menu panel.
    /// </summary>
    public class LeapCoreAssetsDetector : MonoBehaviour
    {
        void Start()
        {
            var text = gameObject.GetComponent<Text>();

#if LEAPMOTIONCORE_PRESENT
            text.text = "The Leap Data Provider can be used in this project";
            text.color = Color.green;
#else
        text.text = "This project has not met the requirements to use the Leap Data Provider. For more information, visit the MRTK Leap Motion Documentation";
        text.color = Color.red;
#endif
        }
    }
}
