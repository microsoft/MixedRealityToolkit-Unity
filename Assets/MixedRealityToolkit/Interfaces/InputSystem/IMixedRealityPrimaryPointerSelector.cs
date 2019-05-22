// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface used by focus providers to select the primary pointer.
    /// </summary>
    public interface IMixedRealityPrimaryPointerSelector
    {
        /// <summary>
        /// Registers a pointer with the selector.
        /// </summary>
        void RegisterPointer(IMixedRealityPointer pointer);

        /// <summary>
        /// Unregisters a pointer with the selector.
        /// </summary>
        void UnregisterPointer(IMixedRealityPointer pointer);

        /// <summary>
        /// Called from the focus provider after updating pointers to obtain the new primary pointer.
        /// </summary>
        ///<returns>The new primary pointer or null if none.</returns>
        IMixedRealityPointer Update();
    }
}