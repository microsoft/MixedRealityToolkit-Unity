// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.ExecuteEvents;

namespace Microsoft.MixedReality.Toolkit.Input.Utilities
{
    /// <summary>
    /// Event handlers used by the Mixed Reality Toolkit's specific implementation of the <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/>.
    /// </summary>
    public static class MixedRealityEventHandlers
    {
        public static readonly EventFunction<IMixedRealitySourceStateHandler> OnSourceDetectedEventHandler =
        delegate (IMixedRealitySourceStateHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<SourceStateEventData>(eventData);
            handler.OnSourceDetected(casted);
        };

        public static readonly EventFunction<IMixedRealitySourceStateHandler> OnSourceLostEventHandler =
        delegate (IMixedRealitySourceStateHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<SourceStateEventData>(eventData);
            handler.OnSourceLost(casted);
        };

        public static readonly EventFunction<IMixedRealitySourcePoseHandler> OnSourceTrackingChangedEventHandler =
        delegate (IMixedRealitySourcePoseHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<SourcePoseEventData<TrackingState>>(eventData);
            handler.OnSourcePoseChanged(casted);
        };

        public static readonly EventFunction<IMixedRealitySourcePoseHandler> OnSourcePoseVector2ChangedEventHandler =
        delegate (IMixedRealitySourcePoseHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<SourcePoseEventData<Vector2>>(eventData);
            handler.OnSourcePoseChanged(casted);
        };

        public static readonly EventFunction<IMixedRealitySourcePoseHandler> OnSourcePositionChangedEventHandler =
        delegate (IMixedRealitySourcePoseHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<SourcePoseEventData<Vector3>>(eventData);
            handler.OnSourcePoseChanged(casted);
        };

        public static readonly EventFunction<IMixedRealitySourcePoseHandler> OnSourceRotationChangedEventHandler =
        delegate (IMixedRealitySourcePoseHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<SourcePoseEventData<Quaternion>>(eventData);
            handler.OnSourcePoseChanged(casted);
        };

        public static readonly EventFunction<IMixedRealitySourcePoseHandler> OnSourcePoseChangedEventHandler =
        delegate (IMixedRealitySourcePoseHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<SourcePoseEventData<MixedRealityPose>>(eventData);
            handler.OnSourcePoseChanged(casted);
        };

        public static readonly EventFunction<IMixedRealityFocusChangedHandler> OnPreFocusChangedHandler =
        delegate (IMixedRealityFocusChangedHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<FocusEventData>(eventData);
            handler.OnBeforeFocusChange(casted);
        };

        public static readonly EventFunction<IMixedRealityFocusChangedHandler> OnFocusChangedHandler =
        delegate (IMixedRealityFocusChangedHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<FocusEventData>(eventData);
            handler.OnFocusChanged(casted);
        };

        public static readonly EventFunction<IMixedRealityFocusHandler> OnFocusEnterEventHandler =
        delegate (IMixedRealityFocusHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<FocusEventData>(eventData);
            handler.OnFocusEnter(casted);
        };

        public static readonly EventFunction<IMixedRealityFocusHandler> OnFocusExitEventHandler =
        delegate (IMixedRealityFocusHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<FocusEventData>(eventData);
            handler.OnFocusExit(casted);
        };

        public static readonly EventFunction<IMixedRealityPointerHandler> OnPointerDownEventHandler =
        delegate (IMixedRealityPointerHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<MixedRealityPointerEventData>(eventData);
            handler.OnPointerDown(casted);
        };

        public static readonly EventFunction<IMixedRealityPointerHandler> OnPointerDraggedEventHandler =
        delegate (IMixedRealityPointerHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<MixedRealityPointerEventData>(eventData);
            handler.OnPointerDragged(casted);
        };

        public static readonly EventFunction<IMixedRealityPointerHandler> OnInputClickedEventHandler =
        delegate (IMixedRealityPointerHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<MixedRealityPointerEventData>(eventData);
            handler.OnPointerClicked(casted);
        };

        public static readonly EventFunction<IMixedRealityPointerHandler> OnPointerUpEventHandler =
        delegate (IMixedRealityPointerHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<MixedRealityPointerEventData>(eventData);
            handler.OnPointerUp(casted);
        };

        public static readonly EventFunction<IMixedRealityInputHandler> OnInputDownEventHandler =
        delegate (IMixedRealityInputHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData>(eventData);
            handler.OnInputDown(casted);
        };

        public static readonly EventFunction<IMixedRealityBaseInputHandler> OnInputDownWithActionEventHandler =
        delegate (IMixedRealityBaseInputHandler handler, BaseEventData eventData)
        {
            var inputData = ValidateEventData<InputEventData>(eventData);
            Debug.Assert(inputData.MixedRealityInputAction != MixedRealityInputAction.None);

            var inputHandler = handler as IMixedRealityInputHandler;
            if (inputHandler != null)
            {
                inputHandler.OnInputDown(inputData);
            }

            var actionHandler = handler as IMixedRealityInputActionHandler;
            if (actionHandler != null)
            {
                actionHandler.OnActionStarted(inputData);
            }
        };

