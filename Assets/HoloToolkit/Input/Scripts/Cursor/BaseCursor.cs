// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Object that represents a cursor in 3D space controlled by gaze.
    /// </summary>
    public abstract class BaseCursor : MonoBehaviour, ICursor
    {
        public CursorStateEnum CursorState { get { return cursorState; } }
        private CursorStateEnum cursorState = CursorStateEnum.None;

        /// <summary>
        /// Minimum distance for cursor if nothing is hit
        /// </summary>
        [Header("Cursor Distance")]
        [Tooltip("The minimum distance the cursor can be with nothing hit")]
        public float MinCursorDistance = 1.0f;

        /// <summary>
        /// Maximum distance for cursor if nothing is hit
        /// </summary>
        [Tooltip("The maximum distance the cursor can be with nothing hit")]
        public float DefaultCursorDistance = 2.0f;

        /// <summary>
        /// Surface distance to place the cursor off of the surface at
        /// </summary>
        [Tooltip("The distance from the hit surface to place the cursor")]
        public float SurfaceCursorDistance = 0.02f;

        [Header("Motion")]
        [Tooltip("When lerping, use unscaled time. This is useful for games that have a pause mechanism or otherwise adjust the game timescale.")]
        public bool UseUnscaledTime = true;

        /// <summary>
        /// Blend value for surface normal to user facing lerp
        /// </summary>
        public float PositionLerpTime = 0.01f;

        /// <summary>
        /// Blend value for surface normal to user facing lerp
        /// </summary>
        public float ScaleLerpTime = 0.01f;

        /// <summary>
        /// Blend value for surface normal to user facing lerp
        /// </summary>
        public float RotationLerpTime = 0.01f;

        /// <summary>
        /// Blend value for surface normal to user facing lerp
        /// </summary>
        [Range(0, 1)]
        public float LookRotationBlend = 0.5f;

        /// <summary>
        /// Visual that is displayed when cursor is active normally
        /// </summary>
        [Header("Transform References")]
        public Transform PrimaryCursorVisual;

        /// <summary>
        /// Indicates if the source is detected.
        /// </summary>
        protected bool IsHandDetected;

        /// <summary>
        /// Indicates pointer or air tap down
        /// </summary>
        protected bool IsPointerDown;

        protected GameObject TargetedObject;

        private uint visibleHandsCount = 0;
        private bool isVisible = true;

        // Position, scale and rotational goals for cursor
        private Vector3 targetPosition;
        private Vector3 targetScale;
        private Quaternion targetRotation;

        #region ICursor Implementation

        /// <summary>
        /// The pointer that this cursor should follow and process input from.
        /// </summary>
        public virtual IPointer Pointer
        {
            get { return pointer; }
            set
            {
                pointer = value;
                pointer.BaseCursor = this;
                RegisterManagers();
            }
        }

        private IPointer pointer;

        public virtual Vector3 Position
        {
            get { return transform.position; }
        }

        public virtual Quaternion Rotation
        {
            get { return transform.rotation; }
        }

        public virtual Vector3 LocalScale
        {
            get { return transform.localScale; }
        }

        /// <summary>
        /// Indicates if the cursor should be visible
        /// </summary>
        public bool IsVisible
        {
            set
            {
                isVisible = value;
                SetVisibility(isVisible);
            }
        }

        #endregion ICursor Implementation

        #region ISourceStateHandler Implementation

        /// <summary>
        /// Input source detected callback for the cursor
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnSourceDetected(SourceStateEventData eventData)
        {
#if UNITY_WSA
            SimulatedInputSource inputSource = null;

            if (Application.isEditor && eventData.InputSource.GetType() == typeof(SimulatedInputSource))
            {
                inputSource = (SimulatedInputSource)eventData.InputSource;
            }

            InteractionSourceKind sourceKind;
            if ((InteractionInputSources.Instance.TryGetSourceKind(eventData.SourceId, out sourceKind) ||
                inputSource != null && inputSource.TryGetSourceKind(out sourceKind))
                && sourceKind == InteractionSourceKind.Hand)
            {
                visibleHandsCount++;
            }
#endif

            if (visibleHandsCount > 0)
            {
                IsHandDetected = true;
            }
        }

        /// <summary>
        /// Input source lost callback for the cursor
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnSourceLost(SourceStateEventData eventData)
        {
#if UNITY_WSA
            SimulatedInputSource inputSource = null;

            if (Application.isEditor && eventData.InputSource.GetType() == typeof(SimulatedInputSource))
            {
                inputSource = (SimulatedInputSource)eventData.InputSource;
            }

            InteractionSourceKind sourceKind;
            if ((InteractionInputSources.Instance.TryGetSourceKind(eventData.SourceId, out sourceKind) ||
                 inputSource != null && inputSource.TryGetSourceKind(out sourceKind))
                && sourceKind == InteractionSourceKind.Hand)
            {
                visibleHandsCount--;
            }
#endif

            if (visibleHandsCount == 0)
            {
                IsHandDetected = false;
                IsPointerDown = false;
            }
        }

        public virtual void OnSourcePositionChanged(SourcePositionEventData eventData) { }

        public virtual void OnSourceRotationChanged(SourceRotationEventData eventData) { }

        #endregion ISourceStateHandler Implementation

        #region IFocusChangedHandler Implementation

        /// <summary>
        /// Updates the currently targeted object and cursor modifier upon getting
        /// an event indicating that the focused object has changed.
        /// </summary>
        public virtual void OnBeforeFocusChange(FocusEventData eventData)
        {
            if (Pointer.PointerId == eventData.Pointer.PointerId)
            {
                TargetedObject = eventData.NewFocusedObject;
            }
        }

        public virtual void OnFocusChanged(FocusEventData eventData) { }

        #endregion IFocusChangedHandler Implementation

        #region IPointerHandler Implementation

        /// <summary>
        /// Function for receiving OnPointerDown events from InputManager
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerDown(ClickEventData eventData)
        {
            foreach (var sourcePointer in eventData.InputSource.Pointers)
            {
                if (sourcePointer.PointerId == Pointer.PointerId)
                {
                    IsPointerDown = true;
                }
            }
        }

        /// <summary>
        /// Function for receiving OnPointerClicked events from InputManager
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerClicked(ClickEventData eventData) { }

        /// <summary>
        /// Function for receiving OnPointerUp events from InputManager
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerUp(ClickEventData eventData)
        {
            foreach (var sourcePointer in eventData.InputSource.Pointers)
            {
                if (sourcePointer.PointerId == Pointer.PointerId)
                {
                    IsPointerDown = false;
                }
            }
        }

        #endregion IPointerHandler Implementation

        #region MonoBehaviour Impementation

        private void Awake()
        {
            // Use the setter to update visibility of the cursor at startup based on user preferences
            IsVisible = isVisible;
            SetVisibility(isVisible);
        }

        private void Update()
        {
            UpdateCursorState();
            UpdateCursorTransform();
        }

        /// <summary>
        /// Override for enable functions
        /// </summary>
        protected virtual void OnEnable()
        {
            OnCursorStateChange(CursorStateEnum.None);
        }

        /// <summary>
        /// Override for disable functions
        /// </summary>
        protected virtual void OnDisable()
        {
            TargetedObject = null;
            visibleHandsCount = 0;
            IsHandDetected = false;
            OnCursorStateChange(CursorStateEnum.Contextual);
        }

        private void OnDestroy()
        {
            UnregisterManagers();
        }

        #endregion MonoBehaviour Impementation

        /// <summary>
        /// Register to events from the managers the cursor needs.
        /// </summary>
        protected virtual void RegisterManagers()
        {
            // Register the cursor as a global listener, so that it can always get input events it cares about
            InputManager.Instance.AddGlobalListener(gameObject);

            // Setup the cursor to be able to respond to input being globally enabled / disabled
            if (InputManager.Instance.IsInputEnabled)
            {
                OnInputEnabled();
            }
            else
            {
                OnInputDisabled();
            }

            InputManager.Instance.InputEnabled += OnInputEnabled;
            InputManager.Instance.InputDisabled += OnInputDisabled;
        }

        /// <summary>
        /// Unregister from events from the managers the cursor needs.
        /// </summary>
        protected virtual void UnregisterManagers()
        {
            if (InputManager.IsInitialized)
            {
                InputManager.Instance.InputEnabled -= OnInputEnabled;
                InputManager.Instance.InputDisabled -= OnInputDisabled;
                InputManager.Instance.RemoveGlobalListener(gameObject);
            }
        }

        /// <summary>
        /// Update the cursor's transform
        /// </summary>
        protected virtual void UpdateCursorTransform()
        {
            Debug.Assert(Pointer != null, "No Pointer has been assigned!");

            FocusDetails focusDetails;
            if (!FocusManager.Instance.TryGetFocusDetails(Pointer, out focusDetails))
            {
                if (FocusManager.Instance.IsPointerRegistered(Pointer))
                {
                    Debug.LogErrorFormat("{0}: Unable to get focus details for {1}!", name, pointer.GetType().Name);
                }
                else
                {
                    Debug.LogErrorFormat("{0} has not been registered!", pointer.GetType().Name);
                }

                return;
            }

            GameObject newTargetedObject = FocusManager.Instance.GetFocusedObject(Pointer);
            Vector3 lookForward;

            // Normalize scale on before update
            targetScale = Vector3.one;

            // If no game object is hit, put the cursor at the default distance
            if (newTargetedObject == null)
            {
                TargetedObject = null;

                targetPosition = RayStep.GetPointByDistance(Pointer.Rays, DefaultCursorDistance);
                lookForward = -RayStep.GetDirectionByDistance(Pointer.Rays, DefaultCursorDistance);
                targetRotation = lookForward.magnitude > 0 ? Quaternion.LookRotation(lookForward, Vector3.up) : transform.rotation;
            }
            else
            {
                // Update currently targeted object
                TargetedObject = newTargetedObject;

                if (Pointer.CursorModifier != null)
                {
                    Pointer.CursorModifier.GetModifiedTransform(this, out targetPosition, out targetRotation, out targetScale);
                }
                else
                {
                    // If no modifier is on the target, just use the hit result to set cursor position
                    // Get the look forward by using distance between pointer origin and target position
                    // (This may not be strictly accurate for extremely wobbly pointers, but it should produce usable results)
                    float distanceToTarget = Vector3.Distance(Pointer.Rays[0].Origin, focusDetails.Point);
                    lookForward = -RayStep.GetDirectionByDistance(Pointer.Rays, distanceToTarget);
                    targetPosition = focusDetails.Point + (lookForward * SurfaceCursorDistance);
                    Vector3 lookRotation = Vector3.Slerp(focusDetails.Normal, lookForward, LookRotationBlend);
                    targetRotation = Quaternion.LookRotation(lookRotation == Vector3.zero ? lookForward : lookRotation, Vector3.up);
                }
            }

            float deltaTime = UseUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            // Use the lerp times to blend the position to the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, deltaTime / PositionLerpTime);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, deltaTime / ScaleLerpTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, deltaTime / RotationLerpTime);
        }

        /// <summary>
        /// Updates the visual representation of the cursor.
        /// </summary>
        public virtual void SetVisibility(bool visible)
        {
            if (PrimaryCursorVisual != null)
            {
                PrimaryCursorVisual.gameObject.SetActive(visible);
            }
        }

        /// <summary>
        /// Disable input and set to contextual to override input
        /// </summary>
        public virtual void OnInputDisabled()
        {
            // Reset visible hands on disable
            visibleHandsCount = 0;
            IsHandDetected = false;

            OnCursorStateChange(CursorStateEnum.Contextual);
        }

        /// <summary>
        /// Enable input and set to none to reset cursor
        /// </summary>
        public virtual void OnInputEnabled()
        {
            OnCursorStateChange(CursorStateEnum.None);
        }

        /// <summary>
        /// Internal update to check for cursor state changes
        /// </summary>
        private void UpdateCursorState()
        {
            CursorStateEnum newState = CheckCursorState();
            if (cursorState != newState)
            {
                OnCursorStateChange(newState);
            }
        }

        /// <summary>
        /// Virtual function for checking state changes.
        /// </summary>
        public virtual CursorStateEnum CheckCursorState()
        {
            if (cursorState != CursorStateEnum.Contextual)
            {
                if (IsPointerDown)
                {
                    return CursorStateEnum.Select;
                }

                if (cursorState == CursorStateEnum.Select)
                {
                    return CursorStateEnum.Release;
                }

                if (IsHandDetected)
                {
                    return TargetedObject != null ? CursorStateEnum.InteractHover : CursorStateEnum.Interact;
                }

                return TargetedObject != null ? CursorStateEnum.ObserveHover : CursorStateEnum.Observe;
            }

            return CursorStateEnum.Contextual;
        }

        /// <summary>
        /// Change the cursor state to the new state.  Override in cursor implementations.
        /// </summary>
        /// <param name="state"></param>
        public virtual void OnCursorStateChange(CursorStateEnum state)
        {
            cursorState = state;
        }
    }
}