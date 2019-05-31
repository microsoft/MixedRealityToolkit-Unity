// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class TouchEventData
    {
        public TouchEventData (HandTrackingInputEventData inputData)
        {
            SourceId = inputData.SourceId;
            TouchPosition = inputData.InputData;
            Handedness = inputData.Handedness;
            Controller = inputData.Controller;
        }
        public IMixedRealityController Controller { get; private set; }
        public uint SourceId { get; private set; }
        public Vector3 TouchPosition { get; private set; }
        public Handedness Handedness { get; private set; }
    }
}