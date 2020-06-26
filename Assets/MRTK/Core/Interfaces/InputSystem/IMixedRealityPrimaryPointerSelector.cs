// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface used by the focus provider to select the pointer that will be considered as primary.
    /// The current primary pointer can we obtained via <see cref="IMixedRealityFocusProvider.PrimaryPointer"/> or 
    /// subscribing to the primary pointer changed event via <see cref="IMixedRealityFocusProvider.SubscribeToPrimaryPointerChanged"/>.
    /// </summary>
    public interface IMixedRealityPrimaryPointerSelector
    {
        /// <summary>
        /// Called on initialization of the focus provider to initialize the selector.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Called on destruction of the focus provider to destroy the selector.
        /// </summary>
        void Destroy();

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
        /// <returns>The new primary pointer or null if none.</returns>
        IMixedRealityPointer Update();
    }
}