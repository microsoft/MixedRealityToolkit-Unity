// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.VR.WSA.Input;
using System.Collections.Generic;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// GestureManager provides access to several different input gestures, including
    /// Tap and Manipulation.
    /// </summary>
    /// <remarks>
    /// When a tap gesture is detected, GestureManager uses GazeManager to find the game object.
    /// GestureManager then sends a message to that game object.
    /// 
    /// Using Manipulation requires subscribing the the ManipulationStarted events and then querying
    /// information about the manipulation gesture via ManipulationOffset and ManipulationHandPosition
    /// </remarks>
    [RequireComponent(typeof(GazeManager))]
    public partial class GestureManager : Singleton<GestureManager>
    {
        /// <summary>
        /// Occurs when a manipulation gesture has started
        /// </summary>
        public System.Action ManipulationStarted;

        /// <summary>
        /// Occurs when a manipulation gesture ended as a result of user input
        /// </summary>
        public System.Action ManipulationCompleted;

        /// <summary>
        /// Occurs when a manipulated gesture ended as a result of some other condition.
        /// (e.g. the hand being used for the gesture is no longer visible).
        /// </summary>
        public System.Action ManipulationCanceled;

        /// <summary>
        /// Key to press in the editor to select the currently gazed hologram
        /// </summary>
        public KeyCode EditorSelectKey = KeyCode.Space;

        /// <summary>
        /// To select even when a hologram is not being gazed at,
        /// set the override focused object.
        /// If its null, then the gazed at object will be selected.
        /// </summary>
        public GameObject OverrideFocusedObject { get; set; }

        /// <summary>
        /// Gets the currently focused object, or null if none.
        /// </summary>
        public GameObject FocusedObject { get; private set; }

        /// <summary>
        /// Whether or not a manipulation gesture is currently in progress
        /// </summary>
        public bool ManipulationInProgress { get; private set; }

        /// <summary>
        /// The offset of the hand from its position at the beginning of 
        /// the currently active manipulation gesture, in world space.  Not valid if
        /// a manipulation gesture is not in progress
        /// </summary>
        public Vector3 ManipulationOffset { get; private set; }

        /// <summary>
        /// The world space position of the hand being used for the current manipulation gesture.  Not valid
        /// if a manipulation gesture is not in progress.
        /// </summary>
        public Vector3 ManipulationHandPosition
        {
            get
            {
                Vector3 handPosition = Vector3.zero;
                currentHandState.properties.location.TryGetPosition(out handPosition);
                return handPosition;
            }
        }

        private GestureRecognizer gestureRecognizer;
        // We use a separate manipulation recognizer here because the tap gesture recognizer cancels
        // capturing gestures whenever the GazeManager focus changes, which is not the behavior
        // we want for manipulation
        private GestureRecognizer manipulationRecognizer;

        private bool hasRecognitionStarted = false;

        private bool HandPressed { get { return pressedHands.Count > 0; } }
        private HashSet<uint> pressedHands = new HashSet<uint>();

        private InteractionSourceState currentHandState;

        void Start()
        {
            InteractionManager.SourcePressed += InteractionManager_SourcePressed;
            InteractionManager.SourceReleased += InteractionManager_SourceReleased;
            InteractionManager.SourceUpdated += InteractionManager_SourceUpdated;
            InteractionManager.SourceLost += InteractionManager_SourceLost;

            // Create a new GestureRecognizer. Sign up for tapped events.
            gestureRecognizer = new GestureRecognizer();
            gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);

            manipulationRecognizer = new GestureRecognizer();
            manipulationRecognizer.SetRecognizableGestures(GestureSettings.ManipulationTranslate);

            gestureRecognizer.TappedEvent += GestureRecognizer_TappedEvent;

            // We need to send pressed and released events to UI so they can provide visual feedback
            // of the current state of the UI based on user input.
            gestureRecognizer.RecognitionStartedEvent += GestureRecognizer_RecognitionStartedEvent;
            gestureRecognizer.RecognitionEndedEvent += GestureRecogniser_RecognitionEndedEvent;

            manipulationRecognizer.ManipulationStartedEvent += ManipulationRecognizer_ManipulationStartedEvent;
            manipulationRecognizer.ManipulationUpdatedEvent += ManipulationRecognizer_ManipulationUpdatedEvent;
            manipulationRecognizer.ManipulationCompletedEvent += ManipulationRecognizer_ManipulationCompletedEvent;
            manipulationRecognizer.ManipulationCanceledEvent += ManipulationRecognizer_ManipulationCanceledEvent;

            // Start looking for gestures.
            gestureRecognizer.StartCapturingGestures();
            manipulationRecognizer.StartCapturingGestures();
        }

        private void InteractionManager_SourcePressed(InteractionSourceState state)
        {
            if (!HandPressed)
            {
                currentHandState = state;
            }

            pressedHands.Add(state.source.id);
        }

        private void InteractionManager_SourceUpdated(InteractionSourceState state)
        {
            if (HandPressed && state.source.id == currentHandState.source.id)
            {
                currentHandState = state;
            }
        }

        private void InteractionManager_SourceReleased(InteractionSourceState state)
        {
            pressedHands.Remove(state.source.id);
        }

        private void InteractionManager_SourceLost(InteractionSourceState state)
        {
            pressedHands.Remove(state.source.id);
        }

        private void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            OnTap();
        }

        private void GestureRecognizer_RecognitionStartedEvent(InteractionSourceKind source, Ray headRay)
        {
            OnRecognitionStarted();
        }

        private void GestureRecogniser_RecognitionEndedEvent(InteractionSourceKind source, Ray headRay)
        {
            OnRecognitionEndeded();
        }

        private void OnTap()
        {
            if (FocusedObject != null)
            {
                FocusedObject.SendMessage("OnSelect", SendMessageOptions.DontRequireReceiver);
            }
        }

        private void OnRecognitionStarted()
        {
            if (FocusedObject != null)
            {
                hasRecognitionStarted = true;
                FocusedObject.SendMessage("OnPressed", SendMessageOptions.DontRequireReceiver);
            }
        }

        private void OnRecognitionEndeded()
        {
            if (FocusedObject != null && hasRecognitionStarted)
            {
                FocusedObject.SendMessage("OnReleased", SendMessageOptions.DontRequireReceiver);
            }

            hasRecognitionStarted = false;
        }

        private void ManipulationRecognizer_ManipulationStartedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            // Don't start another manipulation gesture if one is already underway
            if (!ManipulationInProgress)
            {
                OnManipulation(true, cumulativeDelta);
                if (ManipulationStarted != null)
                {
                    ManipulationStarted();
                }
            }
        }

        private void ManipulationRecognizer_ManipulationUpdatedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            OnManipulation(true, cumulativeDelta);
        }

        private void ManipulationRecognizer_ManipulationCompletedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            OnManipulation(false, cumulativeDelta);
            if (ManipulationCompleted != null)
            {
                ManipulationCompleted();
            }
        }

        private void ManipulationRecognizer_ManipulationCanceledEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            OnManipulation(false, cumulativeDelta);
            if (ManipulationCanceled != null)
            {
                ManipulationCanceled();
            }
        }

        private void OnManipulation(bool inProgress, Vector3 offset)
        {
            ManipulationInProgress = inProgress;
            ManipulationOffset = offset;
        }

        void LateUpdate()
        {
            // set the next focus object to see if focus has changed, but don't replace the current focused object
            // until all the inputs are handled, like Unity Editor input for OnTap().
            GameObject newFocusedObject;

            if (GazeManager.Instance.Hit &&
                OverrideFocusedObject == null &&
                GazeManager.Instance.HitInfo.collider != null)
            {
                // If gaze hits a hologram, set the focused object to that game object.
                // Also if the caller has not decided to override the focused object.
                newFocusedObject = GazeManager.Instance.HitInfo.collider.gameObject;
            }
            else
            {
                // If our gaze doesn't hit a hologram, set the focused object to null or override focused object.
                newFocusedObject = OverrideFocusedObject;
            }

            bool focusedChanged = FocusedObject != newFocusedObject;

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(EditorSelectKey))
            {
                OnTap();
                OnRecognitionStarted();
            }

            if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(EditorSelectKey) || focusedChanged)
            {
                OnRecognitionEndeded();
            }
