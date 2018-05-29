// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Events;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
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

        IMixedRealityInputSource RequestNewGenericInputSource(string name, IMixedRealityPointer[] pointers = null);

        /// <summary>
        /// Raise the event that the Input Source was detected.
        /// </summary>
        /// <param name="source">The detected Input Source.</param>
        void RaiseSourceDetected(IMixedRealityInputSource source);

        /// <summary>
        /// Raise the event that the Input Source was lost.
        /// </summary>
        /// <param name="source">The lost Input Source.</param>
        void RaiseSourceLost(IMixedRealityInputSource source);

        /// <summary>
        /// Raise the pre-focus changed event.
        /// <remarks>This event is useful for doing logic before the focus changed event.</remarks>
        /// </summary>
        /// <param name="pointer">The pointer that the focus change event is raised on.</param>
        /// <param name="oldFocusedObject">The old focused object.</param>
        /// <param name="newFocusedObject">The new focused object.</param>
        void RaisePreFocusChangedEvent(IMixedRealityPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject);

        /// <summary>
        /// Raise the focus changed event.
        /// </summary>
        /// <param name="pointer">The pointer that the focus change event is raised on.</param>
        /// <param name="oldFocusedObject">The old focused object.</param>
        /// <param name="newFocusedObject">The new focused object.</param>
        void OnFocusChangedEvent(IMixedRealityPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject);

        /// <summary>
        /// Raise the focus enter event.
        /// </summary>
        /// <param name="pointer">The pointer that has focus.</param>
        /// <param name="focusedObject">The <see cref="GameObject"/> that the pointer has entered focus on.</param>
        void RaiseFocusEnter(IMixedRealityPointer pointer, GameObject focusedObject);

        /// <summary>
        /// Raise the focus exit event.
        /// </summary>
        /// <param name="pointer">The pointer that has lost focus.</param>
        /// <param name="unfocusedObject">The <see cref="GameObject"/> that the pointer has exited focus on.</param>
        void RaiseFocusExit(IMixedRealityPointer pointer, GameObject unfocusedObject);

        /// <summary>
        /// Raise the pointer down event.
        /// </summary>
        /// <param name="pointer">The pointer where the event originates.</param>
        void RaisePointerDown(IMixedRealityPointer pointer);

        /// <summary>
        /// Raise the pointer down event.
        /// </summary>
        /// <param name="pointer">The pointer where the event originates.</param>
        /// <param name="handedness">The handedness of the event.</param>
        void RaisePointerDown(IMixedRealityPointer pointer, Handedness handedness);

        /// <summary>
        /// Raise the pointer down event.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        void RaisePointerDown(IMixedRealityPointer pointer, Handedness handedness, InputAction inputAction);

        /// <summary>
        /// Raise the pointer clicked event.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="count"></param>
        void RaiseInputClicked(IMixedRealityPointer pointer, int count);

        /// <summary>
        /// Raise the pointer clicked event.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="handedness"></param>
        /// <param name="count"></param>
        void RaiseInputClicked(IMixedRealityPointer pointer, Handedness handedness, int count);

        /// <summary>
        /// Raise the pointer clicked event.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="count"></param>
        void RaiseInputClicked(IMixedRealityPointer pointer, Handedness handedness, InputAction inputAction, int count);

        /// <summary>
        /// Raise the pointer up event.
        /// </summary>
        /// <param name="pointer"></param>
        void RaisePointerUp(IMixedRealityPointer pointer);

        /// <summary>
        /// Raise the pointer up event.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="handedness"></param>
        void RaisePointerUp(IMixedRealityPointer pointer, Handedness handedness);

        /// <summary>
        /// Raise the pointer up event.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        void RaisePointerUp(IMixedRealityPointer pointer, Handedness handedness, InputAction inputAction);

        /// <summary>
        /// Raise the input down event.
        /// </summary>
        /// <param name="source"></param>
        void RaiseOnInputDown(IMixedRealityInputSource source);

        /// <summary>
        /// Raise the input down event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keyCode"></param>
        void RaiseOnInputDown(IMixedRealityInputSource source, KeyCode keyCode);

        /// <summary>
        /// Raise the input down event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        void RaiseOnInputDown(IMixedRealityInputSource source, Handedness handedness);

        /// <summary>
        /// Raise the input down event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="keyCode"></param>
        void RaiseOnInputDown(IMixedRealityInputSource source, Handedness handedness, KeyCode keyCode);

        /// <summary>
        /// Raise the input down event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        void RaiseOnInputDown(IMixedRealityInputSource source, Handedness handedness, InputAction inputAction);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keyCode"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, KeyCode keyCode);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pressAmount"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, float pressAmount);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="pressAmount"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, Handedness handedness, float pressAmount);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keyCode"></param>
        /// <param name="pressAmount"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, KeyCode keyCode, float pressAmount);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="pressAmount"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, InputAction inputAction, float pressAmount);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="keyCode"></param>
        /// <param name="pressAmount"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, Handedness handedness, KeyCode keyCode, float pressAmount);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="pressAmount"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, Handedness handedness, InputAction inputAction, float pressAmount);

        /// <summary>
        /// Raise the input up event.
        /// </summary>
        /// <param name="source"></param>
        void RaiseOnInputUp(IMixedRealityInputSource source);

        /// <summary>
        /// Raise the input up event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keyCode"></param>
        void RaiseOnInputUp(IMixedRealityInputSource source, KeyCode keyCode);

        /// <summary>
        /// Raise the input up event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        void RaiseOnInputUp(IMixedRealityInputSource source, Handedness handedness);

        /// <summary>
        /// Raise the input up event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="keyCode"></param>
        void RaiseOnInputUp(IMixedRealityInputSource source, Handedness handedness, KeyCode keyCode);

        /// <summary>
        /// Raise the input up event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        void RaiseOnInputUp(IMixedRealityInputSource source, Handedness handedness, InputAction inputAction);

        /// <summary>
        /// Raise the 2 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void Raise2DoFInputChanged(IMixedRealityInputSource source, InputAction inputAction, Vector2 position);

        /// <summary>
        /// Raise the 2 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void Raise2DoFInputChanged(IMixedRealityInputSource source, Handedness handedness, InputAction inputAction, Vector2 position);

        /// <summary>
        /// Raise the 3 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void Raise3DoFInputChanged(IMixedRealityInputSource source, InputAction inputAction, Vector3 position);

        /// <summary>
        /// Raise the 3 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void Raise3DoFInputChanged(IMixedRealityInputSource source, Handedness handedness, InputAction inputAction, Vector3 position);

        /// <summary>
        /// Raise the 3 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="rotation"></param>
        void Raise3DoFInputChanged(IMixedRealityInputSource source, InputAction inputAction, Quaternion rotation);

        /// <summary>
        /// Raise the 3 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="rotation"></param>
        void Raise3DoFInputChanged(IMixedRealityInputSource source, Handedness handedness, InputAction inputAction, Quaternion rotation);

        /// <summary>
        /// Raise the 6 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="inputData"></param>
        void Raise6DofInputChanged(IMixedRealityInputSource source, InputAction inputAction, Tuple<Vector3, Quaternion> inputData);

        /// <summary>
        /// Raise the 6 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="inputData"></param>
        void Raise6DofInputChanged(IMixedRealityInputSource source, Handedness handedness, InputAction inputAction, Tuple<Vector3, Quaternion> inputData);

        /// <summary>
        /// Raise the hold started input event.
        /// </summary>
        /// <param name="source"></param>
        void RaiseHoldStarted(IMixedRealityInputSource source);

        /// <summary>
        /// Raise the hold started input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        void RaiseHoldStarted(IMixedRealityInputSource source, Handedness handedness);

        /// <summary>
        /// Raise the hold completed input event.
        /// </summary>
        /// <param name="source"></param>
        void RaiseHoldCompleted(IMixedRealityInputSource source);

        /// <summary>
        /// Raise the hold completed input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        void RaiseHoldCompleted(IMixedRealityInputSource source, Handedness handedness);

        /// <summary>
        /// Raise the hold canceled input event.
        /// </summary>
        /// <param name="source"></param>
        void RaiseHoldCanceled(IMixedRealityInputSource source);

        /// <summary>
        /// Raise the hold canceled input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        void RaiseHoldCanceled(IMixedRealityInputSource source, Handedness handedness);

        /// <summary>
        /// Raise the navigation started input event.
        /// </summary>
        /// <param name="source"></param>
        void RaiseNavigationStarted(IMixedRealityInputSource source);

        /// <summary>
        /// Raise the navigation started input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        void RaiseNavigationStarted(IMixedRealityInputSource source, Handedness handedness);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="normalizedOffset"></param>
        void RaiseNavigationUpdated(IMixedRealityInputSource source, Vector3 normalizedOffset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="normalizedOffset"></param>
        void RaiseNavigationUpdated(IMixedRealityInputSource source, Handedness handedness, Vector3 normalizedOffset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="normalizedOffset"></param>
        void RaiseNavigationCompleted(IMixedRealityInputSource source, Vector3 normalizedOffset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="normalizedOffset"></param>
        void RaiseNavigationCompleted(IMixedRealityInputSource source, Handedness handedness, Vector3 normalizedOffset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        void RaiseNavigationCanceled(IMixedRealityInputSource source);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        void RaiseNavigationCanceled(IMixedRealityInputSource source, Handedness handedness);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        void RaiseManipulationStarted(IMixedRealityInputSource source);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        void RaiseManipulationStarted(IMixedRealityInputSource source, Handedness handedness);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cumulativeDelta"></param>
        void RaiseManipulationUpdated(IMixedRealityInputSource source, Vector3 cumulativeDelta);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="cumulativeDelta"></param>
        void RaiseManipulationUpdated(IMixedRealityInputSource source, Handedness handedness, Vector3 cumulativeDelta);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cumulativeDelta"></param>
        void RaiseManipulationCompleted(IMixedRealityInputSource source, Vector3 cumulativeDelta);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="cumulativeDelta"></param>
        void RaiseManipulationCompleted(IMixedRealityInputSource source, Handedness handedness, Vector3 cumulativeDelta);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        void RaiseManipulationCanceled(IMixedRealityInputSource source);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        void RaiseManipulationCanceled(IMixedRealityInputSource source, Handedness handedness);

        #region Windows Speech
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="confidence"></param>
        /// <param name="phraseDuration"></param>
        /// <param name="phraseStartTime"></param>
        /// <param name="semanticMeanings"></param>
        /// <param name="text"></param>
        void RaiseSpeechCommandRecognized(IMixedRealityInputSource source, UnityEngine.Windows.Speech.ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, UnityEngine.Windows.Speech.SemanticMeaning[] semanticMeanings, string text);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dictationHypothesis"></param>
        /// <param name="dictationAudioClip"></param>
        void RaiseDictationHypothesis(IMixedRealityInputSource source, string dictationHypothesis, AudioClip dictationAudioClip = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dictationResult"></param>
        /// <param name="dictationAudioClip"></param>
        void RaiseDictationResult(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dictationResult"></param>
        /// <param name="dictationAudioClip"></param>
        void RaiseDictationComplete(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dictationResult"></param>
        /// <param name="dictationAudioClip"></param>
        void RaiseDictationError(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip = null);

#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
        #endregion Windows Speech
    }
}