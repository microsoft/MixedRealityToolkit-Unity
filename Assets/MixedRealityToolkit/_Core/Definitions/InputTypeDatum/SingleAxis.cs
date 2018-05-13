// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputTypes
{
    public struct SingleAxis : IMixedRealityInputTypeDatum<float>
    {
        public bool IsSupported { get; }

        public bool IsAvailable { get; }

        public bool Pressed { get; }

        public bool Touched { get; }

        public bool Changed { get; private set; }

        public float Value { get; private set; }

        public void Update(float reading)
        {
            Changed = Value != reading;

            Value = reading;
        }
    }
}