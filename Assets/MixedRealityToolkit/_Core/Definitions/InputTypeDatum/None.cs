// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputTypes
{
    public struct None : IMixedRealityInputTypeDatum<bool>
    {
        public bool IsSupported { get; }

        public bool IsAvailable { get; }

        public bool Pressed { get; }

        public bool Touched { get; }

        public bool Changed { get; private set; }

        public bool Value { get; private set; }

        public void Update(bool reading)
        {
            Changed = Value != reading;

            Value = reading;
        }
    }
}