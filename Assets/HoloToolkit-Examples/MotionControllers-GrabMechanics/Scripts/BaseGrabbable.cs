// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Examples.Grabbables
{
    /// <summary>
    /// //Intended Usage//
    /// Attach a "grabbable_x" script (a script that inherits from this) to any object that is meant to be grabbed
    /// create more specific grab behavior by adding additional scripts/components to the game object, such as scalableObject, rotatableObject, throwableObject 
    /// </summary>
    public abstract class BaseGrabbable : MonoBehaviour
    {
        public event Action<BaseGrabbable> OnGrabStateChange;
        public event Action<BaseGrabbable> OnContactStateChange;
        public event Action<BaseGrabbable> OnGrabbed;
        public event Action<BaseGrabbable> OnReleased;

        public BaseGrabber GrabberPrimary
        {
            get
            {
                return activeGrabbers.Count > 0 ? activeGrabbers[activeGrabbers.Count - 1] : null;
            }
        }

        public BaseGrabber[] ActiveGrabbers
        {
            get
            {
                return activeGrabbers.ToArray();
            }
        }

        public Vector3 GrabPoint
        {
            get
            {
                return grabSpot != null ? grabSpot.position : transform.position;
            }
        }

        /// <summary>
        /// Changes based on how many grabbers are grabbing this object
        /// </summary>
        public GrabStateEnum GrabState
        {
            get
            {
                if (activeGrabbers.Count > 1)
                {
                    return GrabStateEnum.Multi;
                }

                return activeGrabbers.Count > 0 ? GrabStateEnum.Single : GrabStateEnum.Inactive;
            }
        }

        /// <summary>
        /// Changes based on how many grabbers are intersecting with this object
        /// </summary>
        public GrabStateEnum ContactState
        {
            get
            {
                if (availableGrabbers.Count > 1)
                    return GrabStateEnum.Multi;
                else if (availableGrabbers.Count > 0)
                    return GrabStateEnum.Single;
                else
                    return GrabStateEnum.Inactive;
            }
        }

        /// <summary>
        /// Grabbers that could potentially grab this object
        /// This list is maintained by the grabbers
        /// </summary>
        protected HashSet<BaseGrabber> availableGrabbers = new HashSet<BaseGrabber>();

        /// <summary>
        /// Grabbers that are currently grabbing this object
        /// The top-most grabber is the primary grabber
        /// </summary>
        protected List<BaseGrabber> activeGrabbers = new List<BaseGrabber>();

        //left protected unless we have the occasion to use them publicly, then switch to public access
        [SerializeField]
        protected Transform grabSpot;

        [SerializeField]
        protected GrabStyleEnum grabStyle = GrabStyleEnum.Exclusive;

        private GrabStateEnum prevGrabState = GrabStateEnum.Inactive;
        private GrabStateEnum prevContactState = GrabStateEnum.Inactive;
        private Vector3 velocity;
        private Vector3 averageVelocity;
        private Vector3 previousVel;

        public virtual bool TryGrabWith(BaseGrabber grabber)
        {
            // TODO error checking, mult-grab checking
            if (GrabState != GrabStateEnum.Inactive)
            {
                switch (grabStyle)
                {
                    case GrabStyleEnum.Exclusive:
                        // Try to transfer ownership of grabbed object
                        BaseGrabber primary = GrabberPrimary;
                        if (GrabberPrimary.CanTransferOwnershipTo(this, grabber))
                        {
                            // Remove from grabbable list and detatch
                            activeGrabbers.Remove(primary);
                            DetachFromGrabber(primary);
                        }
                        else
                        {
                            // If we can't, it's a no-go
                            return false;
                        }
                        break;
                    case GrabStyleEnum.Multi:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            StartGrab(grabber);
            return true;
        }

        /// <summary>
        /// Adds a grabber object to the list of available grabbers
        /// </summary>
        public void AddContact(BaseGrabber availableObject)
        {
            availableGrabbers.Add(availableObject);
        }

        /// <summary>
        /// Removes a grabber object from the list of available grabbers
        /// </summary>
        public void RemoveContact(BaseGrabber availableObject)
        {
            availableGrabbers.Remove(availableObject);
        }

        //the next three functions provide basic behaviour. Extend from this base script in order to provide more specific functionality.

        protected virtual void AttachToGrabber(BaseGrabber grabber)
        {
            // By default this does nothing
            // In most cases this will parent or create a joint
        }

        protected virtual void DetachFromGrabber(BaseGrabber grabber)
        {
            // By default this does nothing
            // In most cases this will un-parent or destroy a joint
        }

        protected virtual void StartGrab(BaseGrabber grabber)
        {
            Debug.Log("Start grab");
            if (GrabState == GrabStateEnum.Inactive)
            {
                Debug.Log("State is inactive");
                // If we're not already updating our grab state, start now
                activeGrabbers.Add(grabber);
                StartCoroutine(StayGrab());
            }
            else
            {
                Debug.Log("State is not inactive");
                // Otherwise just push the grabber
                activeGrabbers.Add(grabber);
            }

            // Attach ourselves to this grabber
            AttachToGrabber(grabber);
            if (OnGrabbed != null)
            {
                OnGrabbed(this);
            }
        }

        /// <summary>
        /// As long as the grabber script (usually attached to the controller, but not always) reports more than one grabber,
        /// we stay inside of StayGrab. 
        /// </summary>
        protected virtual IEnumerator StayGrab()
        {
            yield return null;

            // While grabbers are grabbing
            while (GrabState != GrabStateEnum.Inactive)
            {
                // Call on grab stay in case this grabbable wants to update itself
                OnGrabStay();
                for (int i = activeGrabbers.Count - 1; i >= 0; i--)
                {
                    if (activeGrabbers[i] == null || !activeGrabbers[i].IsGrabbing(this))
                    {
                        Debug.Log("no longer being grabbed by active grabber");
                        if (activeGrabbers[i] != null)
                        {
                            DetachFromGrabber(activeGrabbers[i]);
                        }

                        activeGrabbers.RemoveAt(i);
                    }
                }
                yield return null;
            }

            EndGrab();
        }
        /// <summary>
        /// Grab end fires off a GrabEnded event, but also cleans up some of the variables associated with an active grab, such
        /// as which grabber was grabbing this object and so forth. 
        /// </summary>
        protected virtual void EndGrab()
        {
            if (OnReleased != null)
            {
                OnReleased(this);
            }
        }

        /// <summary>
        /// Called every frame while StayGrab is active
        /// </summary>
        protected virtual void OnGrabStay()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            if (prevGrabState != GrabState && OnGrabStateChange != null)
            {
                Debug.Log("Calling on grab change in grabbable");
                OnGrabStateChange(this);
            }

            if (prevContactState != ContactState && OnContactStateChange != null)
            {
                Debug.Log("Calling on contact change in grabbable");
                OnContactStateChange(this);
            }

            prevGrabState = GrabState;
            prevContactState = ContactState;
        }
    }
}