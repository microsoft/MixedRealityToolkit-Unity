// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity.Receivers
{
    /// <summary>
    /// An interaction receiver is simply a component that attached to a list of interactable objects and does something
    /// based on events from those interactable objects.  This is the base abstract class to extend from.
    /// </summary>
    public abstract class InteractionReceiver : MonoBehaviour, IFocusHandler, IInputHandler, IHoldHandler, IPointerHandler, IManipulationHandler, INavigationHandler
    {
        #region Public Members
        /// <summary>
        /// List of linked interactable objects to receive events for
        /// </summary>
        [Tooltip("Target interactable Object to receive events for")]
        public List<GameObject> interactables = new List<GameObject>();

        /// <summary>
        /// List of linked targets that the receiver affects
        /// </summary>
        [Tooltip("Targets for the receiver to ")]
        public List<GameObject> Targets = new List<GameObject>();

        /// <summary>
        /// Flag for locking focus while selected
        /// </summary>
        public bool LockFocus
        {
            get
            {
                return lockFocus;
            }
            set
            {
                lockFocus = value;
                CheckLockFocus(_selectingFocuser);
            }
        }
        #endregion

        #region Private and Protected Members
        [Tooltip("If true, this object will remain the prime focus while select is held")]
        [SerializeField]
        private bool lockFocus = false;

        /// <summary>
        /// Protected focuser for the current selecting focuser
        /// </summary>
        protected IPointingSource _selectingFocuser;
        #endregion

        /// <summary>
        /// On start subscribe to all interaction events on elements in the interactables list.
        /// </summary>
        public virtual void OnEnable()
        {
            InputManager.Instance.AddGlobalListener(gameObject);
        }

        /// <summary>
        /// On disable remove all linked interactables from the delegate functions
        /// </summary>
        public virtual void OnDisable()
        {
            if (InputManager.IsInitialized)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
            }
        }

        /// <summary>
        /// Register an interactable with this receiver.
        /// </summary>
        /// <param name="interactable">takes a GameObject as the interactable to register.</param>
        public virtual void Registerinteractable(GameObject interactable)
        {
            if (interactable == null || interactables.Contains(interactable))
            {
                return;
            }

            interactables.Add(interactable);
        }

#if UNITY_EDITOR
        /// <summary>
        /// When selected draw lines to all linked interactables
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if (this.interactables.Count > 0)
            {
                GameObject[] bioList = this.interactables.ToArray();

                for (int i = 0; i < bioList.Length; i++)
                {
                    if (bioList[i] != null)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawLine(this.transform.position, bioList[i].transform.position);
                    }
                }
            }

            if (this.Targets.Count > 0)
            {
                GameObject[] targetList = this.Targets.ToArray();

                for (int i = 0; i < targetList.Length; i++)
                {
                    if (targetList[i] != null)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(this.transform.position, targetList[i].transform.position);
                    }
                }
            }
        }

