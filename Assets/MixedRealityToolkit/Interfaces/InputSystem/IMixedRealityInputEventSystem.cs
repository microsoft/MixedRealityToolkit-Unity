using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public interface IMixedRealityInputEventSystem
    {
        #region Input Events

        #region Input Source Events

        /// <summary>
        /// Raise the event that the Input Source was detected.
        /// </summary>
        /// <param name="source">The detected Input Source.</param>
        /// <param name="controller"></param>
        void RaiseSourceDetected(IMixedRealityInputSource source, IMixedRealityController controller = null);

        /// <summary>
        /// Raise the event that the Input Source was lost.
        /// </summary>
        /// <param name="source">The lost Input Source.</param>
        /// <param name="controller"></param>
        void RaiseSourceLost(IMixedRealityInputSource source, IMixedRealityController controller = null);

        /// <summary>
        /// Raise the event that the Input Source's tracking state has changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="controller"></param>
        /// <param name="state"></param>
        void RaiseSourceTrackingStateChanged(IMixedRealityInputSource source, IMixedRealityController controller, TrackingState state);

        /// <summary>
        /// Raise the event that the Input Source position was changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="controller"></param>
        /// <param name="position"></param>
        void RaiseSourcePositionChanged(IMixedRealityInputSource source, IMixedRealityController controller, Vector2 position);

        /// <summary>
        /// Raise the event that the Input Source position was changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="controller"></param>
        /// <param name="position"></param>
        void RaiseSourcePositionChanged(IMixedRealityInputSource source, IMixedRealityController controller, Vector3 position);

        /// <summary>
        /// Raise the event that the Input Source position was changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="controller"></param>
        /// <param name="rotation"></param>
        void RaiseSourceRotationChanged(IMixedRealityInputSource source, IMixedRealityController controller, Quaternion rotation);

        /// <summary>
        /// Raise the event that the Input Source position was changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="controller"></param>
        /// <param name="position"></param>
        void RaiseSourcePoseChanged(IMixedRealityInputSource source, IMixedRealityController controller, MixedRealityPose position);

        #endregion Input Source Events

        #region Focus Events

        /// <summary>
        /// Raise the pre-focus changed event.
        /// <remarks>This event is useful for doing logic before the focus changed event.</remarks>
        /// </summary>
        /// <param name="pointer">The pointer that the focus change event is raised on.</param>
        /// <param name="oldFocusedObject">The old focused object.</param>
        /// <param name="newFocusedObject">The new focused object.</param>
        void RaisePreFocusChanged(IMixedRealityPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject);

        /// <summary>
        /// Raise the focus changed event.
        /// </summary>
        /// <param name="pointer">The pointer that the focus change event is raised on.</param>
        /// <param name="oldFocusedObject">The old focused object.</param>
        /// <param name="newFocusedObject">The new focused object.</param>
        void RaiseFocusChanged(IMixedRealityPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject);

        /// <summary>
        /// Raise the focus enter event.
        /// </summary>
        /// <param name="pointer">The pointer that has focus.</param>
        /// <param name="focusedObject">The <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> that the pointer has entered focus on.</param>
        void RaiseFocusEnter(IMixedRealityPointer pointer, GameObject focusedObject);

        /// <summary>
        /// Raise the focus exit event.
        /// </summary>
        /// <param name="pointer">The pointer that has lost focus.</param>
        /// <param name="unfocusedObject">The <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> that the pointer has exited focus on.</param>
        void RaiseFocusExit(IMixedRealityPointer pointer, GameObject unfocusedObject);

        #endregion Focus Events

        #region Pointers

        #region Pointer Down

        /// <summary>
        /// Raise the pointer down event.
        /// </summary>
        /// <param name="pointer">The pointer where the event originates.</param>
        /// <param name="inputAction"></param>
        /// <param name="handedness"></param>
        /// <param name="inputSource"></param>
        void RaisePointerDown(IMixedRealityPointer pointer, MixedRealityInputAction inputAction, Handedness handedness = Handedness.None, IMixedRealityInputSource inputSource = null);

        #endregion Pointer Down

        #region Pointer Click

        /// <summary>
        /// Raise the pointer clicked event.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="inputAction"></param>
        /// <param name="count"></param>
        /// <param name="handedness"></param>
        /// <param name="inputSource"></param>
        void RaisePointerClicked(IMixedRealityPointer pointer, MixedRealityInputAction inputAction, int count, Handedness handedness = Handedness.None, IMixedRealityInputSource inputSource = null);

        #endregion Pointer Click

        #region Pointer Up

        /// <summary>
        /// Raise the pointer up event.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="inputAction"></param>
        /// <param name="handedness"></param>
        /// <param name="inputSource"></param>
        void RaisePointerUp(IMixedRealityPointer pointer, MixedRealityInputAction inputAction, Handedness handedness = Handedness.None, IMixedRealityInputSource inputSource = null);

        #endregion Pointer Up

        #endregion Pointers

        #region Generic Input Events

        #region Input Down

        /// <summary>
        /// Raise the input down event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        void RaiseOnInputDown(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction);

        #endregion Input Down

        #region Input Up

        /// <summary>
        /// Raise the input up event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        void RaiseOnInputUp(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction);

        #endregion Input Up

        #region Float Input Changed

        /// <summary>
        /// Raise Float Input Changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="inputValue"></param>
        void RaiseFloatInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, float inputValue);

        #endregion Float Input Changed

        #region Input Position Changed

        /// <summary>
        /// Raise the 2 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void RaisePositionInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, Vector2 position);

        /// <summary>
        /// Raise the 3 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void RaisePositionInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, Vector3 position);

        #endregion Input Position Changed

        #region Input Rotation Changed

        /// <summary>
        /// Raise the 3 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="rotation"></param>
        void RaiseRotationInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, Quaternion rotation);

        #endregion Input Rotation Changed

        #region Input Pose Changed

        /// <summary>
        /// Raise the 6 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="inputData"></param>
        void RaisePoseInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, MixedRealityPose inputData);

        #endregion Input Pose Changed

        #endregion Generic Input Events

        #region Generic Gesture Events

        /// <summary>
        /// Raise the Gesture Started Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        void RaiseGestureStarted(IMixedRealityController controller, MixedRealityInputAction action);

        /// <summary>
        /// Raise the Gesture Updated Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action);

        /// <summary>
        /// Raise the Gesture Updated Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action, Vector2 inputData);

        /// <summary>
        /// Raise the Gesture Updated Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action, Vector3 inputData);

        /// <summary>
        /// Raise the Gesture Updated Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action, Quaternion inputData);

        /// <summary>
        /// Raise the Gesture Updated Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action, MixedRealityPose inputData);

        /// <summary>
        /// Raise the Gesture Completed Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action);

        /// <summary>
        /// Raise the Gesture Completed Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action, Vector2 inputData);

        /// <summary>
        /// Raise the Gesture Completed Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action, Vector3 inputData);

        /// <summary>
        /// Raise the Gesture Completed Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action, Quaternion inputData);

        /// <summary>
        /// Raise the Gesture Completed Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action, MixedRealityPose inputData);

        /// <summary>
        /// Raise the Gesture Canceled Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        void RaiseGestureCanceled(IMixedRealityController controller, MixedRealityInputAction action);

        #endregion

        #region Speech Keyword Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="confidence"></param>
        /// <param name="phraseDuration"></param>
        /// <param name="phraseStartTime"></param>
        /// <param name="command"></param>
        void RaiseSpeechCommandRecognized(IMixedRealityInputSource source, RecognitionConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, SpeechCommands command);

        #endregion Speech Keyword Events

        #region Dictation Events

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

        #endregion Dictation Events

        #region Hand Events

        /// <summary>
        /// Notify system that articulated hand joint info has been updated
        /// </summary>
        void RaiseHandJointsUpdated(IMixedRealityInputSource source, Handedness handedness, IDictionary<TrackedHandJoint, MixedRealityPose> jointPoses);

        /// <summary>
        /// Notify system that articulated hand mesh has been updated
        /// </summary>
        void RaiseHandMeshUpdated(IMixedRealityInputSource source, Handedness handedness, HandMeshInfo handMeshInfo);

        void RaiseOnTouchStarted(IMixedRealityInputSource source, IMixedRealityController controller, Handedness handedness, Vector3 touchPoint);

        void RaiseOnTouchUpdated(IMixedRealityInputSource source, IMixedRealityController controller, Handedness handedness, Vector3 touchPoint);

        void RaiseOnTouchCompleted(IMixedRealityInputSource source, IMixedRealityController controller, Handedness handedness, Vector3 touchPoint);

        #endregion Hand Events

        #endregion Input Events
    }
}
