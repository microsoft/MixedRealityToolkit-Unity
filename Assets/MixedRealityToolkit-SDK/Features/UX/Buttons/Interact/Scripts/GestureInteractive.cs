// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using HoloToolkit.Unity;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using UnityEngine;

#if UNITY_WSA || UNITY_STANDALONE_WIN
using UnityEngine.Windows.Speech;
#endif

namespace Interact
{
    /// <summary>
    /// GestureInteractive extends Interactive and handles more advanced gesture events.
    /// On Press a gesture begins and on release the gesture ends.
    /// Raw gesture data (hand position and gesture state) is passed to a GestureInteractiveController.
    /// Gestures can also be performed with code or voice, see more details below.
    /// </summary>
    
    public struct GestureHandData
    {
        public Vector3 StartHeadOrigin;
        public Vector3 StartHeadRay;
        public Vector3 StartGesturePosition;
        public Vector3 CurrentGesturePosition;
        public GestureInteractive.GestureManipulationState State;
    }
    
    public class GestureInteractive : Interactive // TEMPISourceStateHandler
    {
        /// <summary>
        /// Gesture Manipulation states
        /// </summary>
        public enum GestureManipulationState { None, Start, Update, Lost }
        public GestureManipulationState GestureState { get; protected set; }

        private Object currentInputSource; // TEMP
        private uint currentInputSourceId;

        [Tooltip("Sets the time before the gesture starts after a press has occured, handy when a select event is also being used")]
        public float StartDelay = 0;

        [Tooltip("The GestureInteractiveControl to send gesture updates to")]
        public GestureInteractiveControl Control;

        /// <summary>
        /// Provide additional UI for gesture feedback.
        /// </summary>
        [Tooltip("Should this control hide the cursor during this manipulation?")]
        public bool HideCursorOnManipulation = false;

        /// <summary>
        /// cached gesture values for computations
        /// </summary>
        private Vector3 startHeadPosition;
        private Vector3 startHeadRay;
        private Vector3 startHandPosition;
        private Vector3 currentHandPosition;
        private Cursor cursor;

        private Coroutine ticker;
        private Object tempInputSource;// TEMP
        private uint tempInputSourceId;

        private float tapDistanceThreshold = 0.001f;

        protected virtual void Awake()
        {
            // get the gestureInteractiveControl if not previously set
            // This could reside on another GameObject, so we will not require this to exist on this game object.
            if (Control == null)
            {
                Control = GetComponent<GestureInteractiveControl>();
            }
        }

        /// <summary>
        /// Change the control in code or in a UnityEvent inspector.
        /// </summary>
        /// <param name="newControl"></param>
        public void SetGestureControl(GestureInteractiveControl newControl)
        {
            Control = newControl;
        }

        public GestureHandData GetGestureHandData()
        {
            GestureHandData data = new GestureHandData();
            data.CurrentGesturePosition = currentHandPosition;
            data.StartGesturePosition = startHandPosition;
            data.StartHeadOrigin = startHeadPosition;
            data.StartHeadRay = startHeadRay;
            data.State = GestureState;

            return data;
        }

