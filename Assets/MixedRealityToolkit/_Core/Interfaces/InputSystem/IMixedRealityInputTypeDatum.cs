// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem
{
    public interface IMixedRealityInputTypeDatum<TReading>
    {
        bool IsSupported { get; }
        bool IsAvailable { get; }
        bool Pressed { get; }
        bool Touched { get; }
        bool Changed { get; }
        TReading Value { get; }

        void Update(TReading reading);
    }
}
