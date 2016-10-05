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
        /// Occurs when a manipulation gesture has started.
        /// </summary>
        /// <param name="sourceKind">The Interaction Source Kind that started the event.</param>
        public delegate void ManipulationStartedDelegate(InteractionSourceKind sourceKind);
        public event ManipulationStartedDelegate OnManipulationStarted;

        /// <summary>
        /// Occurs when a manipulation gesture ended as a result of user input.
        /// </summary>
        /// <param name="sourceKind">The Interaction Source Kind that completed the event.</param>
        public delegate void ManipulationCompletedDelegate(InteractionSourceKind sourceKind);
        public event ManipulationCompletedDelegate OnManipulationCompleted;

        /// <summary>
        /// Occurs when a manipulated gesture ended as a result of some other condition.
        /// (e.g. the hand being used for the gesture is no longer visible).
        /// </summary>
        /// <param name="sourceKind">The Interaction Source Kind that cancelled the event.</param>
        public delegate void ManipulationCanceledDelegate(InteractionSourceKind sourceKind);
        public event ManipulationCanceledDelegate OnManipulationCanceled;

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
                Vector3 handPosition;
                if (!currentHandState.properties.location.TryGetPosition(out handPosition))
                {
                    handPosition = Vector3.zero;
                }
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

        private void Start()
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

        /// <summary>
        /// Thrown when the Interaction Source is pressed.
        /// </summary>
        /// <param name="state">The current state of the Interaction source.</param>
        private void InteractionManager_SourcePressed(InteractionSourceState state)
        {
            if (state.source.kind == InteractionSourceKind.Hand)
            {
                if (!HandPressed)
                {
                    currentHandState = state;
                }

                pressedHands.Add(state.source.id);
            }
        }

        /// <summary>
        /// Thrown when the Interaction Source is updated.
        /// </summary>
        /// <param name="state">The current state of the Interaction source.</param>
        private void InteractionManager_SourceUpdated(InteractionSourceState state)
        {
            if (state.source.kind == InteractionSourceKind.Hand)
            {
                if (HandPressed && state.source.id == currentHandState.source.id)
                {
                    currentHandState = state;
                }
            }
        }

        /// <summary>
        /// Thrown when the Interaction Source is released.
        /// </summary>
        /// <param name="state">The current state of the Interaction source.</param>
        private void InteractionManager_SourceReleased(InteractionSourceState state)
        {
            if (state.source.kind == InteractionSourceKind.Hand)
            {
                pressedHands.Remove(state.source.id);
            }
        }

        /// <summary>
        /// Thrown when the Interaction Source is no longer availible.
        /// </summary>
        /// <param name="state">The current state of the Interaction source.</param>
        private void InteractionManager_SourceLost(InteractionSourceState state)
        {
            if (state.source.kind == InteractionSourceKind.Hand)
            {
                pressedHands.Remove(state.source.id);
            }
        }

        /// <summary>
        /// Throws <see cref="OnTap"/>.
        /// </summary>
        /// <param name="source">Interaction Source.</param>
        /// <param name="tapCount">TODO: Need clarification on what this is </param>
        /// <param name="headRay">The Ray from the users forward direction.</param>
        private void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            OnTap();
        }

        /// <summary>
        /// Throws <see cref="OnRecognitionStarted"/>. Only used for UI states.
        /// </summary>
        /// <param name="source">Input Source.</param>
        /// <param name="headRay">The Ray from the users forward direction.</param>
        private void GestureRecognizer_RecognitionStartedEvent(InteractionSourceKind source, Ray headRay)
        {
            OnRecognitionStarted();
        }

        /// <summary>
        /// Throws <see cref="OnRecognitionEndeded"/>. Only used for UI states.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="headRay"></param>
        private void GestureRecogniser_RecognitionEndedEvent(InteractionSourceKind source, Ray headRay)
        {
            OnRecognitionEndeded();
        }

        /// <summary>
        /// Throws OnSelect.
        /// </summary>
        private void OnTap()
        {
            if (FocusedObject != null)
            {
                FocusedObject.SendMessage("OnSelect", SendMessageOptions.DontRequireReceiver);
            }
        }

        /// <summary>
        /// Throws OnPressed.  Only used for determining UI states.
        /// </summary>
        private void OnRecognitionStarted()
        {
            if (FocusedObject != null)
            {
                hasRecognitionStarted = true;
                FocusedObject.SendMessage("OnPressed", SendMessageOptions.DontRequireReceiver);
            }
        }

        /// <summary>
        /// Throws OnReleased. Only used for determining UI states.
        /// </summary>
        private void OnRecognitionEndeded()
        {
            if (FocusedObject != null && hasRecognitionStarted)
            {
                FocusedObject.SendMessage("OnReleased", SendMessageOptions.DontRequireReceiver);
            }

            hasRecognitionStarted = false;
        }

        /// <summary>
        /// Thrown when the gesture manager recognizes that a manipulation has begun.
        /// </summary>
        /// <param name="source">Input Source Kind.</param>
        /// <param name="cumulativeDelta">Cumlulative Data.</param>
        /// <param name="headRay">The Ray from the users forward direction.</param>
        private void ManipulationRecognizer_ManipulationStartedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            // Don't start another manipulation gesture if one is already underway
            if (!ManipulationInProgress)
            {
                OnManipulation(inProgress: true, offset: cumulativeDelta);
                if (OnManipulationStarted != null)
                {
                    OnManipulationStarted(source);
                }
            }
        }

        /// <summary>
        /// Thrown when the gesture manager recognizes that a manipulation has been updated.
        /// </summary>
        /// <param name="source">Input Source Kind.</param>
        /// <param name="cumulativeDelta">Cumlulative Data.</param>
        /// <param name="headRay">The Ray from the users forward direction.</param>
        private void ManipulationRecognizer_ManipulationUpdatedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            OnManipulation(inProgress: true, offset: cumulativeDelta);
        }

        /// <summary>
        /// Thrown when the gesture manager recognizes that a manipulation has completed.
        /// </summary>
        /// <param name="source">Input Source Kind.</param>
        /// <param name="cumulativeDelta">Cumlulative Data.</param>
        /// <param name="headRay">The Ray from the users forward direction.</param>
        private void ManipulationRecognizer_ManipulationCompletedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            OnManipulation(inProgress: false, offset: cumulativeDelta);
            if (OnManipulationCompleted != null)
            {
                OnManipulationCompleted(source);
            }
        }

        /// <summary>
        /// Thrown when the gesture manager recognizes that a manipulation has been canceled.
        /// </summary>
        /// <param name="source">Input Source Kind.</param>
        /// <param name="cumulativeDelta">Cumlulative Data.</param>
        /// <param name="headRay">The Ray from the users forward direction.</param>
        private void ManipulationRecognizer_ManipulationCanceledEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            OnManipulation(inProgress: false, offset: cumulativeDelta);
            if (OnManipulationCanceled != null)
            {
                OnManipulationCanceled(source);
            }
        }

        /// <summary>
        /// Processes Manipulation Data.
        /// </summary>
        /// <param name="inProgress">Is this manipulation in progress?</param>
        /// <param name="offset">The Offset of our manipulation to calulate delta positions.</param>
        private void OnManipulation(bool inProgress, Vector3 offset)
        {
            ManipulationInProgress = inProgress;
            ManipulationOffset = offset;
        }

        private void LateUpdate()
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

            // Checks to see if our focus has changed.
            bool focusedChanged = FocusedObject != newFocusedObject;

            if (focusedChanged)
            {
                // If the currently focused object doesn't match the new focused object, cancel the current gesture.
                // This is to prevent applying gestures from one hologram to another.
                gestureRecognizer.CancelGestures();

                // Set our current Focused Object.
                FocusedObject = newFocusedObject;

                // Start looking for new gestures.
                gestureRecognizer.StartCapturingGestures();
            }

#if UNITY_EDITOR || UNITY_STANDALONE
            // Process Editor/Companion app input.

            // If we're already pressing a button/key or our focus has changed then throw recognition Ended.
            if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(EditorSelectKey) || focusedChanged)
            {
                OnRecognitionEndeded();
            }

            // If we're currently pressing a button/key.
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(EditorSelectKey))
            {
                // If our focus has changed since the last frame then throw a new Tap and start recognition.
                if (focusedChanged)
                {
                    OnTap();
                    OnRecognitionStarted();
                }
            }
#endif
        }

        private void OnDestroy()
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