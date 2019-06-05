// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Information associated with a touch event
    /// </summary>
    public class TouchEventData
    {
        public TouchEventData (HandTrackingInputEventData inputData, TouchHandler source)
        {
            SourceId = inputData.SourceId;
            TouchPosition = inputData.InputData;
            Handedness = inputData.Handedness;
            Controller = inputData.Controller;
            TouchSource = source;
        }

        /// <summary>
        /// The TouchHandler that has fired this event
        /// </summary>
        public TouchHandler TouchSource { get; private set; }
        
        /// <summary>
        /// The controller attached to this touch event
        /// </summary>
        public IMixedRealityController Controller { get; private set; }
        
        /// <summary>
        /// ID of input source attached to this touch event
        /// </summary>
        public uint SourceId { get; private set; }
        
        /// <summary>
        /// World-space position of the input point when the event was fired
        /// </summary>
        public Vector3 TouchPosition { get; private set; }

        /// <summary>
        /// Handedness of the hand / controller firing this event
        /// </summary>
        public Handedness Handedness { get; private set; }
    }
}