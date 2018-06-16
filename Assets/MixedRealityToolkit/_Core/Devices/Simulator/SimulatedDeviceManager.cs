// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using System;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.Simulator
{
    // TODO - Implement
    public class SimulatedDevice : IMixedRealityDeviceManager
    {
        public string Name { get; }
        public uint Priority { get; }
        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Enable()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void Disable()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public IMixedRealityController[] GetActiveControllers()
        {
            throw new NotImplementedException();
        }
    }
}