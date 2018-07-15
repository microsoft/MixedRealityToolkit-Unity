// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.UX
{
    /// <summary>
    /// GestureInteractive extends Interactive and handles more advanced gesture events.
    /// On Press a gesture begins and on release the gesture ends.
    /// Raw gesture data (hand position and gesture state) is passed to a GestureInteractiveController.
    /// Gestures can also be performed with code or voice, see more details below.
    /// </summary>
    public class GestureInteractive : Interactive, IMixedRealitySourceStateHandler, IMixedRealitySpeechHandler
    {
        /// <summary>
        /// Gesture Manipulation states
        /// </summary>
        public enum GestureManipulationState { None, Start, Update, Lost }
        public GestureManipulationState GestureState { get; protected set; }

        private IMixedRealityInputSource mCurrentInputSource;
        private uint mCurrentInputSourceId;

        [Tooltip("Sets the time before the gesture starts after a press has occurred, handy when a select event is also being used")]
        public float StartDelay;

        [Tooltip("The GestureInteractiveControl to send gesture updates to")]
        public InteractiveGestureControl Control;

        /// <summary>
        /// Provide additional UI for gesture feedback.
        /// </summary>
        [Tooltip("Should this control hide the cursor during this manipulation?")]
        public bool HideCursorOnManipulation;

        /// <summary>
        /// cached gesture values for computations
        /// </summary>
        private Vector3 mStartHeadPosition;
        private Vector3 mStartHeadRay;
        private Vector3 mStartHandPosition;
        private Vector3 mCurrentHandPosition;
        private Cursor mCursor;

        private Coroutine mTicker;
        private IMixedRealityInputSource mTempInputSource;
        private uint mTempInputSourceId;

        protected override void Awake()
        {
            base.Awake();

            // get the gestureInteractiveControl if not previously set
            // This could reside on another GameObject, so we will not require this to exist on this game object.
            if (Control == null)
            {
                Control = GetComponent<InteractiveGestureControl>();
            }
        }

        /// <summary>
        /// Change the control in code or in a UnityEvent inspector.
        /// </summary>
        /// <param name="newControl"></param>
        public void SetGestureControl(InteractiveGestureControl newControl)
        {
            Control = newControl;
        }

        /// <summary>
        /// The press event runs before all other gesture based events, so it's safe to register Manipulation events here
        /// </summary>
        public override void OnInputDown(InputEventData eventData)
        {
            base.OnInputDown(eventData);

            mTempInputSource = eventData.InputSource;
            mTempInputSourceId = eventData.SourceId;

            if (StartDelay > 0)
            {
                if (mTicker == null)
                {
                    mTicker = StartCoroutine(Ticker(StartDelay));
                }
            }
            else
            {
                HandleStartGesture();
            }
        }

        // Makes sure when a gesture interactive gets cleared the input source gets the gesture lost event.
        public static void ClearGestureModalInput(GameObject source)
        {
            // Stack could hold a reference that's been removed.
            if (source == null)
            {
                return;
            }

            GestureInteractive gesture = source.GetComponent<GestureInteractive>();
            if (gesture == null)
            {
                return;
            }

            gesture.HandleRelease(false);
            gesture.CleanUpTicker();
        }

        private IEnumerator Ticker(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            HandleStartGesture();
        }

        /// <summary>
        /// Start the gesture
        /// </summary>
        private void HandleStartGesture()
        {
            InputSystem.ClearModalInputStack();

            // Add self as a modal input handler, to get all inputs during the manipulation
            InputSystem.PushModalInputHandler(gameObject);

            mCurrentInputSource = mTempInputSource;
            mCurrentInputSourceId = mTempInputSourceId;

            mStartHeadPosition = CameraCache.Main.transform.position;
            mStartHeadRay = CameraCache.Main.transform.forward;

            //TODO - Add pose input
            Vector3 handPosition = Vector3.zero;
            //mCurrentInputSource.TryGetGripPosition(mCurrentInputSourceId, out handPosition);

            mStartHandPosition = handPosition;
            mCurrentHandPosition = handPosition;
            Control.ManipulationUpdate(mStartHandPosition, mStartHandPosition, mStartHeadPosition, mStartHeadRay, GestureManipulationState.Start);
            HandleCursor(true);
        }

        /// <summary>
        /// ignore this event at face value, the user may roll off the interactive while performing a gesture,
        /// use the ManipulationComplete event instead
        /// </summary>
        public override void OnInputUp(InputEventData eventData)
        {
            //base.OnInputUp(eventData);
            if (mCurrentInputSource != null && (eventData == null || eventData.SourceId == mCurrentInputSourceId))
            {
                HandleRelease(false);
            }

            CleanUpTicker();
        }

        /// <summary>
        /// required by ISourceStateHandler
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSourceDetected(SourceStateEventData eventData)
        {
            // Nothing to do
        }

        /// <summary>
        /// Stops the gesture when the source is lost
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSourceLost(SourceStateEventData eventData)
        {
            if (mCurrentInputSource != null && eventData.SourceId == mCurrentInputSourceId)
            {
                HandleRelease(true);
            }

            CleanUpTicker();
        }

        /// <summary>
        /// manages the timer
        /// </summary>
        private void CleanUpTicker()
        {
            if (mTicker != null)
            {
                StopCoroutine(mTicker);
                mTicker = null;
            }
        }

        /// <summary>
        /// Uniform code for different types of manipulation complete (stopped, source lost, etc..)
        /// </summary>
        private void HandleRelease(bool lost)
        {
            mTempInputSource = null;

            Vector3 handPosition = GetCurrentHandPosition();

            mCurrentHandPosition = handPosition;
            Control.ManipulationUpdate(
                mStartHandPosition,
                mCurrentHandPosition,
                mStartHeadPosition,
                mStartHeadRay,
                lost ? GestureManipulationState.Lost : GestureManipulationState.None);

            InputSystem.ClearModalInputStack();

            if (HasGaze)
            {
                base.OnInputUp(null);
            }
            else
            {
                base.OnInputUp(null);
                base.OnFocusExit(currentFocusData);
            }

            mCurrentInputSource = null;

            HandleCursor(false);
        }

        /// <summary>
        /// Hand position
        /// </summary>
        /// <returns></returns>
        private Vector3 GetCurrentHandPosition()
        {
            Vector3 handPosition = Vector3.zero;

            //TODO - Choosing Grip or Pointer?
//#if UNITY_2017_2_OR_NEWER
//            mCurrentInputSource.TryGetGripPosition(mCurrentInputSourceId, out handPosition);
//#else
//            mCurrentInputSource.TryGetPointerPosition(mCurrentInputSourceId, out handPosition);
//#endif
            return handPosition;
        }

        /// <summary>
        /// Hide the cursor during the gesture
        /// </summary>
        /// <param name="state"></param>
        private void HandleCursor(bool state)
        {
            // Hack for now.
            // TODO: Update Cursor Modifier to handle HideOnGesture, then calculate visibility so cursors can handle this correctly
            // TODO: Cursor handling?
            //if (state)
            //{
            //    mCursor = FindObjectOfType<Cursor>();
            //}

            //if (HideCursorOnManipulation && mCursor != null)
            //{
            //    mCursor.SetVisibility(!state);
            //}
        }

        /// <summary>
        /// Update gestures and send gesture data to GestureInteractiveController
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (mCurrentInputSource != null)
            {
                mCurrentHandPosition = GetCurrentHandPosition();
                Control.ManipulationUpdate(mStartHandPosition, mCurrentHandPosition, mStartHeadPosition, mStartHeadRay, GestureManipulationState.Update);
            }
        }

        /// <summary>
        /// From Interactive, but customized for triggering gestures from keywords
        /// Handle the manipulation in the GestureInteractiveControl
        /// </summary>
        /// <param name="args"></param>
        public override void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            base.OnSpeechKeywordRecognized(eventData);

            // Check to make sure the recognized keyword matches, then invoke the corresponding method.
            if ((!KeywordRequiresGaze || HasGaze))
            {
                //TODO - handle speech
                //int index;
                //if (mKeywordDictionary.TryGetValue(args.text, out index))
                //{
                //    Control.setGestureValue(index);
                //}
            }
        }

        /// <summary>
        /// Clean up
        /// </summary>
        protected override void OnDestroy()
        {
            if (mTicker != null)
            {
                StopCoroutine(mTicker);
                mTicker = null;
            }

            base.OnDestroy();
        }
    }
}