        public static readonly EventFunction<IMixedRealityInputHandler> OnInputUpEventHandler =
        delegate (IMixedRealityInputHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData>(eventData);
            handler.OnInputUp(casted);
        };

        public static readonly EventFunction<IMixedRealityBaseInputHandler> OnInputUpWithActionEventHandler =
        delegate (IMixedRealityBaseInputHandler handler, BaseEventData eventData)
        {
            var inputData = ValidateEventData<InputEventData>(eventData);
            Debug.Assert(inputData.MixedRealityInputAction != MixedRealityInputAction.None);

            var inputHandler = handler as IMixedRealityInputHandler;
            if (inputHandler != null)
            {
                inputHandler.OnInputUp(inputData);
            }

            var actionHandler = handler as IMixedRealityInputActionHandler;
            if (actionHandler != null)
            {
                actionHandler.OnActionEnded(inputData);
            }
        };

        public static readonly EventFunction<IMixedRealityInputHandler<float>> OnFloatInputChanged =
        delegate (IMixedRealityInputHandler<float> handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData<float>>(eventData);
            handler.OnInputChanged(casted);
        };

        public static readonly EventFunction<IMixedRealityInputHandler<Vector2>> OnTwoDoFInputChanged =
        delegate (IMixedRealityInputHandler<Vector2> handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData<Vector2>>(eventData);
            handler.OnInputChanged(casted);
        };

        public static readonly EventFunction<IMixedRealityInputHandler<Vector3>> OnPositionInputChanged =
        delegate (IMixedRealityInputHandler<Vector3> handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData<Vector3>>(eventData);
            handler.OnInputChanged(casted);
        };

        public static readonly EventFunction<IMixedRealityInputHandler<Quaternion>> OnRotationInputChanged =
        delegate (IMixedRealityInputHandler<Quaternion> handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData<Quaternion>>(eventData);
            handler.OnInputChanged(casted);
        };

        public static readonly EventFunction<IMixedRealityInputHandler<MixedRealityPose>> OnPoseInputChanged =
        delegate (IMixedRealityInputHandler<MixedRealityPose> handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData<MixedRealityPose>>(eventData);
            handler.OnInputChanged(casted);
        };

        public static readonly EventFunction<IMixedRealityGestureHandler> OnGestureStarted =
        delegate (IMixedRealityGestureHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData>(eventData);
            handler.OnGestureStarted(casted);
        };

        public static readonly EventFunction<IMixedRealityBaseInputHandler> OnGestureStartedWithAction =
        delegate (IMixedRealityBaseInputHandler handler, BaseEventData eventData)
        {
            var inputData = ValidateEventData<InputEventData>(eventData);
            Debug.Assert(inputData.MixedRealityInputAction != MixedRealityInputAction.None);

            var gestureHandler = handler as IMixedRealityGestureHandler;
            if (gestureHandler != null)
            {
                gestureHandler.OnGestureStarted(inputData);
            }

            var actionHandler = handler as IMixedRealityInputActionHandler;
            if (actionHandler != null)
            {
                actionHandler.OnActionStarted(inputData);
            }
        };

        public static readonly EventFunction<IMixedRealityGestureHandler> OnGestureUpdated =
        delegate (IMixedRealityGestureHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData>(eventData);
            handler.OnGestureUpdated(casted);
        };

        public static readonly EventFunction<IMixedRealityGestureHandler<Vector2>> OnGestureVector2PositionUpdated =
        delegate (IMixedRealityGestureHandler<Vector2> handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData<Vector2>>(eventData);
            handler.OnGestureUpdated(casted);
        };

        public static readonly EventFunction<IMixedRealityGestureHandler<Vector3>> OnGesturePositionUpdated =
        delegate (IMixedRealityGestureHandler<Vector3> handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData<Vector3>>(eventData);
            handler.OnGestureUpdated(casted);
        };

        public static readonly EventFunction<IMixedRealityGestureHandler<Quaternion>> OnGestureRotationUpdated =
        delegate (IMixedRealityGestureHandler<Quaternion> handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData<Quaternion>>(eventData);
            handler.OnGestureUpdated(casted);
        };

        public static readonly EventFunction<IMixedRealityGestureHandler<MixedRealityPose>> OnGesturePoseUpdated =
        delegate (IMixedRealityGestureHandler<MixedRealityPose> handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData<MixedRealityPose>>(eventData);
            handler.OnGestureUpdated(casted);
        };

        public static readonly EventFunction<IMixedRealityGestureHandler> OnGestureCompleted =
        delegate (IMixedRealityGestureHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData>(eventData);
            handler.OnGestureCompleted(casted);
        };

