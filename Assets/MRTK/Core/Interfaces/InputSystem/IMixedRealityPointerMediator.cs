// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface for handling groups of pointers resolving conflicts between them.
    /// E.g., ensuring that far pointers are disabled when a near pointer is active.
    /// </summary>
    public interface IMixedRealityPointerMediator
    {
        void RegisterPointers(IMixedRealityPointer[] pointers);
        void UnregisterPointers(IMixedRealityPointer[] pointers);

        void UpdatePointers();

        /// <summary>
        /// Called to set the pointer preferences of the current input and focus
        /// configuration.
        /// </summary>
        /// <remarks>
        /// <para>These preferences can be used by the pointer mediator to determine runtime
        /// preferences set by the caller (for example, the caller could request to turn
        /// off all hand rays). It's possible that some of these preferences may not be
        /// honored (for example, current input system is set up to not have hand rays
        /// at all, and a request comes in to turn on/off hand rays).</para>
        /// </remarks>
        void SetPointerPreferences(IPointerPreferences pointerPreferences);
    }
}