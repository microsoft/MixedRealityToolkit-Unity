// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputTypes
{
    public struct SixDoF : IMixedRealityInputTypeDatum<Tuple<Vector3, Quaternion>>
    {
        public SixDoF(Tuple<Vector3, Quaternion> startPosition)
        {
            IsSupported = true;
            IsAvailable = true;
            Pressed = false;
            Touched = false;
            Changed = false;
            Value = startPosition;
        }

        public bool IsSupported { get; }

        public bool IsAvailable { get; }

        public bool Pressed { get; }

        public bool Touched { get; }

        public bool Changed { get; private set; }

        public Tuple<Vector3, Quaternion> Value { get; set; }

        public void Update(Tuple<Vector3, Quaternion> reading)
        {
            Changed = Value != reading;
            Value = reading;
        }

        public void Update(Vector3 position, Quaternion rotation)
        {
            Changed = Value.Item1 != position && Value.Item2 != rotation;
            Value.Item1.Set(position.x, position.y, position.z);
            Value.Item2.Set(rotation.x, rotation.y, rotation.z, rotation.w);
        }
    }
}