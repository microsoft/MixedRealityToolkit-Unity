// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.InputSystem.InputSources;
using Microsoft.MixedReality.Toolkit.InputSystem.Pointers;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces
{
    /// <summary>
    /// Manager interface for a Input system in the Mixed Reality Toolkit
    /// All replacement systems for providing Input System functionality should derive from this interface
    /// </summary>
    public interface IMixedRealityInputSystem : IEventSystemManager
    {
        event Action InputEnabled;
        event Action InputDisabled;

        /// <summary>
        /// List of the Interaction Input Sources as detected by the input manager like hands or motion controllers.
        /// </summary>
        HashSet<IInputSource> DetectedInputSources { get; }

        /// <summary>
        /// Indicates if input is currently enabled or not.
        /// </summary>
        bool IsInputEnabled { get; }

        /// <summary>
        /// Push a disabled input state onto the input manager.
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

        /// <summary>
        /// Generates a new unique input source id.<para/>
        /// <remarks>All Input Sources are required to call this method in their constructor or initialization.</remarks>
        /// </summary>
        /// <returns>a new unique Id for the input source.</returns>
        uint GenerateNewSourceId();

        IGazeProvider GazeProvider { get; }

        IFocusProvider FocusProvider { get; }

        void RaiseSourceDetected(IInputSource source, object[] tags = null);

        void RaiseSourceLost(IInputSource source, object[] tags = null);

        void RaiseSourcePositionChanged(IInputSource source, Vector3 pointerPosition, Vector3 gripPosition, object[] tags = null);

        void RaiseSourcePositionChanged(IInputSource source, Vector3 pointerPosition, Vector3 gripPosition, Handedness sourceHandedness, object[] tags = null);

        void RaiseSourceRotationChanged(IInputSource source, Quaternion pointerRotation, Quaternion gripRotation, Handedness sourceHandedness, object[] tags = null);

        void RaisePreFocusChangedEvent(IPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject);

        void OnFocusChangedEvent(IPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject);

        void RaiseFocusEnter(IPointer pointer, GameObject focusedObject);

        void RaiseFocusExit(IPointer pointer, GameObject unfocusedObject);

        void RaisePointerDown(IPointer pointer, object[] tags = null);

        void RaisePointerDown(IPointer pointer, Handedness handedness, object[] tags = null);

        void RaisePointerDown(IPointer pointer, InputType inputType, Handedness handedness, object[] tags = null);

        void RaiseInputClicked(IPointer pointer, int tapCount, object[] tags = null);

        void RaiseInputClicked(IPointer pointer, int count, Handedness handedness, object[] tags = null);

        void RaiseInputClicked(IPointer pointer, int count, InputType inputType, Handedness handedness, object[] tags = null);

        void RaisePointerUp(IPointer pointer, object[] tags = null);

        void RaisePointerUp(IPointer pointer, Handedness handedness, object[] tags = null);

        void RaisePointerUp(IPointer pointer, InputType inputType, Handedness handedness, object[] tags = null);

        void RaiseOnInputDown(IInputSource source, object[] tags = null);

        void RaiseOnInputDown(IInputSource source, KeyCode keyCode, object[] tags = null);

        void RaiseOnInputDown(IInputSource source, Handedness handedness, object[] tags = null);

        void RaiseOnInputDown(IInputSource source, KeyCode keyCode, Handedness handedness, object[] tags = null);

        void RaiseOnInputDown(IInputSource source, InputType inputType, Handedness handedness, object[] tags = null);

        void RaiseOnInputPressed(IInputSource source, object[] tags = null);

        void RaiseOnInputPressed(IInputSource source, KeyCode keyCode, object[] tags = null);

        void RaiseOnInputPressed(IInputSource source, double pressAmount, object[] tags = null);

        void RaiseOnInputPressed(IInputSource source, KeyCode keyCode, double pressAmount, object[] tags = null);

        void RaiseOnInputPressed(IInputSource source, double pressAmount, Handedness handedness, object[] tags = null);

        void RaiseOnInputPressed(IInputSource source, KeyCode keyCode, double pressAmount, Handedness handedness, object[] tags = null);

        void RaiseOnInputPressed(IInputSource source, float pressAmount, InputType inputType, Handedness handedness, object[] tags = null);

        void RaiseOnInputUp(IInputSource source, object[] tags = null);

        void RaiseOnInputUp(IInputSource source, KeyCode keyCode, object[] tags = null);

        void RaiseOnInputUp(IInputSource source, Handedness handedness, object[] tags = null);

        void RaiseOnInputUp(IInputSource source, KeyCode keyCode, Handedness handedness, object[] tags = null);

        void RaiseOnInputUp(IInputSource source, InputType inputType, Handedness handedness, object[] tags = null);

        void RaiseDualAxisInputChanged(IInputSource source, InputType inputType, Vector2 inputPosition, object[] tags = null);

        void RaiseDualAxisInputChanged(IInputSource source, InputType inputType, Vector2 inputPosition, Handedness handedness, object[] tags = null);

        void RaiseHoldStarted(IInputSource source, object[] tags = null);

        void RaiseHoldStarted(IInputSource source, Handedness handedness, object[] tags = null);

        void RaiseHoldCompleted(IInputSource source, object[] tags = null);

        void RaiseHoldCompleted(IInputSource source, Handedness handedness, object[] tags = null);

        void RaiseHoldCanceled(IInputSource source, object[] tags = null);

        void RaiseHoldCanceled(IInputSource source, Handedness handedness, object[] tags = null);

        void RaiseNavigationStarted(IInputSource source, object[] tags = null);

        void RaiseNavigationStarted(IInputSource source, Handedness handedness, object[] tags = null);

        void RaiseNavigationUpdated(IInputSource source, Vector3 normalizedOffset, object[] tags = null);

        void RaiseNavigationUpdated(IInputSource source, Vector3 normalizedOffset, Handedness handedness, object[] tags = null);

        void RaiseNavigationCompleted(IInputSource source, Vector3 normalizedOffset, object[] tags = null);

        void RaiseNavigationCompleted(IInputSource source, Vector3 normalizedOffset, Handedness handedness, object[] tags = null);

        void RaiseNavigationCanceled(IInputSource source, object[] tags = null);

        void RaiseNavigationCanceled(IInputSource source, Handedness handedness, object[] tags = null);

        void RaiseManipulationStarted(IInputSource source, object[] tags = null);

        void RaiseManipulationStarted(IInputSource source, Handedness handedness, object[] tags = null);

        void RaiseManipulationUpdated(IInputSource source, Vector3 cumulativeDelta, object[] tags = null);

        void RaiseManipulationUpdated(IInputSource source, Vector3 cumulativeDelta, Handedness handedness, object[] tags = null);

        void RaiseManipulationCompleted(IInputSource source, Vector3 cumulativeDelta, object[] tags = null);

        void RaiseManipulationCompleted(IInputSource source, Vector3 cumulativeDelta, Handedness handedness, object[] tags = null);

        void RaiseManipulationCanceled(IInputSource source, object[] tags = null);

        void RaiseManipulationCanceled(IInputSource source, Handedness handedness, object[] tags = null);

        void RaisePlacingStarted(IInputSource source, GameObject objectBeingPlaced, object[] tags = null);

        void RaisePlacingCompleted(IInputSource source, GameObject objectBeingPlaced, object[] tags = null);

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        void RaiseSpeechKeywordPhraseRecognized(IInputSource source, UnityEngine.Windows.Speech.ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, UnityEngine.Windows.Speech.SemanticMeaning[] semanticMeanings, string text, object[] tags = null);

        void RaiseDictationHypothesis(IInputSource source, string dictationHypothesis, AudioClip dictationAudioClip = null, object[] tags = null);

        void RaiseDictationResult(IInputSource source, string dictationResult, AudioClip dictationAudioClip = null, object[] tags = null);

        void RaiseDictationComplete(IInputSource source, string dictationResult, AudioClip dictationAudioClip, object[] tags = null);

        void RaiseDictationError(IInputSource source, string dictationResult, AudioClip dictationAudioClip = null, object[] tags = null);

#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
    }
}