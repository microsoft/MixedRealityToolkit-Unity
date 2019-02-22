// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Receivers
{
    /// <summary>
    /// An interaction receiver is simply a component that attached to a list of interactable objects and does something
    /// based on events from those interactable objects. This is the base abstract class to extend from.
    /// </summary>
    public abstract class InteractionReceiver : BaseInputHandler,
        IMixedRealityFocusChangedHandler,
        IMixedRealityInputHandler,
        IMixedRealityInputHandler<float>,
        IMixedRealityInputHandler<Vector2>,
        IMixedRealityGestureHandler<Vector2>,
        IMixedRealityGestureHandler<Vector3>,
        IMixedRealityGestureHandler<Quaternion>
    {
        #region Public Members

        [SerializeField]
        [Tooltip("Target interactable Object to receive events for")]
        private List<GameObject> interactables = new List<GameObject>();

        /// <summary>
        /// List of linked interactable objects to receive events for
        /// </summary>
        public List<GameObject> Interactables
        {
            get { return interactables; }
            private set { value = interactables; }
        }

        [Tooltip("Targets for the receiver to affect")]
        private List<GameObject> targets = new List<GameObject>();

        /// <summary>
        /// List of linked targets that the receiver affects
        /// </summary>
        public List<GameObject> Targets
        {
            get { return targets; }
            private set { value = targets; }
        }

        #endregion Public Members

        [SerializeField]
        [Tooltip("When true, this interaction receiver will draw connections in the editor to Interactables and Targets")]
        private bool drawEditorConnections = true;

        #region MonoBehaviour Implementation

        /// <summary>
        /// On enable, set the BaseInputHandler's IsFocusRequired to false to receive all events.
        /// </summary>
        protected override void OnEnable()
        {
            IsFocusRequired = false;
            base.OnEnable();
        }

        /// <summary>
        /// When selected draw lines to all linked interactables
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if (drawEditorConnections)
            {
                if (interactables.Count > 0)
                {
                    GameObject[] interactableList = interactables.ToArray();

                    for (int i = 0; i < interactableList.Length; i++)
                    {
                        if (interactableList[i] != null)
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawLine(transform.position, interactableList[i].transform.position);
                        }
                    }
                }

                if (Targets.Count > 0)
                {
                    GameObject[] targetList = Targets.ToArray();

                    for (int i = 0; i < targetList.Length; i++)
                    {
                        if (targetList[i] != null)
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawLine(transform.position, targetList[i].transform.position);
                        }
                    }
                }
            }
        }

        #endregion MonoBehaviour Implementation

        /// <summary>
        /// Register an interactable with this receiver.
        /// </summary>
        /// <param name="interactable">takes a GameObject as the interactable to register.</param>
        public virtual void RegisterInteractable(GameObject interactable)
        {
            if (interactable == null || interactables.Contains(interactable))
            {
                return;
            }

            interactables.Add(interactable);
        }

        /// <summary>
        /// Function to remove an interactable from the linked list.
        /// </summary>
        /// <param name="interactable"></param>
        public virtual void RemoveInteractable(GameObject interactable)
        {
            if (interactable != null && interactables.Contains(interactable))
            {
                interactables.Remove(interactable);
            }
        }

        /// <summary>
        /// Clear the interactables list and unregister them
        /// </summary>
        public virtual void ClearInteractables()
        {
            GameObject[] _intList = interactables.ToArray();

            for (int i = 0; i < _intList.Length; i++)
            {
                RemoveInteractable(_intList[i]);
            }
        }

        /// <summary>
        /// Is the game object interactable in our list of interactables
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
        protected bool IsInteractable(GameObject interactable)
        {
            return (interactables != null && interactables.Contains(interactable));
        }

        #region IMixedRealityFocusChangedHandler Implementation

        /// <inheritdoc />
        void IMixedRealityFocusChangedHandler.OnBeforeFocusChange(FocusEventData eventData) { /*Unused*/ }

        /// <inheritdoc />
        void IMixedRealityFocusChangedHandler.OnFocusChanged(FocusEventData eventData)
        {
            if (eventData.NewFocusedObject != null && IsInteractable(eventData.NewFocusedObject))
            {
                FocusEnter(eventData.NewFocusedObject, eventData);
            }

            if (eventData.OldFocusedObject != null && IsInteractable(eventData.OldFocusedObject))
            {
                FocusExit(eventData.OldFocusedObject, eventData);
            }
        }

        #endregion IMixedRealityFocusChangedHandler Implementation

        #region IMixedRealityInputHandler Implementation

        /// <inheritdoc />
        void IMixedRealityInputHandler.OnInputUp(InputEventData eventData)
        {
            if (IsInteractable(eventData.selectedObject))
            {
                InputUp(eventData.selectedObject, eventData);
            }
        }

        /// <inheritdoc />
        void IMixedRealityInputHandler.OnInputDown(InputEventData eventData)
        {
            if (IsInteractable(eventData.selectedObject))
            {
                InputDown(eventData.selectedObject, eventData);
            }
        }

        [Obsolete("Use IMixedRealityInputHandler<float>.OnInputChanged instead.")]
        void IMixedRealityInputHandler.OnInputPressed(InputEventData<float> eventData)
        {
            Debug.LogWarning("Obsolete. Use IMixedRealityInputHandler<float>.OnInputChanged instead.");
        }

        /// <inheritdoc />
        void IMixedRealityInputHandler<float>.OnInputChanged(InputEventData<float> eventData)
        {
            if (IsInteractable(eventData.selectedObject))
            {
                InputPressed(eventData.selectedObject, eventData);
            }
        }

        /// <inheritdoc />
        [Obsolete("Use IMixedRealityInputHandler<Vector2>.OnInputChanged instead.")]
        void IMixedRealityInputHandler.OnPositionInputChanged(InputEventData<Vector2> eventData)
        {
            Debug.LogWarning("Obsolete. Use IMixedRealityInputHandler<Vector2>.OnInputChanged instead.");
        }

        /// <inheritdoc />
        void IMixedRealityInputHandler<Vector2>.OnInputChanged(InputEventData<Vector2> eventData)
        {
            if (IsInteractable(eventData.selectedObject))
            {
                PositionInputChanged(eventData.selectedObject, eventData);
            }
        }

        #endregion IMixedRealityInputHandler Implementation

        #region IMixedRealityGestureHandler Implementation

        /// <inheritdoc />
        void IMixedRealityGestureHandler.OnGestureStarted(InputEventData eventData)
        {
            if (IsInteractable(eventData.selectedObject))
            {
                GestureStarted(eventData.selectedObject, eventData);
            }
        }

        /// <inheritdoc />
        void IMixedRealityGestureHandler.OnGestureUpdated(InputEventData eventData)
        {
            if (IsInteractable(eventData.selectedObject))
            {
                GestureUpdated(eventData.selectedObject, eventData);
            }
        }

        /// <inheritdoc />
        void IMixedRealityGestureHandler<Vector2>.OnGestureUpdated(InputEventData<Vector2> eventData)
        {
            if (IsInteractable(eventData.selectedObject))
            {
                GestureUpdated(eventData.selectedObject, eventData);
            }
        }

        /// <inheritdoc />
        void IMixedRealityGestureHandler<Vector3>.OnGestureUpdated(InputEventData<Vector3> eventData)
        {
            if (IsInteractable(eventData.selectedObject))
            {
                GestureUpdated(eventData.selectedObject, eventData);
            }
        }

        /// <inheritdoc />
        void IMixedRealityGestureHandler<Quaternion>.OnGestureUpdated(InputEventData<Quaternion> eventData)
        {
            if (IsInteractable(eventData.selectedObject))
            {
                GestureUpdated(eventData.selectedObject, eventData);
            }
        }

        /// <inheritdoc />
        void IMixedRealityGestureHandler.OnGestureCompleted(InputEventData eventData)
        {
            if (IsInteractable(eventData.selectedObject))
            {
                GestureCompleted(eventData.selectedObject, eventData);
            }
        }

        /// <inheritdoc />
        void IMixedRealityGestureHandler<Vector2>.OnGestureCompleted(InputEventData<Vector2> eventData)
        {
            if (IsInteractable(eventData.selectedObject))
            {
                GestureCompleted(eventData.selectedObject, eventData);
            }
        }

        /// <inheritdoc />
        void IMixedRealityGestureHandler<Vector3>.OnGestureCompleted(InputEventData<Vector3> eventData)
        {
            if (IsInteractable(eventData.selectedObject))
            {
                GestureCompleted(eventData.selectedObject, eventData);
            }
        }

        /// <inheritdoc />
        void IMixedRealityGestureHandler<Quaternion>.OnGestureCompleted(InputEventData<Quaternion> eventData)
        {
            if (IsInteractable(eventData.selectedObject))
            {
                GestureCompleted(eventData.selectedObject, eventData);
            }
        }

        /// <inheritdoc />
        void IMixedRealityGestureHandler.OnGestureCanceled(InputEventData eventData)
        {
            if (IsInteractable(eventData.selectedObject))
            {
                GestureCanceled(eventData.selectedObject, eventData);
            }
        }

        #endregion IMixedRealityGestureHandler Implementation

        #region Protected Virtual Callback Functions

        /// <summary>
        /// Raised when the target interactable object is focused.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void FocusEnter(GameObject targetObject, FocusEventData eventData) { }

        /// <summary>
        /// Raised when the target interactable object has lost focus.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void FocusExit(GameObject targetObject, FocusEventData eventData) { }

        /// <summary>
        /// Raised when the target interactable object receives an input down event.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void InputDown(GameObject targetObject, InputEventData eventData) { }

        /// <summary>
        /// Raised when the target interactable object receives an input up event.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void InputUp(GameObject targetObject, InputEventData eventData) { }

        /// <summary>
        /// Raised when the target interactable object receives an input pressed event.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void InputPressed(GameObject targetObject, InputEventData<float> eventData) { }

        /// <summary>
        /// Raised when the target interactable object receives an input changed event.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void PositionInputChanged(GameObject targetObject, InputEventData<Vector2> eventData) { }

        /// <summary>
        /// Raised when the target interactable object receives an input changed event.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void PositionInputChanged(GameObject targetObject, InputEventData<Vector3> eventData) { }

        /// <summary>
        /// Raised when the target interactable object receives an input changed event.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void PositionInputChanged(GameObject targetObject, InputEventData<Quaternion> eventData) { }

        /// <summary>
        /// Raised when the target interactable object receives an input changed event.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void PositionInputChanged(GameObject targetObject, InputEventData<MixedRealityPose> eventData) { }

        /// <summary>
        /// Raised when the target interactable object receives an gesture started event.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void GestureStarted(GameObject targetObject, InputEventData eventData) { }

        /// <summary>
        /// Raised when the target interactable object receives an gesture updated event.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void GestureUpdated(GameObject targetObject, InputEventData eventData) { }

        /// <summary>
        /// Raised when the target interactable object receives an gesture updated event.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void GestureUpdated(GameObject targetObject, InputEventData<Vector2> eventData) { }

        /// <summary>
        /// Raised when the target interactable object receives an gesture updated event.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void GestureUpdated(GameObject targetObject, InputEventData<Vector3> eventData) { }

        /// <summary>
        /// Raised when the target interactable object receives an gesture updated event.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void GestureUpdated(GameObject targetObject, InputEventData<Quaternion> eventData) { }

        /// <summary>
        /// Raised when the target interactable object receives an gesture completed event.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void GestureCompleted(GameObject targetObject, InputEventData eventData) { }

        /// <summary>
        /// Raised when the target interactable object receives an gesture completed event.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void GestureCompleted(GameObject targetObject, InputEventData<Vector2> eventData) { }

        /// <summary>
        /// Raised when the target interactable object receives an gesture completed event.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void GestureCompleted(GameObject targetObject, InputEventData<Vector3> eventData) { }

        /// <summary>
        /// Raised when the target interactable object receives an gesture completed event.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void GestureCompleted(GameObject targetObject, InputEventData<Quaternion> eventData) { }

        /// <summary>
        /// Raised when the target interactable object receives an gesture canceled event.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="eventData"></param>
        protected virtual void GestureCanceled(GameObject targetObject, InputEventData eventData) { }

        #endregion Protected Virtual Callback Functions
    }
}