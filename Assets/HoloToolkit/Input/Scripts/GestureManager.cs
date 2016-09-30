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
    /// Using the Gesture Manager requires a GameObject component to subscribe to the OnTap event.
    /// When a tap gesture is detected, GestureManager uses GazeManager to find the game object.
    /// See TODO: "Link to HTK documentation" for examples of usage.
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
        public delegate void OnManipulationStarted(InteractionSourceKind sourceKind);
        public event OnManipulationStarted ManipulationStarted;

        /// <summary>
        /// Occurs when a manipulation gesture ended as a result of user input.
        /// </summary>
        /// <param name="sourceKind">The Interaction Source Kind that completed the event.</param>
        public delegate void OnManipulationCompleted(InteractionSourceKind sourceKind);
        public event OnManipulationCompleted ManipulationCompleted;

        /// <summary>
        /// Occurs when a manipulated gesture ended as a result of some other condition.
        /// (e.g. the hand being used for the gesture is no longer visible).
        /// </summary>
        /// <param name="sourceKind">The Interaction Source Kind that cancelled the event.</param>
        public delegate void OnManipulationCanceled(InteractionSourceKind sourceKind);
        public event OnManipulationCanceled ManipulationCanceled;

        /// <summary>
        /// Occurs when a user calls the TappedEvent from the gesture recognizer.
        /// </summary>
        /// <param name="tappedObject">Selected GameObject User has tapped.</param>
        public delegate void OnTapEvent(GameObject tappedObject);
        public event OnTapEvent OnTap;

        /// <summary>
        /// Occurs when a Focused Object is pressed.
        /// </summary>
        /// <param name="sourceKind">The Interaction Source Kind that started the event.</param>
        public delegate void OnPressDelegate(InteractionSourceKind sourceKind);
        public event OnPressDelegate OnPressed;

        /// <summary>
        /// Occurs when a Focused Object that is currently being pressed, is released.
        /// </summary>
        /// <param name="sourceKind">The Interaction Source Kind that completed the event.</param>
        public delegate void OnReseaseDelegate(InteractionSourceKind sourceKind);
        public event OnReseaseDelegate OnRelease;

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

        /// <summary>
        /// We use a separate manipulation recognizer here because the tap gesture recognizer cancels
        /// capturing gestures whenever the GazeManager focus changes, which is not the behavior
        /// we want for manipulation.
        /// </summary>
        private GestureRecognizer manipulationRecognizer;

        private bool hasRecognitionStarted;

        private bool HandPressed { get { return pressedHands.Count > 0; } }

        private HashSet<uint> pressedHands = new HashSet<uint>();

        private InteractionSourceState currentHandState;

        private Dictionary<GameObject, IInteractable> interactableCache = new Dictionary<GameObject, IInteractable>();

        IInteractable focusedInteractable;

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

        private void GestureRecognizer_TappedEvent(InteractionSourceKind sourceKind, int tapCount, Ray headRay)
        {
            ProcessTap();
        }

        private void GestureRecognizer_RecognitionStartedEvent(InteractionSourceKind sourceKind, Ray headRay)
        {
            OnRecognitionStarted(sourceKind);
        }

        private void GestureRecogniser_RecognitionEndedEvent(InteractionSourceKind sourceKind, Ray headRay)
        {
            OnRecognitionEndeded(sourceKind);
        }

        private void ProcessTap()
        {
            if (FocusedObject != null)
            {
                if (focusedInteractable != null)
                {
                    focusedInteractable.OnTap();
                }

                if (OnTap != null)
                {
                    OnTap(FocusedObject);
                }

                // Obsolete!  Please subscribe to OnTap event.
                FocusedObject.SendMessage("OnSelect", SendMessageOptions.DontRequireReceiver);
                SendMessage("OnSelect", SendMessageOptions.DontRequireReceiver);
                // End Obsolete
            }
        }

        private void OnRecognitionStarted(InteractionSourceKind sourceKind)
        {
            if (FocusedObject != null)
            {
                hasRecognitionStarted = true;

                if (OnPressed != null)
                {
                    OnPressed(sourceKind);
                }

                FocusedObject.SendMessage("OnPressed", SendMessageOptions.DontRequireReceiver);
            }
        }

        private void OnRecognitionEndeded(InteractionSourceKind sourceKind)
        {
            if (FocusedObject != null && hasRecognitionStarted)
            {
                if (OnRelease != null)
                {
                    OnRelease(sourceKind);
                }

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
                    ManipulationStarted(source);
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
                ManipulationCompleted(source);
            }
        }

        private void ManipulationRecognizer_ManipulationCanceledEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            OnManipulation(false, cumulativeDelta);
            if (ManipulationCanceled != null)
            {
                ManipulationCanceled(source);
            }
        }

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

            bool focusedChanged = FocusedObject != newFocusedObject;

            if (focusedChanged && FocusedObject != null)
            {
                if (!interactableCache.TryGetValue(FocusedObject, out focusedInteractable))
                {
                    focusedInteractable = FocusedObject.GetComponent<IInteractable>();
                    interactableCache.Add(FocusedObject, focusedInteractable);
                }
            }

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(EditorSelectKey))
            {
                ProcessTap();
                OnRecognitionStarted(InteractionSourceKind.Other);
            }

            if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(EditorSelectKey) || focusedChanged)
            {
                OnRecognitionEndeded(InteractionSourceKind.Other);
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

        public void OnSelect()
        {
            ThrowObsoleteWarning();
        }

        public void OnGazeLeave()
        {
            ThrowObsoleteWarning();
        }

        public void OnGazeEnter()
        {
            ThrowObsoleteWarning();
        }

        private void ThrowObsoleteWarning()
        {
            // For more information see: TODO:Link to HoloToolkit Documentation.
            Debug.LogWarning("Using SendMessage is not recommended in the HoloToolkit and has been replaced by either Interface or delegate events. See Documentation");
        }
    }
}