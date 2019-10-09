// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Provides interface for getting and setting behaviors and
    /// possible other settings for pointers in the input system.
    /// Behaviors are described based on pointer type and input type,
    /// not per pointer. This is to ensure that new pointers that appear
    /// maintain consistent behavior.
    /// </summary>
    public interface IPointerPreferences
    {
        /// <summary>
        /// Gets the <seealso cref="MixedReality.Toolkit.Input.PointerBehavior"/> for a given pointer
        /// </summary>
        PointerBehavior GetPointerBehavior(IMixedRealityPointer pointer);

        /// <summary>
        /// Gets the <seealso cref="PointerBehavior"/> for a given pointer type,
        /// handedness, and input type
        /// </summary>
        /// <typeparam name="T">All pointers that are of this type, or a subclass of this type, will have the given behavior</typeparam>
        /// <param name="handedness">Specify Handedness.Any to apply to all handedness, or specify a specific handedness to just disable, right, left.</param>
        /// <param name="sourceType">Allows specification of pointer behavior per input source, so that pointers can be disabled for hands but not controllers, and vice versa.</param>
        PointerBehavior GetPointerBehavior<T>(
            Handedness handedness,
            InputSourceType sourceType) where T : class, IMixedRealityPointer;

        /// <summary>
        /// Sets the <seealso cref="PointerBehavior"/> for a given pointer type,
        /// handedness, and input type
        /// </summary>
        /// <typeparam name="T">All pointers that are of this type, or a subclass of this type, will have the given behavior</typeparam>
        /// <param name="handedness">Specify Handedness.Any to apply to all handedness, or specify a specific handedness to just disable, right, left.</param>
        /// <param name="sourceType">Allows specification of pointer behavior per input source, so that pointers can be disabled for hands but not controllers, and vice versa.</param>
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