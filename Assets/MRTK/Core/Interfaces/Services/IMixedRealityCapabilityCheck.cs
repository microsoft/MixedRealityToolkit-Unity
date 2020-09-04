// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    public interface IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Checks to see if one or more registered data providers supports the requested capability
        /// on the current platform.
        /// </summary>
        /// <param name="capability">The capability to check.</param>
        /// <returns>True if the capability is supported, false otherwise.</returns>
        bool CheckCapability(MixedRealityCapability capability);
    }
}
