// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem
{
    /// <summary>
    /// Manager interface for a Input system in the Mixed Reality Toolkit
    /// All replacement systems for providing Input System functionality should derive from this interface
    /// </summary>
    public interface IMixedRealityInputSystem : IMixedRealityEventSystem
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
        /// The current Focus Provider that's been implemented by this Input System.
        /// </summary>
        IMixedRealityFocusProvider FocusProvider { get; }

        /// <summary>
        /// The current Gaze Provider that's been implemented by this Input System.
        /// </summary>
        IMixedRealityGazeProvider GazeProvider { get; }

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

        /// <summary>
        /// Generates a new unique input source id.<para/>
        /// <remarks>All Input Sources are required to call this method in their constructor or initialization.</remarks>
        /// </summary>
        /// <returns>a new unique Id for the input source.</returns>
        uint GenerateNewSourceId();

        void RaiseSourceDetected(IMixedRealityInputSource source, object[] tags = null);

        void RaiseSourceLost(IMixedRealityInputSource source, object[] tags = null);

        void RaiseSourcePositionChanged(IMixedRealityInputSource source, Vector3 pointerPosition, Vector3 gripPosition, object[] tags = null);

        void RaiseSourcePositionChanged(IMixedRealityInputSource source, Handedness sourceHandedness, Vector3 pointerPosition, Vector3 gripPosition, object[] tags = null);

        void RaiseSourceRotationChanged(IMixedRealityInputSource source, Quaternion pointerRotation, Quaternion gripRotation, object[] tags = null);

        void RaiseSourceRotationChanged(IMixedRealityInputSource source, Handedness sourceHandedness, Quaternion pointerRotation, Quaternion gripRotation, object[] tags = null);

        void RaisePreFocusChangedEvent(IMixedRealityPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject);

        void OnFocusChangedEvent(IMixedRealityPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject);

        void RaiseFocusEnter(IMixedRealityPointer pointer, GameObject focusedObject);

        void RaiseFocusExit(IMixedRealityPointer pointer, GameObject unfocusedObject);

        void RaisePointerDown(IMixedRealityPointer pointer, object[] tags = null);

        void RaisePointerDown(IMixedRealityPointer pointer, Handedness handedness, object[] tags = null);

        void RaisePointerDown(IMixedRealityPointer pointer, Handedness handedness, InputType inputType, object[] tags = null);

        void RaiseInputClicked(IMixedRealityPointer pointer, int tapCount, object[] tags = null);

        void RaiseInputClicked(IMixedRealityPointer pointer, Handedness handedness, int count, object[] tags = null);

        void RaiseInputClicked(IMixedRealityPointer pointer, Handedness handedness, InputType inputType, int count, object[] tags = null);

        void RaisePointerUp(IMixedRealityPointer pointer, object[] tags = null);

        void RaisePointerUp(IMixedRealityPointer pointer, Handedness handedness, object[] tags = null);

        void RaisePointerUp(IMixedRealityPointer pointer, Handedness handedness, InputType inputType, object[] tags = null);

        void RaiseOnInputDown(IMixedRealityInputSource source, object[] tags = null);

        void RaiseOnInputDown(IMixedRealityInputSource source, KeyCode keyCode, object[] tags = null);

        void RaiseOnInputDown(IMixedRealityInputSource source, Handedness handedness, object[] tags = null);

        void RaiseOnInputDown(IMixedRealityInputSource source, Handedness handedness, KeyCode keyCode, object[] tags = null);

        void RaiseOnInputDown(IMixedRealityInputSource source, Handedness handedness, InputType inputType, object[] tags = null);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="tags"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, object[] tags = null);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keyCode"></param>
        /// <param name="tags"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, KeyCode keyCode, object[] tags = null);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pressAmount"></param>
        /// <param name="tags"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, double pressAmount, object[] tags = null);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="pressAmount"></param>
        /// <param name="tags"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, Handedness handedness, double pressAmount, object[] tags = null);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keyCode"></param>
        /// <param name="pressAmount"></param>
        /// <param name="tags"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, KeyCode keyCode, double pressAmount, object[] tags = null);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputType"></param>
        /// <param name="pressAmount"></param>
        /// <param name="tags"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, InputType inputType, double pressAmount, object[] tags = null);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="keyCode"></param>
        /// <param name="pressAmount"></param>
        /// <param name="tags"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, Handedness handedness, KeyCode keyCode, double pressAmount, object[] tags = null);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputType"></param>
        /// <param name="pressAmount"></param>
        /// <param name="tags"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, Handedness handedness, InputType inputType, float pressAmount, object[] tags = null);

        void RaiseOnInputUp(IMixedRealityInputSource source, object[] tags = null);

        void RaiseOnInputUp(IMixedRealityInputSource source, KeyCode keyCode, object[] tags = null);

        void RaiseOnInputUp(IMixedRealityInputSource source, Handedness handedness, object[] tags = null);

        void RaiseOnInputUp(IMixedRealityInputSource source, Handedness handedness, KeyCode keyCode, object[] tags = null);

        void RaiseOnInputUp(IMixedRealityInputSource source, Handedness handedness, InputType inputType, object[] tags = null);

        void RaiseDualAxisInputChanged(IMixedRealityInputSource source, InputType inputType, Vector2 inputPosition, object[] tags = null);

        void RaiseDualAxisInputChanged(IMixedRealityInputSource source, Handedness handedness, InputType inputType, Vector2 inputPosition, object[] tags = null);

        void RaiseHoldStarted(IMixedRealityInputSource source, object[] tags = null);

        void RaiseHoldStarted(IMixedRealityInputSource source, Handedness handedness, object[] tags = null);

        void RaiseHoldCompleted(IMixedRealityInputSource source, object[] tags = null);

        void RaiseHoldCompleted(IMixedRealityInputSource source, Handedness handedness, object[] tags = null);

        void RaiseHoldCanceled(IMixedRealityInputSource source, object[] tags = null);

        void RaiseHoldCanceled(IMixedRealityInputSource source, Handedness handedness, object[] tags = null);

        void RaiseNavigationStarted(IMixedRealityInputSource source, object[] tags = null);

        void RaiseNavigationStarted(IMixedRealityInputSource source, Handedness handedness, object[] tags = null);

        void RaiseNavigationUpdated(IMixedRealityInputSource source, Vector3 normalizedOffset, object[] tags = null);

        void RaiseNavigationUpdated(IMixedRealityInputSource source, Handedness handedness, Vector3 normalizedOffset, object[] tags = null);

        void RaiseNavigationCompleted(IMixedRealityInputSource source, Vector3 normalizedOffset, object[] tags = null);

        void RaiseNavigationCompleted(IMixedRealityInputSource source, Handedness handedness, Vector3 normalizedOffset, object[] tags = null);

        void RaiseNavigationCanceled(IMixedRealityInputSource source, object[] tags = null);

        void RaiseNavigationCanceled(IMixedRealityInputSource source, Handedness handedness, object[] tags = null);

        void RaiseManipulationStarted(IMixedRealityInputSource source, object[] tags = null);

        void RaiseManipulationStarted(IMixedRealityInputSource source, Handedness handedness, object[] tags = null);

        void RaiseManipulationUpdated(IMixedRealityInputSource source, Vector3 cumulativeDelta, object[] tags = null);

        void RaiseManipulationUpdated(IMixedRealityInputSource source, Handedness handedness, Vector3 cumulativeDelta, object[] tags = null);

        void RaiseManipulationCompleted(IMixedRealityInputSource source, Vector3 cumulativeDelta, object[] tags = null);

        void RaiseManipulationCompleted(IMixedRealityInputSource source, Handedness handedness, Vector3 cumulativeDelta, object[] tags = null);

        void RaiseManipulationCanceled(IMixedRealityInputSource source, object[] tags = null);

        void RaiseManipulationCanceled(IMixedRealityInputSource source, Handedness handedness, object[] tags = null);

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        void RaiseSpeechKeywordPhraseRecognized(IMixedRealityInputSource source, UnityEngine.Windows.Speech.ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, UnityEngine.Windows.Speech.SemanticMeaning[] semanticMeanings, string text, object[] tags = null);

        void RaiseDictationHypothesis(IMixedRealityInputSource source, string dictationHypothesis, AudioClip dictationAudioClip = null, object[] tags = null);

        void RaiseDictationResult(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip = null, object[] tags = null);

        void RaiseDictationComplete(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip, object[] tags = null);

        void RaiseDictationError(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip = null, object[] tags = null);

#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
    }
}