// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Object that represents a cursor in 3D space controlled by gaze.
    /// </summary>
    public class Cursor : MonoBehaviour, IInputHandler, ISourceStateHandler
    {
        public float MinCursorDistance = 1.0f;
        public float DefaultCursorDistance = 2.0f;
        public Interpolator Interpolator;
        public string DefaultCursorStateTrigger;

        /// <summary>
        /// Visual that is displayed when cursor is active normally
        /// </summary>
        public Transform PrimaryCursorVisual;

        /// <summary>
        /// Indicates if hand is current in the view
        /// </summary>
        protected bool IsHandVisible;

        /// <summary>
        /// Indicates air tap down
        /// </summary>
        protected bool IsInputSourceDown;

        protected GameObject TargetedObject;
        protected CursorModifier TargetedCursorModifier;
        private bool isRegisteredToGazeManager = false;
        private bool isInputRegistered = false;

        [SerializeField]
        protected Animator CursorAnimation = null;

        private Vector3 lastCursorForward = Vector3.zero;
        private const float LastCursorForwardInterpolationSpeed = 10.0f;

        private bool skipInterpolation;
        private uint visibleHandsCount = 0;
        private bool isVisible = true;
        private GazeManager gazeManager;

        /// <summary>
        /// Indicates if the cursor should be visible
        /// </summary>
        public bool IsVisible
        {
            set
            {
                isVisible = value;
                UpdateVisualState();
            }
        }

        private void Awake()
        {
            if (Interpolator == null)
            {
                Interpolator = GetComponent<Interpolator>();
            }

            // Use the setter to update visibility of the cursor at startup based on user preferences
            IsVisible = isVisible;
        }

        private void Start()
        {
            gazeManager = GazeManager.Instance;
            RegisterGazeManager();
            RegisterInput();
        }

        private void Update()
        {
            UpdateCursorTransform();
            UpdateCursorAnimation();
        }

        protected virtual void OnEnable()
        {
            RegisterGazeManager();
            RegisterInput();
            skipInterpolation = true;
            UpdateVisualState();
        }

        private void OnDisable()
        {
            UnregisterInput();
            UnregisterGazeManager();
            TargetedObject = null;
            TargetedCursorModifier = null;
            visibleHandsCount = 0;
        }

        private void OnDestroy()
        {
            UnregisterInput();
            UnregisterGazeManager();
        }

        /// <summary>
        /// Register to events from the gaze manager, if not already registered.
        /// </summary>
        private void RegisterGazeManager()
        {
            if (!isRegisteredToGazeManager && gazeManager != null)
            {
                gazeManager.FocusedObjectChanged += OnFocusedObjectChanged;
                isRegisteredToGazeManager = true;
            }
        }

        /// <summary>
        /// Unregister from events from the gaze manager.
        /// </summary>
        private void UnregisterGazeManager()
        {
            if (isRegisteredToGazeManager && gazeManager != null)
            {
                gazeManager.FocusedObjectChanged -= OnFocusedObjectChanged;
                isRegisteredToGazeManager = false;
            }
        }

        /// <summary>
        /// Register to input events that can impact cursor state.
        /// </summary>
        private void RegisterInput()
        {
            if (isInputRegistered)
            {
                return;
            }

            if (InputManager.Instance == null)
            {
                return;
            }

            // Register the cursor as a global listener, so that it can always get input events it cares about
            InputManager.Instance.AddGlobalListener(gameObject);
            isInputRegistered = true;
        }

        /// <summary>
        /// Unregister from input events.
        /// </summary>
        private void UnregisterInput()
        {
            if (!isInputRegistered)
            {
                return;
            }

            if (InputManager.Instance != null)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
                isInputRegistered = false;
            }
        }

        /// <summary>
        /// Updates the currently targeted object and cursor modifier upon getting
        /// an event indicating that the focused object has changed.
        /// </summary>
        /// <param name="previousObject">Object that was previously being focused.</param>
        /// <param name="newObject">New object being focused.</param>
        private void OnFocusedObjectChanged(GameObject previousObject, GameObject newObject)
        {
            TargetedObject = newObject;
            if (newObject != null)
            {
                TargetedCursorModifier = newObject.GetComponent<CursorModifier>();

                // Trigger the cursor state change coming from a change of focus
                if (TargetedCursorModifier == null)
                {
                    TriggerCursorState(DefaultCursorStateTrigger);
                }
                else
                {
                    TriggerCursorState(TargetedCursorModifier.CursorTriggerName);
                }
            }
        }

        /// <summary>
        /// Disables input, putting the cursor in a wait state.
        /// </summary>
        public void DisableInput()
        {
            if (CursorAnimation != null)
            {
                CursorAnimation.SetTrigger("StartWaiting");
            }

            TargetedCursorModifier = null;
            UpdateVisualState();
        }

        /// <summary>
        /// Enables input, putting the cursor back into its regular state.
        /// </summary>
        public void EnableInput()
        {
            if (CursorAnimation != null)
            {
                CursorAnimation.SetTrigger("DoneWaiting");
            }
        }

        /// <summary>
        /// Triggers a cursor state change by using its underlying mecanim.
        /// </summary>
        /// <param name="animationTrigger">Animation trigger to use to trigger a cursor change.</param>
        public void TriggerCursorState(string animationTrigger)
        {
            if (CursorAnimation != null && !string.IsNullOrEmpty(animationTrigger))
            {
                CursorAnimation.SetTrigger(animationTrigger);
            }
        }

        /// <summary>
        /// Update the cursor's transform
        /// </summary>
        private void UpdateCursorTransform()
        {
            // Get the necessary info from the gaze source
            RaycastHit hitResult = gazeManager.HitInfo;
            GameObject newTargetedObject = gazeManager.HitObject;
            Vector3 cursorSourcePosition = gazeManager.GazeOrigin;
            Vector3 cursorSourceForward = gazeManager.GazeNormal;

            Vector3 targetPosition;
            Vector3 targetForward;
            Vector3 cursorScaleOffset = Vector3.one;

            // If no game object is hit, put the cursor at the default distance
            if (TargetedObject == null)
            {
                this.TargetedObject = null;
                targetPosition = cursorSourcePosition + cursorSourceForward * DefaultCursorDistance;
                targetForward = -cursorSourceForward;
            }
            else
            {
                // Update currently targeted object
                this.TargetedObject = newTargetedObject;

                if (TargetedCursorModifier != null)
                {
                    Transform targetTransform = TargetedCursorModifier.HostTransform;

                    // Set the cursor position
                    if (TargetedCursorModifier.SnapCursor)
                    {
                        // Snap if the targeted object has a cursor modifier that supports snapping
                        targetPosition = targetTransform.position +
                                         targetTransform.TransformVector(TargetedCursorModifier.CursorOffset);
                    }
                    // Else, consider the modifiers on the cursor modifier, but don't snap
                    else
                    {
                        targetPosition = hitResult.point + targetTransform.TransformVector(TargetedCursorModifier.CursorOffset);
                    }

                    // Set the cursor forward
                    if (TargetedCursorModifier.UseGazeBasedNormal)
                    {
                        targetForward = -cursorSourceForward;
                    }
                    else
                    {
                        targetForward = targetTransform.rotation * TargetedCursorModifier.CursorNormal;
                    }

                    // Set cursor scale
                    cursorScaleOffset = TargetedCursorModifier.CursorScaleOffset;
                }
                else
                {
                    // If no modifier is on the target, just use the hit result to set cursor position
                    targetPosition = hitResult.point;
                    targetForward = hitResult.normal;
                }
            }

            // Further blend the cursor normal so it doesn't cause any jarring pops
            lastCursorForward = Vector3.Lerp(lastCursorForward, targetForward,
                Time.deltaTime * LastCursorForwardInterpolationSpeed);
            Quaternion targetRotation = GetCursorRotation(lastCursorForward);

            if (Interpolator != null)
            {
                Interpolator.SetTargetPosition(targetPosition);
                Interpolator.SetTargetLocalScale(cursorScaleOffset);


                Interpolator.SetTargetRotation(targetRotation);
            }
            else
            {
                transform.position = targetPosition;
                transform.rotation = targetRotation;
                transform.localScale = cursorScaleOffset;
            }

            UpdateVisualState();

            if (skipInterpolation && Interpolator != null)
            {
                Interpolator.SnapToTarget();
                skipInterpolation = false;
            }
        }

        private Quaternion GetCursorRotation(Vector3 forward)
        {
            Quaternion existingRotation = transform.rotation;
            Quaternion deltaRotation = Quaternion.FromToRotation(existingRotation * Vector3.forward, forward);

            return deltaRotation * existingRotation;
        }

        /// <summary>
        /// Updates the visual representation of the cursor.
        /// </summary>
        private void UpdateVisualState()
        {
            bool hasCursorModifier = TargetedCursorModifier != null;
            bool cursorVisible = isVisible && (!hasCursorModifier || !TargetedCursorModifier.HideCursorOnFocus);

            if (PrimaryCursorVisual != null)
            {
                PrimaryCursorVisual.gameObject.SetActive(cursorVisible);
            }

            UpdateCursorAnimation();
        }

        /// <summary>
        /// Updates the cursor animation state.
        /// </summary>
        protected virtual void UpdateCursorAnimation()
        {
            if (CursorAnimation == null)
            {
                return;
            }

            CursorAnimation.SetBool("IsRing", IsHandVisible);
        }

        public void OnInputUp(InputEventData eventData)
        {
            if (IsInputSourceDown == false)
            {
                return;
            }
            IsInputSourceDown = false;
            if (CursorAnimation != null)
            {
                CursorAnimation.SetTrigger("AirTapUp");
            }
            UpdateVisualState();
        }

        public void OnInputDown(InputEventData eventData)
        {
            IsInputSourceDown = true;
            if (CursorAnimation != null)
            {
                CursorAnimation.SetTrigger("AirTapDown");
            }
            UpdateVisualState();
        }

        public void OnInputClicked(InputEventData eventData)
        {
            // Nothing to do
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            visibleHandsCount++;
            IsHandVisible = true;
            UpdateVisualState();
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            visibleHandsCount--;
            if (visibleHandsCount == 0)
            {
                IsHandVisible = false;
            }
            UpdateVisualState();
        }
    }
}