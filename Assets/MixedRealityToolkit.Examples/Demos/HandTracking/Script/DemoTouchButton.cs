// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class DemoTouchButton : MonoBehaviour, IMixedRealityPointerHandler
    {
        [SerializeField]
        private TextMesh debugMessage = null;

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (debugMessage != null)
            {
                debugMessage.text = "OnPointerDown: " + Time.unscaledTime.ToString();
                Debug.Log(eventData.MixedRealityInputAction.Description + " down");
            }
        }

        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if (debugMessage != null)
            {
                debugMessage.text = "OnPointerUp: " + Time.unscaledTime.ToString();
                Debug.Log(eventData.MixedRealityInputAction.Description + " up");
            }
        }
    }
}