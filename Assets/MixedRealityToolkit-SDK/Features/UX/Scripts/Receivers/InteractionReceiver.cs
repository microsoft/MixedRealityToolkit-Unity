// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using Microsoft.MixedReality.Toolkit.SDK.Input;
using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Receivers
{
    /// <summary>
    /// An interaction receiver is simply a component that attached to a list of interactable objects and does something
    /// based on events from those interactable objects.  This is the base abstract class to extend from.
    /// </summary>
    public abstract class InteractionReceiver : BaseInputHandler,
        IMixedRealityFocusChangedHandler,
        IMixedRealityGestureHandler,
        IMixedRealityInputHandler,
        IMixedRealityGestureHandler<Vector2>,
        IMixedRealityGestureHandler<Vector3>,
        IMixedRealityGestureHandler<Quaternion>

    {
        #region Public Members
        /// <summary>
        /// List of linked interactable objects to receive events for
        /// </summary>
        [SerializeField]
        [ Tooltip("Target interactable Object to receive events for")]
        private List<GameObject> interactables = new List<GameObject>();

        public List<GameObject> Interactables
        {
            get { return interactables; }
            set { value = interactables; }
        }

        /// <summary>
        /// List of linked targets that the receiver affects
        /// </summary>
        [Tooltip("Targets for the receiver to ")]
        private List<GameObject> targets = new List<GameObject>();

        public List<GameObject> Targets
        {
            get { return targets; }
            set { value = targets; }
        }

        [Tooltip( "If true, this object will remain the prime focus while select is held" )]
        [SerializeField]
        private bool lockFocus = false;

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

        /// <summary>
        /// Protected focuser for the current selecting focuser
        /// </summary>
        protected IMixedRealityPointer _selectingFocuser;
        
        #endregion

        /// <summary>
        /// On start subscribe to all interaction events on elements in the interactables list.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            //Still need to register since focus isn't required
            if(InputSystem != null && InputSystem.IsInputEnabled)
            {
                InputSystem.Register( gameObject );
            }
        }

        /// <summary>
        /// On disable remove all linked interactables from the delegate functions
        /// </summary>
        protected override void OnDisable()
        {
            //Clean up our event subscription
            if ( InputSystem != null && InputSystem.IsInputEnabled )
            {
                InputSystem.Unregister( gameObject );
            }
            base.OnDisable();
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
        public virtual void ClearInteractables()
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
        protected bool IsInteractable(GameObject interactable)
        {
            return (interactables != null && interactables.Contains(interactable));
        }

        private void CheckLockFocus( IMixedRealityPointer focuser )
        {
            // If our previous selecting focuser isn't the same
            if (_selectingFocuser != null && _selectingFocuser != focuser)
            {
                // If our focus is currently locked, unlock it before moving on
                if (LockFocus)
                {
                    _selectingFocuser.IsFocusLocked = false;
                }
            }

            // Set to the new focuser
            _selectingFocuser = focuser;
            if (_selectingFocuser != null)
            {
                _selectingFocuser.IsFocusLocked = LockFocus;
            }
        }

        private void LockFocuser( IMixedRealityPointer focuser )
        {
            if (focuser != null)
            {
                ReleaseFocuser();
                _selectingFocuser = focuser;
                _selectingFocuser.IsFocusLocked = true;
            }
        }

        private void ReleaseFocuser()
        {
            if (_selectingFocuser != null)
            {
                _selectingFocuser.IsFocusLocked = false;
                _selectingFocuser = null;
            }
        }

        public void OnBeforeFocusChange( FocusEventData eventData ) { /*Unused*/ }

        public void OnFocusChanged( FocusEventData eventData )
        {
            FocusEventData newData = new FocusEventData( EventSystem.current );
            newData.Initialize( eventData.Pointer );

            if ( eventData.NewFocusedObject != null && IsInteractable( eventData.NewFocusedObject ) )
            {
                FocusEnter( eventData.NewFocusedObject, newData );
            }

            if ( eventData.OldFocusedObject != null && IsInteractable( eventData.OldFocusedObject ) )
            {
                FocusExit( eventData.OldFocusedObject, newData );
            }

            CheckLockFocus( eventData.Pointer );
        }

        #region Global Listener Callbacks

        public void OnGestureStarted( InputEventData eventData )
        {
            if ( IsInteractable( eventData.selectedObject ) )
            {
                GestureStarted( eventData.selectedObject, eventData );
            }
        }

        public void OnGestureUpdated( InputEventData eventData )
        {
            if ( IsInteractable( eventData.selectedObject ) )
            {
                GestureUpdated( eventData.selectedObject, eventData );
            }
        }

        public void OnGestureUpdated( InputEventData<float> eventData )
        {
            if ( IsInteractable( eventData.selectedObject ) )
            {
                GestureUpdated( eventData.selectedObject, eventData );
            }
        }
        public void OnGestureUpdated( InputEventData<Vector2> eventData )
        {
            if ( IsInteractable( eventData.selectedObject ) )
            {
                GestureUpdated( eventData.selectedObject, eventData );
            }
        }
        public void OnGestureUpdated( InputEventData<Vector3> eventData )
        {
            if ( IsInteractable( eventData.selectedObject ) )
            {
                GestureUpdated( eventData.selectedObject, eventData );
            }
        }
        public void OnGestureUpdated( InputEventData<Quaternion> eventData )
        {
            if ( IsInteractable( eventData.selectedObject ) )
            {
                GestureUpdated( eventData.selectedObject, eventData );
            }
        }

        public void OnGestureCompleted( InputEventData eventData )
        {
            if ( IsInteractable( eventData.selectedObject ) )
            {
                GestureCompleted( eventData.selectedObject, eventData );
            }

        }
        public void OnGestureCompleted( InputEventData<float> eventData )
        {
            if ( IsInteractable( eventData.selectedObject ) )
            {
                GestureCompleted( eventData.selectedObject, eventData );
            }

        }
        public void OnGestureCompleted( InputEventData<Vector2> eventData )
        {
            if ( IsInteractable( eventData.selectedObject ) )
            {
                GestureCompleted( eventData.selectedObject, eventData );
            }

        }

        public void OnGestureCompleted( InputEventData<Vector3> eventData )
        {
            if ( IsInteractable( eventData.selectedObject ) )
            {
                GestureCompleted( eventData.selectedObject, eventData );
            }

        }

        public void OnGestureCompleted( InputEventData<Quaternion> eventData )
        {
            if ( IsInteractable( eventData.selectedObject ) )
            {
                GestureCompleted( eventData.selectedObject, eventData );
            }

        }
        
        public void OnGestureCanceled( InputEventData eventData )
        {
            if ( IsInteractable( eventData.selectedObject ) )
            {
                GestureCanceled( eventData.selectedObject, eventData );
            }
        }
        public void OnInputUp( InputEventData eventData )
        {
            if ( IsInteractable( eventData.selectedObject ) )
            {
                InputUp( eventData.selectedObject, eventData );
            }
        }

        public void OnInputDown( InputEventData eventData )
        {
            if ( IsInteractable( eventData.selectedObject ) )
            {
                InputDown( eventData.selectedObject, eventData );
            }
        }

        public void OnInputPressed( InputEventData<float> eventData )
        {
            if ( IsInteractable( eventData.selectedObject ) )
            {
                InputPressed( eventData.selectedObject, eventData );
            }
        }

        public void OnPositionInputChanged( InputEventData<Vector2> eventData )
        {
            if ( IsInteractable( eventData.selectedObject ) )
            {
                PositionInputChanged( eventData.selectedObject, eventData );
            }
        }

        #endregion

        #region Protected Virtual Callback Functions

        protected virtual void FocusEnter(GameObject obj, FocusEventData eventData ) { }
        protected virtual void FocusExit(GameObject obj, FocusEventData eventData ) { }

        protected virtual void InputDown(GameObject obj, InputEventData eventData) { }
        protected virtual void InputUp(GameObject obj, InputEventData eventData) { }
        protected virtual void InputClicked(GameObject obj, InputEventData eventData ) { }
        protected virtual void InputPressed( GameObject obj, InputEventData<float> eventData ) { }
        protected virtual void PositionInputChanged( GameObject obj, InputEventData<Vector2> eventData ) { }

        protected virtual void GestureStarted( GameObject obj, InputEventData eventData ) { }
        protected virtual void GestureCanceled( GameObject obj, InputEventData eventData ) { }

        protected virtual void GestureUpdated( GameObject obj, InputEventData eventData ) { }
        protected virtual void GestureUpdated( GameObject obj, InputEventData<float> eventData ) { }
        protected virtual void GestureUpdated( GameObject obj, InputEventData<Vector2> eventData ) { }
        protected virtual void GestureUpdated( GameObject obj, InputEventData<Vector3> eventData ) { }
        protected virtual void GestureUpdated( GameObject obj, InputEventData<Quaternion> eventData ) { }

        protected virtual void GestureCompleted( GameObject obj, InputEventData eventData ) { }
        protected virtual void GestureCompleted( GameObject obj, InputEventData<float> eventData ) { }
        protected virtual void GestureCompleted( GameObject obj, InputEventData<Vector2> eventData ) { }
        protected virtual void GestureCompleted( GameObject obj, InputEventData<Vector3> eventData ) { }
        protected virtual void GestureCompleted( GameObject obj, InputEventData<Quaternion> eventData ) { }

        #endregion
    }
}