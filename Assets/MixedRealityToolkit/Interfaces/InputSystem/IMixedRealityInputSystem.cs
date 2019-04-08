// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Manager interface for a Input system in the Mixed Reality Toolkit
    /// All replacement systems for providing Input System functionality should derive from this interface
    /// </summary>
    public interface IMixedRealityInputSystem : IMixedRealityEventSystem, IMixedRealityInputEventSystem
    {
        /// <summary>
        /// Event that's raised when the Input is enabled.
        /// </summary>
        event Action InputEnabled;

        /// <summary>
        /// Event that's raised when the Input is disabled.
        /// </summary>
        event Action InputDisabled;

        /// <summary>
        /// List of the Interaction Input Sources as detected by the input manager like hands or motion controllers.
        /// </summary>
        HashSet<IMixedRealityInputSource> DetectedInputSources { get; }

        /// <summary>
        /// List of <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityController"/>s currently detected by the input manager.
        /// </summary>
        /// <remarks>
        /// This property is similar to <see cref="DetectedInputSources"/>, as this is a subset of those <see cref="IMixedRealityInputSource"/>s in that list.
        /// </remarks>
        HashSet<IMixedRealityController> DetectedControllers { get; }

        /// <summary>
        /// The current Focus Provider that's been implemented by this Input System.
        /// </summary>
        IMixedRealityFocusProvider FocusProvider { get; }

        /// <summary>
        /// The current Gaze Provider that's been implemented by this Input System.
        /// </summary>
        IMixedRealityGazeProvider GazeProvider { get; }

        /// <summary>
        /// The current Eye Gaze Provider that's been implemented by this Input System.
        /// </summary>
        IMixedRealityEyeGazeProvider EyeGazeProvider { get; }

        /// <summary>
        /// Indicates if input is currently enabled or not.
        /// </summary>
        bool IsInputEnabled { get; }

        /// <summary>
        /// Push a disabled input state onto the Input System.
        /// While input is disabled no events will be sent out and the cursor displays
        /// a waiting animation.
        /// </summary>
        void PushInputDisable();

        /// <summary>
        /// Pop disabled input state. When the last disabled state is 
        /// popped off the stack input will be re-enabled.
        /// </summary>
        void PopInputDisable();

        /// <summary>
        /// Clear the input disable stack, which will immediately re-enable input.
        /// </summary>
        void ClearInputDisableStack();

        /// <summary>
        /// Push a game object into the modal input stack. Any input handlers
        /// on the game object are given priority to input events before any focused objects.
        /// </summary>
        /// <param name="inputHandler">The input handler to push</param>
        void PushModalInputHandler(GameObject inputHandler);

        /// <summary>
        /// Remove the last game object from the modal input stack.
        /// </summary>
        void PopModalInputHandler();

        /// <summary>
        /// Clear all modal input handlers off the stack.
        /// </summary>
        void ClearModalInputStack();

        /// <summary>
        /// Push a game object into the fallback input stack. Any input handlers on
        /// the game object are given input events when no modal or focused objects consume the event.
        /// </summary>
        /// <param name="inputHandler">The input handler to push</param>
        void PushFallbackInputHandler(GameObject inputHandler);

        /// <summary>
        /// Remove the last game object from the fallback input stack.
        /// </summary>
        void PopFallbackInputHandler();

        /// <summary>
        /// Clear all fallback input handlers off the stack.
        /// </summary>
        void ClearFallbackInputStack();

        #region Input Events
        /// <summary>
        /// Generates a new unique input source id.<para/>
        /// <remarks>All Input Sources are required to call this method in their constructor or initialization.</remarks>
        /// </summary>
        /// <returns>a new unique Id for the input source.</returns>
        uint GenerateNewSourceId();

        IMixedRealityInputSource RequestNewGenericInputSource(string name, IMixedRealityPointer[] pointers = null, InputSourceType sourceType = InputSourceType.Other);
        #endregion Input Events
    }
}
