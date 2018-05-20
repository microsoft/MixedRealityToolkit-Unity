// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    public interface IMixedRealityDevice
    {
        void Initialize();

        void Enable();

        void Disable();

        void Destroy();

        IMixedRealityInputSource[] GetActiveControllers();
    }
}