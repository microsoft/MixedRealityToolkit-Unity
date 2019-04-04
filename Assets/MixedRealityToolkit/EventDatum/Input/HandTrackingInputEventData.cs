// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class HandTrackingInputEventData : InputEventData<Vector3>
    {
        /// <summary>
        /// Constructor creates a default EventData object.
        /// Requires initialization.
        /// </summary>
        /// <param name="eventSystem"></param>
        public HandTrackingInputEventData(EventSystem eventSystem) : base(eventSystem) { }

        public IMixedRealityController Controller { get; set; }

        /// <summary>
        /// This function is called to fill the HandTrackingIntputEventData object with information
        /// </summary>
        /// <param name="inputSource">This is a reference to the HandTrackingInputSource that created the EventData</param>
        /// <param name="controller">This is a reference to the IMixedRealityController that created the EventData</param>
        /// <param name="grabbing">This is a the state (grabbing or not grabbing) of the HandTrackingInputSource that created the EventData</param>
        /// <param name="pressing">This is a the state (pressing or not pressing) of the HandTrackingInputSource that created the EventData</param>
        /// <param name="actionPoint">This is a the global position grabbed by the HandTrackingInputSource that created the EventData</param>
        /// <param name="touchedObject">This is a the global position of the HandTrackingInputSource that created the EventData</param>
        public void Initialize(IMixedRealityInputSource inputSource, IMixedRealityController controller, Handedness sourceHandedness, Vector3 touchPoint)
        {
            Initialize(inputSource, Handedness.None, MixedRealityInputAction.None, touchPoint);
            Controller = controller;
        }
    }
}
