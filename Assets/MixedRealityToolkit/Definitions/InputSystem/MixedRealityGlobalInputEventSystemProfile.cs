using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Configuration profile settings for setting configuring global input event responses
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Global Input Event System Profile", fileName = "MixedRealityGlobalInputEventSystemProfile", order = (int)CreateProfileMenuItemIndices.InputActions)]
    public class MixedRealityGlobalInputEventSystemProfile : BaseMixedRealityProfile, IMixedRealityGlobalInputEventSystem
    {
        public virtual void RaiseSourceDetected(IMixedRealityInputSource source, IMixedRealityController controller = null)
        {
        }

        public virtual void RaiseSourceLost(IMixedRealityInputSource source, IMixedRealityController controller = null)
        {
        }

        public virtual void RaiseSourceTrackingStateChanged(IMixedRealityInputSource source, IMixedRealityController controller,
            TrackingState state)
        {
        }

        public virtual void RaiseSourcePositionChanged(IMixedRealityInputSource source, IMixedRealityController controller, Vector2 position)
        {
        }

        public virtual void RaiseSourcePositionChanged(IMixedRealityInputSource source, IMixedRealityController controller, Vector3 position)
        {
        }

        public virtual void RaiseSourceRotationChanged(IMixedRealityInputSource source, IMixedRealityController controller,
            Quaternion rotation)
        {
        }

        public virtual void RaiseSourcePoseChanged(IMixedRealityInputSource source, IMixedRealityController controller,
            MixedRealityPose position)
        {
        }

        public virtual void RaisePreFocusChanged(IMixedRealityPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
        }

        public virtual void RaiseFocusChanged(IMixedRealityPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
        }

        public virtual void RaiseFocusEnter(IMixedRealityPointer pointer, GameObject focusedObject)
        {
        }

        public virtual void RaiseFocusExit(IMixedRealityPointer pointer, GameObject unfocusedObject)
        {
        }

        public virtual void RaisePointerDown(IMixedRealityPointer pointer, MixedRealityInputAction inputAction,
            Handedness handedness = Handedness.None, IMixedRealityInputSource inputSource = null)
        {
        }

        public virtual void RaisePointerClicked(IMixedRealityPointer pointer, MixedRealityInputAction inputAction, int count,
            Handedness handedness = Handedness.None, IMixedRealityInputSource inputSource = null)
        {
        }

        public virtual void RaisePointerUp(IMixedRealityPointer pointer, MixedRealityInputAction inputAction,
            Handedness handedness = Handedness.None, IMixedRealityInputSource inputSource = null)
        {
        }

        public virtual void RaiseOnInputDown(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
        }

        public virtual void RaiseOnInputUp(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
        }

        public virtual void RaiseFloatInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction,
            float inputValue)
        {
        }

        public virtual void RaisePositionInputChanged(IMixedRealityInputSource source, Handedness handedness,
            MixedRealityInputAction inputAction, Vector2 position)
        {
        }

        public virtual void RaisePositionInputChanged(IMixedRealityInputSource source, Handedness handedness,
            MixedRealityInputAction inputAction, Vector3 position)
        {
        }

        public virtual void RaiseRotationInputChanged(IMixedRealityInputSource source, Handedness handedness,
            MixedRealityInputAction inputAction, Quaternion rotation)
        {
        }

        public virtual void RaisePoseInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction,
            MixedRealityPose inputData)
        {
        }

        public virtual void RaiseGestureStarted(IMixedRealityController controller, MixedRealityInputAction action)
        {
        }

        public virtual void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action)
        {
        }

        public virtual void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action, Vector2 inputData)
        {
        }

        public virtual void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action, Vector3 inputData)
        {
        }

        public virtual void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action, Quaternion inputData)
        {
        }

        public virtual void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action,
            MixedRealityPose inputData)
        {
        }

        public virtual void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action)
        {
        }

        public virtual void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action, Vector2 inputData)
        {
        }

        public virtual void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action, Vector3 inputData)
        {
        }

        public virtual void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action, Quaternion inputData)
        {
        }

        public virtual void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action,
            MixedRealityPose inputData)
        {
        }

        public virtual void RaiseGestureCanceled(IMixedRealityController controller, MixedRealityInputAction action)
        {
        }

        public virtual void RaiseSpeechCommandRecognized(IMixedRealityInputSource source, RecognitionConfidenceLevel confidence,
            TimeSpan phraseDuration, DateTime phraseStartTime, SpeechCommands command)
        {
        }

        public virtual void RaiseDictationHypothesis(IMixedRealityInputSource source, string dictationHypothesis,
            AudioClip dictationAudioClip = null)
        {
        }

        public virtual void RaiseDictationResult(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip = null)
        {
        }

        public virtual void RaiseDictationComplete(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip)
        {
        }

        public virtual void RaiseDictationError(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip = null)
        {
        }

        public virtual void RaiseHandJointsUpdated(IMixedRealityInputSource source, Handedness handedness, IDictionary<TrackedHandJoint, MixedRealityPose> jointPoses)
        {
        }

        public virtual void RaiseHandMeshUpdated(IMixedRealityInputSource source, Handedness handedness, HandMeshInfo handMeshInfo)
        {
        }

        public virtual void RaiseOnTouchStarted(IMixedRealityInputSource source, IMixedRealityController controller, Handedness handedness,
            Vector3 touchPoint)
        {
        }

        public virtual void RaiseOnTouchUpdated(IMixedRealityInputSource source, IMixedRealityController controller, Handedness handedness,
            Vector3 touchPoint)
        {
        }

        public virtual void RaiseOnTouchCompleted(IMixedRealityInputSource source, IMixedRealityController controller, Handedness handedness,
            Vector3 touchPoint)
        {
        }
    }
}
