// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Provides interface for accessing pointer preferences
    /// </summary>
    public interface IPointerPreferences
    {
        PointerBehavior GetPointerBehavior(IMixedRealityPointer pointer);
        PointerBehavior GetPointerBehavior<T>(
            Handedness handedness,
            InputSourceType sourceType) where T : class, IMixedRealityPointer;

        void SetPointerBehavior<T>(Handedness handedness, InputSourceType inputType, PointerBehavior pointerBehavior) where T : class, IMixedRealityPointer;

        /// <summary>
        /// Pointer behavior for the gaze pointer.
        /// We make gaze pointer unique because the internal 
        /// gaze pointer actually cannot be referenced from here 
        /// since it's an internal class.
        /// </summary>
        PointerBehavior GazePointerBehavior { get; set; }
    }
}