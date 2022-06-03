// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Simulation
{
    /// <summary>
    /// Input simulation control mapping systems.
    /// </summary>
    public enum InputSimulatorControlSet
    {
        [InspectorName("MRTK v3")]
        MrtkV3 = 0,

        [InspectorName("MRTK v2")]
        MrtkV2 = 1,

        // todo: soon
        // [InspectorName("HoloLens 2 Emulator")]
        // HoloLens2Emulator = 2

        // todo: soon
        // [InspectorName("Custom")]
        // Custom = (uint)(-1)
    }
}