#endif
            if (focusedChanged)
            {
                // If the currently focused object doesn't match the new focused object, cancel the current gesture.
                // Start looking for new gestures.  This is to prevent applying gestures from one hologram to another.
                gestureRecognizer.CancelGestures();
                FocusedObject = newFocusedObject;
                gestureRecognizer.StartCapturingGestures();
            }
        }

        void OnDestroy()
        {
            gestureRecognizer.StopCapturingGestures();
            gestureRecognizer.TappedEvent -= GestureRecognizer_TappedEvent;
            gestureRecognizer.RecognitionStartedEvent -= GestureRecognizer_RecognitionStartedEvent;
            gestureRecognizer.RecognitionEndedEvent -= GestureRecogniser_RecognitionEndedEvent;

            manipulationRecognizer.StopCapturingGestures();
            manipulationRecognizer.ManipulationStartedEvent -= ManipulationRecognizer_ManipulationStartedEvent;
            manipulationRecognizer.ManipulationUpdatedEvent -= ManipulationRecognizer_ManipulationUpdatedEvent;
            manipulationRecognizer.ManipulationCompletedEvent -= ManipulationRecognizer_ManipulationCompletedEvent;
            manipulationRecognizer.ManipulationCanceledEvent -= ManipulationRecognizer_ManipulationCanceledEvent;

            InteractionManager.SourcePressed -= InteractionManager_SourcePressed;
            InteractionManager.SourceReleased -= InteractionManager_SourceReleased;
            InteractionManager.SourceUpdated -= InteractionManager_SourceUpdated;
            InteractionManager.SourceLost -= InteractionManager_SourceLost;
        }
    }
}