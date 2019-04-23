// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class PokePointer : BaseControllerPointer, IMixedRealityNearPointer
    {
        [SerializeField]
        protected float distBack;

        [SerializeField]
        protected float distFront;

        [SerializeField]
        protected float debounceThreshold;

        [SerializeField]
        protected LineRenderer line;

        [SerializeField]
        protected GameObject visuals;
        
        private float closestDistance = 0.0f;

        // The closest touchable component limits the set of objects which are currently touchable.
        // These are all the game objects in the subtree of the closest touchable component's owner object.
        private NearInteractionTouchable closestProximityTouchable = null;
        // The current object that is being touched. We need to make sure to consistently fire 
        // poke-down / poke-up events for this object. This is also the case when the object within
        // the same current closest touchable component's changes (e.g. Unity UI control elements).
        private GameObject currentTouchableObjectDown = null;

        protected void OnValidate()
        {
            Debug.Assert(distBack > 0, this);
            Debug.Assert(distFront > 0, this);
            Debug.Assert(debounceThreshold > 0, this);
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

            // Check proximity
            NearInteractionTouchable newClosestTouchable = null;
            {
                closestDistance = distFront; // NOTE: Start at distFront for cutoff
                foreach (var prox in NearInteractionTouchable.Instances)
                {
                    if (prox.ColliderEnabled)
                    {
                       float dist = prox.DistanceToSurface(Position);
                       if (dist < closestDistance)
                       {
   
                           closestDistance = dist;
                           newClosestTouchable = prox;
                       }
                    }
                }
            }

            // Determine ray direction
            Vector3 rayDirection = Rotation * Vector3.forward;
            if (newClosestTouchable != null)
            {
                rayDirection = -newClosestTouchable.Forward;
            }

            // Build ray (poke from in front to the back of the pointer position)
            Vector3 start = Position - distBack * rayDirection;
            Vector3 end = Position + distFront * rayDirection;
            Rays[0].UpdateRayStep(ref start, ref end);

            // Check if the currently touched object is still part of the new touchable.
            if (currentTouchableObjectDown != null)
            {
                if (!IsObjectPartOfTouchable(currentTouchableObjectDown, newClosestTouchable))
                {
                    TryRaisePokeUp(Result.CurrentPointerTarget, Position);
                }
            }

            line.SetPosition(0, Position);
            line.SetPosition(1, end);

            // Set new touchable only now: If we have to raise a poke-up event for the previous touchable object,
            // we need to to so using the previous touchable in TryRaisePokeUp().
            closestProximityTouchable = newClosestTouchable;

            IsActive = IsNearObject;
            visuals.SetActive(IsNearObject);
        }

        public override void OnPostSceneQuery()
        {
            base.OnPostSceneQuery();

            if (Result?.CurrentPointerTarget != null)
            {
                float dist = Vector3.Distance(Result.StartPoint, Result.Details.Point) - distBack;
                bool newIsDown = (dist < debounceThreshold);

                if (newIsDown)
                {
                    TryRaisePokeDown(Result.CurrentPointerTarget, Position);
                }
                else
                {
                    TryRaisePokeUp(Result.CurrentPointerTarget, Position);
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
                        MixedRealityToolkit.InputSystem?.RaisePointerDown(this, pointerAction, Handedness);
                    }
                    else if (closestProximityTouchable.EventsToReceive == TouchableEventType.Touch)
                    {
                        MixedRealityToolkit.InputSystem?.RaiseOnTouchStarted(InputSourceParent, Controller, Handedness, touchPosition);
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
                    MixedRealityToolkit.InputSystem?.RaisePointerUp(this, pointerAction, Handedness);
                }
                else if (closestProximityTouchable.EventsToReceive == TouchableEventType.Touch)
                {
                    MixedRealityToolkit.InputSystem?.RaiseOnTouchCompleted(InputSourceParent, Controller, Handedness, touchPosition);
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
                    MixedRealityToolkit.InputSystem?.RaiseOnTouchUpdated(InputSourceParent, Controller, Handedness, touchPosition);
                }
            }
        }

        private static bool IsObjectPartOfTouchable(GameObject targetObject, NearInteractionTouchable touchable)
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
            normal = (closestProximityTouchable != null) ? closestProximityTouchable.Forward : Vector3.forward;
            return true;
        }

        /// <inheritdoc />
        public override void OnSourceLost(SourceStateEventData eventData)
        {
            TryRaisePokeUp();

            base.OnSourceLost(eventData);
        }
    }
}