        /// <summary>
        /// The press event runs before all other gesture based events, so it's safe to register Manipulation events here
        /// </summary>
        public override void OnInputDown(Object eventData)// TEMP
        {
            // TEMP base.OnInputDown(eventData);

            // TEMP tempInputSource = eventData.InputSource;
            // TEMP tempInputSourceId = eventData.SourceId;

            if (StartDelay > 0)
            {
                if (ticker == null)
                {
                    ticker = StartCoroutine(Ticker(StartDelay));
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
        /// <param name="ticker"></param>
        /// <param name="type"></param>
        private void HandleStartGesture()
        {
            // TEMP InputManager.Instance.ClearModalInputStack();

            // Add self as a modal input handler, to get all inputs during the manipulation
            // TEMP InputManager.Instance.PushModalInputHandler(gameObject);

            currentInputSource = tempInputSource;
            currentInputSourceId = tempInputSourceId;

            // TEMP startHeadPosition = CameraCache.Main.transform.position;
            // TEMP startHeadRay = CameraCache.Main.transform.forward;

            Vector3 handPosition;
            // TEMP currentInputSource.TryGetGripPosition(currentInputSourceId, out handPosition);

            // TEMP startHandPosition = handPosition;
            // TEMP currentHandPosition = handPosition;

            GestureState = GestureManipulationState.Start;

            if (Control != null)
            {
                Control.ManipulationUpdate(startHandPosition, startHandPosition, startHeadPosition, startHeadRay, GestureManipulationState.Start);
            }

            HandleCursor(true);
        }

        /// <summary>
        /// ignore this event at face value, the user may roll off the interactive while performing a gesture,
        /// use the ManipulationComplete event instead
        /// </summary>
        public override void OnInputUp(Object eventData)// TEMP
        {
            //base.OnInputUp(eventData);
            if (currentInputSource != null && (eventData == null ))// TEMP|| eventData.SourceId == currentInputSourceId))
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
            if (currentInputSource != null && eventData.SourceId == currentInputSourceId)
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
            if (ticker != null)
            {
                StopCoroutine(ticker);
                ticker = null;
            }
        }

        /// <summary>
        /// Uniform code for different types of manipulation complete (stopped, source lost, etc..)
        /// </summary>
        private void HandleRelease(bool lost)
        {
            tempInputSource = null;

            Vector3 handPosition;
            // TEMP currentInputSource.TryGetGripPosition(currentInputSourceId, out handPosition);

            // TEMP currentHandPosition = handPosition;

            GestureState = lost ? GestureManipulationState.Lost : GestureManipulationState.None;

            if (Control != null)
                Control.ManipulationUpdate(
                    startHandPosition,
                    currentHandPosition,
                    startHeadPosition,
                    startHeadRay,
                    lost ? GestureManipulationState.Lost : GestureManipulationState.None);

            // TEMP InputManager.Instance.ClearModalInputStack();

            if (HasGaze)
            {
                base.OnInputUp(null);
            }
            else
            {
                base.OnInputUp(null);
                base.OnFocusExit();
            }

            currentInputSource = null;

            HandleCursor(false);
        }

        /// <summary>
        /// Works like an Interactive if no manipulation has begun
        /// </summary>
        public override void OnFocusExit()
        {
            //base.OnGazeLeave();
            if (currentInputSource == null)
            {
                base.OnFocusExit();
            }
        }

        /// <summary>
        /// Interactive
        /// </summary>
        public override void OnFocusEnter()
        {
            if (currentInputSource == null)
            {
                base.OnFocusEnter();
            }
        }

        /// <summary>
        /// Hand position
        /// </summary>
        /// <returns></returns>
        private Vector3 GetCurrentHandPosition()
        {
            Vector3 handPosition;
            // TEMP currentInputSource.TryGetGripPosition(currentInputSourceId, out handPosition);

            return Vector3.zero; // TEMP handPosition;
        }

        /// <summary>
        /// Hide the cursor during the gesture
        /// </summary>
        /// <param name="state"></param>
        private void HandleCursor(bool state)
        {
            // Hack for now.
            // TODO: Update Cursor Modifier to handle HideOnGesture, then calculate visibility so cursors can handle this correctly
            if (state)
            {
                // TEMP cursor = FindObjectOfType<Cursor>();
            }

            if (HideCursorOnManipulation && cursor != null)
            {
                // TEMP cursor.SetVisibility(!state);
            }
        }

        /// <summary>
        /// Update gestures and send gesture data to GestureInteractiveController
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (currentInputSource != null)
            {
                currentHandPosition = GetCurrentHandPosition();
                GestureState = GestureManipulationState.Update;

                if (Control != null)
                {
                    Control.ManipulationUpdate(startHandPosition, currentHandPosition, startHeadPosition, startHeadRay, GestureManipulationState.Update);
                }
            }
        }

#if UNITY_WSA || UNITY_STANDALONE_WIN
        /// <summary>
        /// From Interactive, but customized for triggering gestures from keywords
        /// Handle the manipulation in the GestureInteractiveControl
        /// </summary>
        /// <param name="args"></param>
        protected override void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            base.KeywordRecognizer_OnPhraseRecognized(args);

            // Check to make sure the recognized keyword matches, then invoke the corresponding method.
            if ((!KeywordRequiresGaze || HasGaze) && mKeywordDictionary != null)
            {
                int index;
                if (mKeywordDictionary.TryGetValue(args.text, out index) && Control != null)
                {
                    Control.SetGestureValue(index / mKeywordDictionary.Count - 1);
                }
            }
        }
#endif
        protected override void HandleTaps()
        {
            float distance = (currentHandPosition - startHandPosition).magnitude;
            if (tapDistanceThreshold > distance && HasGaze)
            {
                base.HandleTaps();
                if (Control != null)
                {
                    Control.Tap();
                }
            }
        }

        /// <summary>
        /// Clean up
        /// </summary>
        protected override void OnDestroy()
        {
            if (ticker != null)
            {
                StopCoroutine(ticker);
                ticker = null;
            }

            base.OnDestroy();
        }
    }
}
