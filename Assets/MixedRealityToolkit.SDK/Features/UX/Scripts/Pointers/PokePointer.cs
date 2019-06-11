// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class PokePointer : BaseControllerPointer, IMixedRealityNearPointer
    {
        [SerializeField]
        protected LineRenderer line;

        [SerializeField]
        protected GameObject visuals;
        
        private float closestDistance = 0.0f;

        private Vector3 closestNormal = Vector3.forward;

        // The closest touchable component limits the set of objects which are currently touchable.
        // These are all the game objects in the subtree of the closest touchable component's owner object.
        private BaseNearInteractionTouchable closestProximityTouchable = null;
        // The current object that is being touched. We need to make sure to consistently fire 
        // poke-down / poke-up events for this object. This is also the case when the object within
        // the same current closest touchable component's changes (e.g. Unity UI control elements).
        private GameObject currentTouchableObjectDown = null;

        protected void OnValidate()
        {
            Debug.Assert(line != null, this);
            Debug.Assert(visuals != null, this);
        }

        public bool IsNearObject
        {
            get { return (closestProximityTouchable != null); }
        }

        public override void OnPreSceneQuery()
        {
            if (Rays == null)
            {
                Rays = new RayStep[1];
            }

            closestNormal = Rotation * Vector3.forward;

            // Check proximity
            BaseNearInteractionTouchable newClosestTouchable = null;
            {
                closestDistance = float.PositiveInfinity;
                foreach (var prox in BaseNearInteractionTouchable.Instances)
                {
                    if (prox.ColliderEnabled)
                    {
                        Vector3 normal;
                        float dist = prox.DistanceToTouchable(Position, out normal);
                        if (dist < prox.DistFront && dist < closestDistance)
                        {   
                            closestDistance = dist;
                            newClosestTouchable = prox;
                            closestNormal = normal;
                        }
                    }
                }
            }

            if (newClosestTouchable != null)
            {
                // Build ray (poke from in front to the back of the pointer position)
                Vector3 start = Position - newClosestTouchable.DistBack * -closestNormal;
                Vector3 end = Position + newClosestTouchable.DistFront * -closestNormal;
                Rays[0].UpdateRayStep(ref start, ref end);

                line.SetPosition(0, Position);
                line.SetPosition(1, end);
            }

            // Check if the currently touched object is still part of the new touchable.
            if (currentTouchableObjectDown != null)
            {
                if (!IsObjectPartOfTouchable(currentTouchableObjectDown, newClosestTouchable))
                {
                    TryRaisePokeUp(Result.CurrentPointerTarget, Position);
                }
            }

            // Set new touchable only now: If we have to raise a poke-up event for the previous touchable object,
            // we need to do so using the previous touchable in TryRaisePokeUp().
            closestProximityTouchable = newClosestTouchable;

            visuals.SetActive(IsActive);
        }

        public override void OnPostSceneQuery()
        {
            base.OnPostSceneQuery();

            if (!IsActive)
            {
                return;
            }

            if (Result?.CurrentPointerTarget != null && closestProximityTouchable != null)
            {
                float distToFront = Vector3.Distance(Result.StartPoint, Result.Details.Point) - closestProximityTouchable.DistBack;
                bool newIsDown = (distToFront < 0);
                bool newIsUp = (distToFront > closestProximityTouchable.DebounceThreshold);

                if (newIsDown)
                {
                    TryRaisePokeDown(Result.CurrentPointerTarget, Position);
                }
                else if (currentTouchableObjectDown != null)
                {
                    if (newIsUp)
                    {
                        TryRaisePokeUp(Result.CurrentPointerTarget, Position);
                    }
                    else
                    {
                        TryRaisePokeDown(Result.CurrentPointerTarget, Position);
                    }
                }
            }

            if (!IsNearObject)
            {
                line.endColor = line.startColor = new Color(1, 1, 1, 0.25f);
            }
            else if (currentTouchableObjectDown == null)
            {
                line.endColor = line.startColor = new Color(1, 1, 1, 0.75f);
            }
            else
            {
                line.endColor = line.startColor = new Color(0, 0, 1, 0.75f);
            }
        }

        public override void OnPreCurrentPointerTargetChange()
        {
            // We need to raise the event now, since the pointer's focused object or touchable will change 
            // after we leave this function. This will make sure the same object that received the Down event
            // will also receive the Up event.
            TryRaisePokeUp();
        }

        private void TryRaisePokeDown(GameObject targetObject, Vector3 touchPosition)
        {
            if (currentTouchableObjectDown == null)
            {
                // In order to get reliable up/down event behavior, only allow the closest touchable to be touched.
                if (IsObjectPartOfTouchable(targetObject, closestProximityTouchable))
                {
                    currentTouchableObjectDown = targetObject;

                    if (closestProximityTouchable.EventsToReceive == TouchableEventType.Pointer)
                    {
                        InputSystem?.RaisePointerDown(this, pointerAction, Handedness);
                    }
                    else if (closestProximityTouchable.EventsToReceive == TouchableEventType.Touch)
                    {
                        InputSystem?.RaiseOnTouchStarted(InputSourceParent, Controller, Handedness, touchPosition);
                    }
                }
            }
            else
            {
                RaiseTouchUpdated(targetObject, touchPosition);
            }
        }

        private void TryRaisePokeUp(GameObject targetObject, Vector3 touchPosition)
        {
            if (currentTouchableObjectDown != null)
            {
                Debug.Assert(Result.CurrentPointerTarget == currentTouchableObjectDown, "PokeUp will not be raised for correct object.");

                if (closestProximityTouchable.EventsToReceive == TouchableEventType.Pointer)
                {
                    InputSystem.RaisePointerClicked(this, pointerAction, 0, Handedness);
                    InputSystem?.RaisePointerUp(this, pointerAction, Handedness);
                }
                else if (closestProximityTouchable.EventsToReceive == TouchableEventType.Touch)
                {
                    InputSystem?.RaiseOnTouchCompleted(InputSourceParent, Controller, Handedness, touchPosition);
                }

                currentTouchableObjectDown = null;
            }
        }

        private void TryRaisePokeUp()
        {
            if (currentTouchableObjectDown != null)
            {
                TryRaisePokeUp(Result.CurrentPointerTarget, Position);
            }
        }

        private void RaiseTouchUpdated(GameObject targetObject, Vector3 touchPosition)
        {
            if (currentTouchableObjectDown != null)
            {
                Debug.Assert(Result?.CurrentPointerTarget == currentTouchableObjectDown);

                if (closestProximityTouchable.EventsToReceive == TouchableEventType.Touch)
                {
                    InputSystem?.RaiseOnTouchUpdated(InputSourceParent, Controller, Handedness, touchPosition);
                }
            }
        }

        private static bool IsObjectPartOfTouchable(GameObject targetObject, BaseNearInteractionTouchable touchable)
        {
            return targetObject != null && touchable != null &&
                (targetObject == touchable.gameObject ||
                // Descendant game objects are touchable as well. In particular, this is needed to be able to send
                // touch events to Unity UI control elements.
                (targetObject.transform != null && touchable.gameObject.transform != null &&
                targetObject.transform.IsChildOf(touchable.gameObject.transform)));
        }

        /// <inheritdoc />
        bool IMixedRealityNearPointer.TryGetNearGraspPoint(out Vector3 position)
        {
            position = Vector3.zero;
            return false;
        }

        /// <inheritdoc />
        bool IMixedRealityNearPointer.TryGetDistanceToNearestSurface(out float distance)
        {
            distance = closestDistance;
            return true;
        }

        /// <inheritdoc />
        bool IMixedRealityNearPointer.TryGetNormalToNearestSurface(out Vector3 normal)
        {
            normal = closestNormal;
            return true;
        }

        /// <inheritdoc />
        public override void OnSourceLost(SourceStateEventData eventData)
        {
            TryRaisePokeUp();

            base.OnSourceLost(eventData);
        }

        public override void OnInputDown(InputEventData eventData)
        {
            // Poke pointer should not respond when a button is pressed or hand is pinched
            // It should only dispatch events based on collision with touchables.
        }

        public override void OnInputUp(InputEventData eventData)
        {
            // Poke pointer should not respond when a button is released or hand is un-pinched
            // It should only dispatch events based on collision with touchables.
        }
    }
}
