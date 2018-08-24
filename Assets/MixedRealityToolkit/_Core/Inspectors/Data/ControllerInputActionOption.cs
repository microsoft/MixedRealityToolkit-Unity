// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Data
{
    [Serializable]
    public class ControllerInputActionOption
    {
        public SupportedControllerType Controller;
        public Handedness Handedness;
        public Vector2[] InputLabelPositions;
        public bool[] IsLabelFlipped;
    }
}