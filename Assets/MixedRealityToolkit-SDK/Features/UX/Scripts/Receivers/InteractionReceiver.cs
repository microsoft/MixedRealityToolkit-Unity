// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;
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
        [Tooltip( "Target interactable Object to receive events for" )]
        private List<GameObject> interactables = new List<GameObject>();

        public List<GameObject> Interactables
        {
            get { return interactables; }
            private set { value = interactables; }
        }

        /// <summary>
        /// Is Focus required to receive input events on this GameObject?
        /// InteractionReceivers handle events for objects indirectly, in order for this class work properly, keep this set to false.
        /// </summary>
        public override bool IsFocusRequired
        {
            get { return base.IsFocusRequired; }
            protected set { base.IsFocusRequired = value; }
        }

        /// <summary>
        /// List of linked targets that the receiver affects
        /// </summary>
        [Tooltip( "Targets for the receiver to affect" )]
        private List<GameObject> targets = new List<GameObject>();

        public List<GameObject> Targets
        {
            get { return targets; }
            private set { value = targets; }
        }

        #endregion

        #region private variables
        /// <summary>
        /// When true, this interaction receiver will draw connections in the editor to Interactables and Targets
        /// </summary>
        [Tooltip( "When true, this interaction receiver will draw connections in the editor to Interactables and Targets" )]
        [SerializeField]
        private bool drawEditorConnections = true;
        
        #endregion

        /// <summary>
        /// On enable, set the BaseInputHandler's IsFocusRequired to false to receive all events.
        /// </summary>
        protected override void OnEnable()
        {
            IsFocusRequired = false;
            base.OnEnable();
        }

        /// <summary>
        /// Register an interactable with this receiver.
        /// </summary>
        /// <param name="interactable">takes a GameObject as the interactable to register.</param>
        public virtual void RegisterInteractable( GameObject interactable )
        {
            if ( interactable == null || interactables.Contains( interactable ) )
            {
                return;
            }

            interactables.Add( interactable );
        }

        /// <summary>
        /// When selected draw lines to all linked interactables
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if ( drawEditorConnections )
            {
                if ( interactables.Count > 0 )
                {
                    GameObject[] interactableList = interactables.ToArray();

                    for ( int i = 0; i < interactableList.Length; i++ )
                    {
                        if ( interactableList[ i ] != null )
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawLine( transform.position, interactableList[ i ].transform.position );
                        }
                    }
                }

                if ( Targets.Count > 0 )
                {
                    GameObject[] targetList = Targets.ToArray();

                    for ( int i = 0; i < targetList.Length; i++ )
                    {
                        if ( targetList[ i ] != null )
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawLine( transform.position, targetList[ i ].transform.position );
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Function to remove an interactable from the linked list.
        /// </summary>
        /// <param name="interactable"></param>
        public virtual void RemoveInteractable( GameObject interactable )
        {
            if ( interactable != null && interactables.Contains( interactable ) )
            {
                interactables.Remove( interactable );
            }
        }

        /// <summary>
        /// Clear the interactables list and unregister them
        /// </summary>
        public virtual void ClearInteractables()
        {
            GameObject[] _intList = interactables.ToArray();

            for ( int i = 0; i < _intList.Length; i++ )
            {
                RemoveInteractable( _intList[ i ] );
            }
        }

        /// <summary>
        /// Is the game object interactable in our list of interactables
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
        protected bool IsInteractable( GameObject interactable )
        {
            return ( interactables != null && interactables.Contains( interactable ) );
        }

        public void OnBeforeFocusChange( FocusEventData eventData ) { /*Unused*/ }

        public void OnFocusChanged( FocusEventData eventData )
        {
            if ( eventData.NewFocusedObject != null && IsInteractable( eventData.NewFocusedObject ) )
            {
                FocusEnter( eventData.NewFocusedObject, eventData );
            }

            if ( eventData.OldFocusedObject != null && IsInteractable( eventData.OldFocusedObject ) )
            {
                FocusExit( eventData.OldFocusedObject, eventData );
            }
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

        protected virtual void FocusEnter( GameObject obj, FocusEventData eventData ) { }
        protected virtual void FocusExit( GameObject obj, FocusEventData eventData ) { }

        protected virtual void InputDown( GameObject obj, InputEventData eventData ) { }
        protected virtual void InputUp( GameObject obj, InputEventData eventData ) { }
        protected virtual void InputClicked( GameObject obj, InputEventData eventData ) { }
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