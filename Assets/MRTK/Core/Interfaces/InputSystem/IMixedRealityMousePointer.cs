// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface for handling mouse pointers.
    /// </summary>
    public interface IMixedRealityMousePointer : IMixedRealityPointer
    {
        /// <summary>
        /// Should the mouse cursor be hidden when no active input is received?
        /// </summary>
        bool HideCursorWhenInactive { get; }

        /// <summary>
        /// What is the movement threshold to reach before un-hiding mouse cursor?
        /// </summary>
        float MovementThresholdToUnHide { get; }

        /// <summary>
        /// How long should it take before the mouse cursor is hidden?
        /// </summary>
        float HideTimeout { get; }
    }
}