        public static readonly EventFunction<IMixedRealityBaseInputHandler> OnGestureCompletedWithAction =
        delegate (IMixedRealityBaseInputHandler handler, BaseEventData eventData)
        {
            var inputData = ValidateEventData<InputEventData>(eventData);
            Debug.Assert(inputData.MixedRealityInputAction != MixedRealityInputAction.None);

            var gestureHandler = handler as IMixedRealityGestureHandler;
            if (gestureHandler != null)
            {
                gestureHandler.OnGestureCompleted(inputData);
            }

            var actionHandler = handler as IMixedRealityInputActionHandler;
            if (actionHandler != null)
            {
                actionHandler.OnActionEnded(inputData);
            }
        };

        public static readonly EventFunction<IMixedRealityGestureHandler<Vector2>> OnGestureVector2PositionCompleted =
        delegate (IMixedRealityGestureHandler<Vector2> handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData<Vector2>>(eventData);
            handler.OnGestureCompleted(casted);
        };

        public static readonly EventFunction<IMixedRealityGestureHandler<Vector3>> OnGesturePositionCompleted =
        delegate (IMixedRealityGestureHandler<Vector3> handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData<Vector3>>(eventData);
            handler.OnGestureCompleted(casted);
        };

        public static readonly EventFunction<IMixedRealityGestureHandler<Quaternion>> OnGestureRotationCompleted =
        delegate (IMixedRealityGestureHandler<Quaternion> handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData<Quaternion>>(eventData);
            handler.OnGestureCompleted(casted);
        };

        public static readonly EventFunction<IMixedRealityGestureHandler<MixedRealityPose>> OnGesturePoseCompleted =
        delegate (IMixedRealityGestureHandler<MixedRealityPose> handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData<MixedRealityPose>>(eventData);
            handler.OnGestureCompleted(casted);
        };

        public static readonly EventFunction<IMixedRealityGestureHandler> OnGestureCanceled =
        delegate (IMixedRealityGestureHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData>(eventData);
            handler.OnGestureCanceled(casted);
        };

        public static readonly EventFunction<IMixedRealitySpeechHandler> OnSpeechKeywordRecognizedEventHandler =
        delegate (IMixedRealitySpeechHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<SpeechEventData>(eventData);
            handler.OnSpeechKeywordRecognized(casted);
        };

        public static readonly EventFunction<IMixedRealityBaseInputHandler> OnSpeechKeywordRecognizedWithActionEventHandler =
        delegate (IMixedRealityBaseInputHandler handler, BaseEventData eventData)
        {
            var speechData = ValidateEventData<SpeechEventData>(eventData);
            Debug.Assert(speechData.MixedRealityInputAction != MixedRealityInputAction.None);

            var speechHandler = handler as IMixedRealitySpeechHandler;
            if (speechHandler != null)
            {
                speechHandler.OnSpeechKeywordRecognized(speechData);
            }

            var actionHandler = handler as IMixedRealityInputActionHandler;
            if (actionHandler != null)
            {
                actionHandler.OnActionStarted(speechData);
                actionHandler.OnActionEnded(speechData);
            }
        };

        public static readonly EventFunction<IMixedRealityDictationHandler> OnDictationHypothesisEventHandler =
        delegate (IMixedRealityDictationHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<DictationEventData>(eventData);
            handler.OnDictationHypothesis(casted);
        };

        public static readonly EventFunction<IMixedRealityDictationHandler> OnDictationResultEventHandler =
        delegate (IMixedRealityDictationHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<DictationEventData>(eventData);
            handler.OnDictationResult(casted);
        };

        public static readonly EventFunction<IMixedRealityDictationHandler> OnDictationCompleteEventHandler =
        delegate (IMixedRealityDictationHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<DictationEventData>(eventData);
            handler.OnDictationComplete(casted);
        };

        public static readonly EventFunction<IMixedRealityDictationHandler> OnDictationErrorEventHandler =
        delegate (IMixedRealityDictationHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<DictationEventData>(eventData);
            handler.OnDictationError(casted);
        };

        public static readonly EventFunction<IMixedRealityHandJointHandler> OnHandJointsUpdatedEventHandler =
        delegate (IMixedRealityHandJointHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>>>(eventData);

            handler.OnHandJointsUpdated(casted);
        };

        public static readonly EventFunction<IMixedRealityHandMeshHandler> OnHandMeshUpdatedEventHandler =
        delegate (IMixedRealityHandMeshHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<InputEventData<HandMeshInfo>>(eventData);

            handler.OnHandMeshUpdated(casted);
        };

        public static readonly EventFunction<IMixedRealityTouchHandler> OnTouchStartedEventHandler =
        delegate (IMixedRealityTouchHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<HandTrackingInputEventData>(eventData);
            handler.OnTouchStarted(casted);
        };

        public static readonly EventFunction<IMixedRealityTouchHandler> OnTouchCompletedEventHandler =
        delegate (IMixedRealityTouchHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<HandTrackingInputEventData>(eventData);
            handler.OnTouchCompleted(casted);
        };

        public static readonly EventFunction<IMixedRealityTouchHandler> OnTouchUpdatedEventHandler =
        delegate (IMixedRealityTouchHandler handler, BaseEventData eventData)
        {
            var casted = ValidateEventData<HandTrackingInputEventData>(eventData);
            handler.OnTouchUpdated(casted);
        };
    }
}
