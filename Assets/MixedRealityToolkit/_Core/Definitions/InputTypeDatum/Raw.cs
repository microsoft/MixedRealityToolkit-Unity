// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputTypes
{
    public struct Raw<T> : IMixedRealityInputTypeDatum<T>
    {
        public bool IsSupported { get; }

        public bool IsAvailable { get; }

        public bool Pressed { get; }

        public bool Touched { get; }

        public bool Changed { get; private set; }

        public T Value { get; private set; }

        public void Update(T reading)
        {
            Changed = true;

            Value = reading;
        }
    }
}