// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    /// <summary>
    /// Used to aid in layout of Controller Input Actions.
    /// </summary>
    [Serializable]
    public class ControllerInputActionOption
    {
        public SupportedControllerType Controller;
        public Handedness Handedness;
        public Vector2[] InputLabelPositions;
        public bool[] IsLabelFlipped;
    }
}