//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity.Receivers
{
    /// <summary>
    /// An interaction receiver is simply a component that attached to a list of interactible objects and does something
    /// based on events from those interactible objects.  This is the base abstact class to extend from.
    /// </summary>
    public abstract class InteractionReceiver : MonoBehaviour
    {
        #region Public Members
        /// <summary>
        /// List of linked interactible objects to receive events for
        /// </summary>
        [Tooltip("Target Interactible Object to receive events for")]
        public List<GameObject> Interactibles = new List<GameObject>();

        /// <summary>
        /// List of linked targets that the receiver affects
        /// </summary>
        [Tooltip("Targets for the receiver to ")]
        public List<GameObject> Targets = new List<GameObject>();

        /// <summary>
        /// Flag for locking focus while selected
        /// </summary>
        [Tooltip("If true, this object will remain the prime focus while select is held")]
        public bool bLockFocus;
        #endregion

        #region Private and Protected Members
        /// <summary>
        /// Internal protected member for our default gizmo icon
        /// </summary>
        protected string _gizmoIconDefault = "HUX/hux_receiver_icon.png";

        /// <summary>
        /// Internal protected member for our gizmo selected icon
        /// </summary>
        protected string _gizmoIconSelected = "HUX/hux_receiver_icon_selected.png";

        /// <summary>
        /// Protected string for the current active gizmo icon
        /// </summary>
        protected string _gizmoIcon;

        /// <summary>
        /// Protected focuser for the current selecting focuser
        /// </summary>
        protected IPointingSource _selectingFocuser;
        #endregion

        /// <summary>
        /// On enable subscrible to all interaction events on elements in the interactibles list.
        /// </summary>
        public virtual void OnEnable()
        {
            InputManager.Instance.AddGlobalListener(gameObject);
        }

        /// <summary>
        /// On disable remove all linked interacibles from the delegate functions
        /// </summary>
        public virtual void OnDisable()
        {
            InputManager.Instance.RemoveGlobalListener(gameObject);
        }

        /// <summary>
        /// Register an interactible with this receiver.
        /// </summary>
        /// <param name="interactible">takes a GameObject as the interactible to register.</param>
        public virtual void RegisterInteractible(GameObject interactible)
        {
            if (interactible == null || Interactibles.Contains(interactible))
            {
                return;
            }

            Interactibles.Add(interactible);
        }

#if UNITY_EDITOR
        /// <summary>
        /// When selected draw lines to all linked interactibles
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if (this.Interactibles.Count > 0)
            {
                GameObject[] bioList = this.Interactibles.ToArray();

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

        /// <summary>
        /// On Draw Gizmo show the receiver icon
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            _gizmoIcon = UnityEditor.Selection.activeGameObject == this.gameObject ? _gizmoIconSelected : _gizmoIconDefault;
            Gizmos.DrawIcon(this.transform.position, _gizmoIcon, false);
        }
#endif

        /// <summary>
        /// Function to remove an interactible from the linked list.
        /// </summary>
        /// <param name="interactible"></param>
        public virtual void RemoveInteractible(GameObject interactible)
        {
            if (interactible != null && Interactibles.Contains(interactible))
            {
                Interactibles.Remove(interactible);
            }
        }

        /// <summary>
        /// Clear the interactibles list and unregister them
        /// </summary>
        public virtual void ClearInteractibles()
        {
            GameObject[] _intList = Interactibles.ToArray();

            for (int i = 0; i < _intList.Length; i++)
            {
                this.RemoveInteractible(_intList[i]);
            }
        }

        /// <summary>
        /// Is the game object interactible in our list of interactibles
        /// </summary>
        /// <param name="interactible"></param>
        /// <returns></returns>
        protected bool IsInteractible(GameObject interactible)
        {
            return (Interactibles != null && Interactibles.Contains(interactible));
        }

        private void CheckLockFocus(IPointingSource focuser)
        {
            if (bLockFocus)
            {
                //LockFocus(focuser);
            }
        }

        private void LockFocus(IPointingSource focuser)
        {
            if (focuser != null)
            {
                ReleaseFocus();
                _selectingFocuser = focuser;
                // _selectingFocuser.LockFocus();
            }
        }

        private void ReleaseFocus()
        {
            if (_selectingFocuser != null)
            {
                // _selectingFocuser.ReleaseFocus();
                _selectingFocuser = null;
            }
        }

        #region Global Listener Callbacks

        public void OnFocusEnter(PointerSpecificEventData eventData)
        {
            if (IsInteractible(eventData.selectedObject))
            {
                FocusEnter(eventData.selectedObject, eventData);
            }
        }

        public void OnFocusExit(PointerSpecificEventData eventData)
        {
            if (IsInteractible(eventData.selectedObject))
            {
                FocusExit(eventData.selectedObject, eventData);
            }
        }

        public void OnInputDown(InputEventData eventData)
        {
            if (IsInteractible(eventData.selectedObject))
            {
                InputDown(eventData.selectedObject, eventData);
            }
        }

        public void OnInputUp(InputEventData eventData)
        {
            if (IsInteractible(eventData.selectedObject))
            {
                InputUp(eventData.selectedObject, eventData);
            }
        }

        public void OnHoldStarted(HoldEventData eventData)
        {
            if (IsInteractible(eventData.selectedObject))
            {
                HoldStarted(eventData.selectedObject, eventData);
            }
        }

        public void OnHoldCompleted(HoldEventData eventData)
        {
            if (IsInteractible(eventData.selectedObject))
            {
                HoldCompleted(eventData.selectedObject, eventData);
            }
        }

        public void OnHoldCanceled(HoldEventData eventData)
        {
            if (IsInteractible(eventData.selectedObject))
            {
                HoldCanceled(eventData.selectedObject, eventData);
            }
        }

        public void OnManipulationStarted(ManipulationEventData eventData)
        {
            if (IsInteractible(eventData.selectedObject))
            {
                ManipulationStarted(eventData.selectedObject, eventData);
            }
        }

        public void OnManipulationUpdated(ManipulationEventData eventData)
        {
            if (IsInteractible(eventData.selectedObject))
            {
                ManipulationUpdated(eventData.selectedObject, eventData);
            }
        }

        public void OnManipulationCompleted(ManipulationEventData eventData)
        {
            if (IsInteractible(eventData.selectedObject))
            {
                ManipulationCompleted(eventData.selectedObject, eventData);
            }
        }

        public void OnManipulationCanceled(ManipulationEventData eventData)
        {
            if (IsInteractible(eventData.selectedObject))
            {
                ManipulationCanceled(eventData.selectedObject, eventData);
            }
        }

        public void OnNavigationStarted(NavigationEventData eventData)
        {
            if (IsInteractible(eventData.selectedObject))
            {
                NavigationStarted(eventData.selectedObject, eventData);
            }
        }

        public void OnNavigationUpdated(NavigationEventData eventData)
        {
            if (IsInteractible(eventData.selectedObject))
            {
                NavigationUpdated(eventData.selectedObject, eventData);
            }
        }

        public void OnNavigationCompleted(NavigationEventData eventData)
        {
            if (IsInteractible(eventData.selectedObject))
            {
                NavigationCompleted(eventData.selectedObject, eventData);
            }
        }

        public void OnNavigationCanceled(NavigationEventData eventData)
        {
            if (IsInteractible(eventData.selectedObject))
            {
                NavigationCanceled(eventData.selectedObject, eventData);
            }
        }
        #endregion

        #region Protected Virtual Callback Functions
        protected virtual void FocusEnter(GameObject obj, PointerSpecificEventData eventData) { }

        protected virtual void FocusExit(GameObject obj, PointerSpecificEventData eventData) { }

        protected virtual void InputDown(GameObject obj, InputEventData eventData) { }
        protected virtual void InputUp(GameObject obj, InputEventData eventData) { }

        protected virtual void HoldStarted(GameObject obj, HoldEventData eventData) { }
        protected virtual void HoldCompleted(GameObject obj, HoldEventData eventData) { }
        protected virtual void HoldCanceled(GameObject obj, HoldEventData eventData) { }

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
