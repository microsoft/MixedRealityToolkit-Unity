// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using HoloToolkit.Unity;
using UnityEngine.VR.WSA.Input;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Examples.Prototyping;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// GestureInteractive extends Interactive and handles more advanced gesture events.
    /// </summary>
    public class GestureInteractive : Interactive, ISourceStateHandler
    {
        /// <summary>
        /// Gesture Manipulation states
        /// </summary>
        public enum GestureManipulationState { None, Start, Update, Lost };
        public GestureManipulationState GestureState { get; protected set; }

        private IInputSource mCurrentInputSource;
        private uint mCurrentInputSourceId;

        public float StartDelay = 0;

        /// <summary>
        /// The GestureInteractiveControl to send gesture updates to
        /// </summary>
        public GestureInteractiveControl Control;

        /// <summary>
        /// Should this control hide the cursor during this manipulation?
        /// Provide additional UI if cursor is missing.
        /// </summary>
        public bool HideCursorOnManipulation = false;

        /// <summary>
        /// cached gesture values for computations
        /// </summary>
        private Vector3 mStartHeadPosition;
        private Vector3 mStartHeadRay;
        private Vector3 mStartHandPosition;
        private Vector3 mCurrentHandPosition;
        private HoloToolkit.Unity.InputModule.Cursor mCursor;

        private Ticker mTicker;
        private IInputSource mTempInputSource;
        private uint mTempInputSourceId;

        // Needed to change the control in a UnityEvent inspector.
        public void SetGestureControl(GestureInteractiveControl newControl)
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
                    mTicker = new Ticker(this, StartDelay);
                    mTicker.OnComplete += HandleStartGesture;
                }
                mTicker.Start();
            }
            else
            {
                HandleStartGesture(null, Ticker.TickerEventType.OnComplete);
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

        private void HandleStartGesture(Ticker ticker, Ticker.TickerEventType type)
        {
            InputManager.Instance.ClearModalInputStack();

            // Add self as a modal input handler, to get all inputs during the manipulation
            InputManager.Instance.PushModalInputHandler(gameObject);

            mCurrentInputSource = mTempInputSource;
            mCurrentInputSourceId = mTempInputSourceId;

            mStartHeadPosition = Camera.main.transform.position;
            mStartHeadRay = Camera.main.transform.forward;

            Vector3 handPosition;
            mCurrentInputSource.TryGetPosition(mCurrentInputSourceId, out handPosition);

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

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            // Nothing to do
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            if (mCurrentInputSource != null && eventData.SourceId == mCurrentInputSourceId)
            {
                HandleRelease(true);
            }

            CleanUpTicker();
        }

        private void CleanUpTicker()
        {
            if (mTicker != null)
            {
                mTicker.Stop();
            }
        }

        /// <summary>
        /// Uniform code for both Hololens and Unity Editor, for manipulation complete
        /// What to do about gaze when the manipulation is complete?
        /// </summary>
        private void HandleRelease(bool lost)
        {
            mTempInputSource = null;

            Vector3 handPosition;
            mCurrentInputSource.TryGetPosition(mCurrentInputSourceId, out handPosition);

            mCurrentHandPosition = handPosition;
            Control.ManipulationUpdate(
				mStartHandPosition, 
				mCurrentHandPosition, 
				mStartHeadPosition, 
				mStartHeadRay, 
				lost ? GestureManipulationState.Lost : GestureManipulationState.None);

            InputManager.Instance.ClearModalInputStack();

            if (HasGaze)
            {
                base.OnInputUp(null);
            }
            else
            {
                base.OnInputUp(null);
                base.OnFocusExit();
            }

            mCurrentInputSource = null;

            HandleCursor(false);
        }

        /// <summary>
        /// Works like an Interactive if no manipulation has begun
        /// </summary>
        public override void OnFocusExit()
        {
            //base.OnGazeLeave();
            if (mCurrentInputSource == null)
            {
                base.OnFocusExit();
            }
        }

        /// <summary>
        /// Interactive
        /// </summary>
        public override void OnFocusEnter()
        {
            if (mCurrentInputSource == null)
            {
                base.OnFocusEnter();
            }
        }

        private Vector3 GetCurrentHandPosition()
        {
            Vector3 handPosition;
            mCurrentInputSource.TryGetPosition(mCurrentInputSourceId, out handPosition);

            return handPosition;
        }

        /// <summary>
        /// TODO: build a rule for handling the cursor when a gesture has begun.
        /// Then put it back when the manipulation has ended
        /// </summary>
        /// <param name="state"></param>
        private void HandleCursor(bool state)
        {
            if (state)
            {
                mCursor = GameObject.FindObjectOfType<HoloToolkit.Unity.InputModule.Cursor>();
            }
            
            if (HideCursorOnManipulation && mCursor != null)
            {
                mCursor.gameObject.SetActive(!state);
            }
        }

        /// <summary>
        /// Update gestures
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

        protected override void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            base.KeywordRecognizer_OnPhraseRecognized(args);

            int index;
            //base.KeywordRecognizer_OnPhraseRecognized(args);
            // Check to make sure the recognized keyword matches, then invoke the corresponding method.
            if ((!KeywordRequiresGaze || HasGaze) && mKeywordDictionary != null)
            {
                if (mKeywordDictionary.TryGetValue(args.text, out index))
                {
                    Control.setGestureValue(index);
                }
            }
        }

        protected override void OnDestroy()
        {
            if (mTicker != null)
            {
                mTicker.Stop();
                mTicker.OnComplete -= HandleStartGesture;
                mTicker = null;
            }

            base.OnDestroy();
        }
    }
}
