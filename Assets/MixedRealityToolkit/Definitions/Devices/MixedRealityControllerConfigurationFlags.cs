// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Devices
{
    /// <summary>
    /// Flags used by MixedRealityControllerAttribute.
    /// </summary>
    [System.Flags]
    public enum MixedRealityControllerConfigurationFlags : byte
    {
        /// <summary>
        /// Controllers with custom interaction mappings can have their mappings be added / removed to the
        /// controller mapping profile in the property inspector.
        /// </summary>
        UseCustomInteractionMappings = 1 << 0,
    }
}