#endif

        /// <summary>
        /// Function to remove an interactable from the linked list.
        /// </summary>
        /// <param name="interactable"></param>
        public virtual void Removeinteractable(GameObject interactable)
        {
            if (interactable != null && interactables.Contains(interactable))
            {
                interactables.Remove(interactable);
            }
        }

        /// <summary>
        /// Clear the interactables list and unregister them
        /// </summary>
        public virtual void Clearinteractables()
        {
            GameObject[] _intList = interactables.ToArray();

            for (int i = 0; i < _intList.Length; i++)
            {
                this.Removeinteractable(_intList[i]);
            }
        }

        /// <summary>
        /// Is the game object interactable in our list of interactables
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
        protected bool Isinteractable(GameObject interactable)
        {
            return (interactables != null && interactables.Contains(interactable));
        }

        private void CheckLockFocus(IPointingSource focuser)
        {
            // If our previous selecting focuser isn't the same
            if (_selectingFocuser != null && _selectingFocuser != focuser)
            {
                // If our focus is currently locked, unlock it before moving on
                if (LockFocus)
                {
                    _selectingFocuser.FocusLocked = false;
                }
            }

            // Set to the new focuser
            _selectingFocuser = focuser;
            if (_selectingFocuser != null)
            {
                _selectingFocuser.FocusLocked = LockFocus;
            }
        }

        private void LockFocuser(IPointingSource focuser)
        {
            if (focuser != null)
            {
                ReleaseFocuser();
                _selectingFocuser = focuser;
                _selectingFocuser.FocusLocked = true;
            }
        }

        private void ReleaseFocuser()
        {
            if (_selectingFocuser != null)
            {
                _selectingFocuser.FocusLocked = false;
                _selectingFocuser = null;
            }
        }

        #region Global Listener Callbacks

        void IFocusHandler.OnFocusEnter(FocusEventData eventData) { }

        void IFocusHandler.OnFocusExit(FocusEventData eventData) { }

        void IFocusHandler.OnFocusChanged(FocusEventData eventData)
        {
            if (eventData.NewFocusedObject != null && Isinteractable(eventData.NewFocusedObject))
            {
                FocusEnter(eventData.NewFocusedObject, eventData);
            }

            if (eventData.OldFocusedObject != null && Isinteractable(eventData.OldFocusedObject))
            {
                FocusExit(eventData.OldFocusedObject, eventData);
            }

            CheckLockFocus(eventData.Pointer);
        }

        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            if (Isinteractable(eventData.selectedObject))
            {
                InputDown(eventData.selectedObject, eventData);
            }
        }

        void IInputHandler.OnInputPressed(InputPressedEventData eventData) { }

        void IInputHandler.OnInputPositionChanged(InputPositionEventData eventData) { }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            if (Isinteractable(eventData.selectedObject))
            {
                InputUp(eventData.selectedObject, eventData);
            }
        }

        void IPointerHandler.OnPointerUp(ClickEventData eventData) { }

        void IPointerHandler.OnPointerDown(ClickEventData eventData) { }

        void IPointerHandler.OnPointerClicked(ClickEventData eventData)
        {
            if (Isinteractable(eventData.selectedObject))
            {
                InputClicked(eventData.selectedObject, eventData);
            }
        }

        void IHoldHandler.OnHoldStarted(InputEventData eventData)
        {
            if (Isinteractable(eventData.selectedObject))
            {
                HoldStarted(eventData.selectedObject, eventData);
            }
        }

        void IHoldHandler.OnHoldCompleted(InputEventData eventData)
        {
            if (Isinteractable(eventData.selectedObject))
            {
                HoldCompleted(eventData.selectedObject, eventData);
            }
        }

        void IHoldHandler.OnHoldCanceled(InputEventData eventData)
        {
            if (Isinteractable(eventData.selectedObject))
            {
                HoldCanceled(eventData.selectedObject, eventData);
            }
        }

        void IManipulationHandler.OnManipulationStarted(ManipulationEventData eventData)
        {
            if (Isinteractable(eventData.selectedObject))
            {
                ManipulationStarted(eventData.selectedObject, eventData);
            }
        }

        void IManipulationHandler.OnManipulationUpdated(ManipulationEventData eventData)
        {
            if (Isinteractable(eventData.selectedObject))
            {
                ManipulationUpdated(eventData.selectedObject, eventData);
            }
        }

        void IManipulationHandler.OnManipulationCompleted(ManipulationEventData eventData)
        {
            if (Isinteractable(eventData.selectedObject))
            {
                ManipulationCompleted(eventData.selectedObject, eventData);
            }
        }

        void IManipulationHandler.OnManipulationCanceled(ManipulationEventData eventData)
        {
            if (Isinteractable(eventData.selectedObject))
            {
                ManipulationCanceled(eventData.selectedObject, eventData);
            }
        }

        void INavigationHandler.OnNavigationStarted(NavigationEventData eventData)
        {
            if (Isinteractable(eventData.selectedObject))
            {
                NavigationStarted(eventData.selectedObject, eventData);
            }
        }

        void INavigationHandler.OnNavigationUpdated(NavigationEventData eventData)
        {
            if (Isinteractable(eventData.selectedObject))
            {
                NavigationUpdated(eventData.selectedObject, eventData);
            }
        }

        void INavigationHandler.OnNavigationCompleted(NavigationEventData eventData)
        {
            if (Isinteractable(eventData.selectedObject))
            {
                NavigationCompleted(eventData.selectedObject, eventData);
            }
        }

        void INavigationHandler.OnNavigationCanceled(NavigationEventData eventData)
        {
            if (Isinteractable(eventData.selectedObject))
            {
                NavigationCanceled(eventData.selectedObject, eventData);
            }
        }

        #endregion Global Listener Callbacks

        #region Protected Virtual Callback Functions
        protected virtual void FocusEnter(GameObject obj, FocusEventData focusEventData) { }
        protected virtual void FocusExit(GameObject obj, FocusEventData focusEventData) { }

        protected virtual void InputDown(GameObject obj, InputEventData eventData) { }
        protected virtual void InputUp(GameObject obj, InputEventData eventData) { }
        protected virtual void InputClicked(GameObject obj, ClickEventData eventData) { }

        protected virtual void HoldStarted(GameObject obj, InputEventData eventData) { }
        protected virtual void HoldCompleted(GameObject obj, InputEventData eventData) { }
        protected virtual void HoldCanceled(GameObject obj, InputEventData eventData) { }

        protected virtual void ManipulationStarted(GameObject obj, ManipulationEventData eventData) { }
        protected virtual void ManipulationUpdated(GameObject obj, ManipulationEventData eventData) { }
        protected virtual void ManipulationCompleted(GameObject obj, ManipulationEventData eventData) { }
        protected virtual void ManipulationCanceled(GameObject obj, ManipulationEventData eventData) { }

        protected virtual void NavigationStarted(GameObject obj, NavigationEventData eventData) { }
        protected virtual void NavigationUpdated(GameObject obj, NavigationEventData eventData) { }
        protected virtual void NavigationCompleted(GameObject obj, NavigationEventData eventData) { }
        protected virtual void NavigationCanceled(GameObject obj, NavigationEventData eventData) { }
        #endregion

    }
}
