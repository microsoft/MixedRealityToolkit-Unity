using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// //Intended Usage//
/// Attach a "grabbable_x" script (a script that inherits from this) to any object that is meant to be grabbed
/// create more specific grab behavior by adding additional scripts/components to the game object, such as scalableObject, rotatableObject, throwableObject 
/// </summary>

namespace MRTK.Grabbables
{
    public enum GrabStateEnum
    {
        Inactive,
        Single,
        Multi,
    }

    public abstract class BaseGrabbable : MonoBehaviour
    {
        public Action<BaseGrabbable> OnGrabStateChange;
        public Action<BaseGrabbable> OnContactStateChange;
        public Action<BaseGrabbable> OnGrabbed;
        public Action<BaseGrabbable> OnReleased;

        public BaseGrabber GrabberPrimary
        {
            get
            {
                return activeGrabbers.Count > 0 ? activeGrabbers.Peek() : null;
            }
            set
            {
                if (!activeGrabbers.Contains(value))
                {
                    activeGrabbers.Push(value);
                }
            }
        }

        public BaseGrabber[] ActiveGrabbers
        {
            get
            {
                List<BaseGrabber> activeGrabbersList = new List<BaseGrabber>();
                foreach (BaseGrabber activeGrabber in activeGrabbers)
                {
                    if (activeGrabber != null && activeGrabber.IsGrabbing(this))
                    {
                        activeGrabbersList.Add(activeGrabber);
                    }
                }
                return activeGrabbersList.ToArray();
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
                    return GrabStateEnum.Multi;
                else if (activeGrabbers.Count > 0)
                    return GrabStateEnum.Single;
                else
                    return GrabStateEnum.Inactive;
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

        public virtual bool TryToGrabWith (BaseGrabber grabber)
        {
            // TODO error checking, mult-grab checking
            if (GrabState != GrabStateEnum.Inactive)
                return false;

            activeGrabbers.Push(grabber);
            StartGrab(grabber);
            return true;
        }

        //left protected unless we have the occasion to use them publicly, then switch to public properties
        [SerializeField]
        protected Transform grabSpot;
        protected Transform myOriginalParent;
        protected bool multiGrabAvailable;
        protected bool AwaitingGrab;
        protected Texture AwaitingGrabVisual;
        protected bool grabbable;
        protected bool StayAttachedOnTeleport;

        //these events for GrabStarted and GrabEnded are subscribed to by scalable, rotatable, and throwable scripts
        /*public delegate void GrabActive(GameObject grabber);
        public static event GrabActive GrabStarted;

        public delegate void GrabFalse(GameObject grabber);
        public static event GrabFalse GrabEnded;*/
        
        protected virtual void Start() {

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
            if (GrabState == GrabStateEnum.Inactive)
            {
                // If we're not already updating our grab state, start now
                activeGrabbers.Push(grabber);
                StartCoroutine(StayGrab());
            }
            else
            {
                // Otherwise just push the grabber
                activeGrabbers.Push(grabber);
            }

            // Attach ourselves to this grabber
            AttachToGrabber(grabber);
            
            if (OnGrabbed != null)
                OnGrabbed(this);
        }

        /// <summary>
        /// As long as the grabber script (usually attached to the controller, but not always) reports GrabActive as true,
        /// we stay inside of StayGrab. If the grabactive is false, then we transition into GrabEnd baheviour.
        /// </summary>
        /// <param name="grabber"></param>
        /// <returns></returns>
        protected virtual IEnumerator StayGrab()
        {            
            // While grabbers are grabbing
            while (GrabState != GrabStateEnum.Inactive)
            {
                // Call on grab stay in case this grabbable wants to update itself
                OnGrabStay();

                // Check to make sure these grabbers actually exist
                while (activeGrabbers.Peek() == null || !activeGrabbers.Peek().IsGrabbing(this))
                {
                    BaseGrabber grabber = activeGrabbers.Pop();
                    if (grabber != null)
                        DetachFromGrabber(grabber);
                }
                yield return null;
            }

            EndGrab();
            yield return null;
        }

        /// <summary>
        /// Grab end fires off a GrabEnded event, but also cleans up some of the variables associated with an active grab, such
        /// as which grabber was grabbing this object and others
        /// </summary>
        /// <param name="grabber"></param>
        protected virtual void EndGrab()
        {
            if (OnReleased != null)
                OnReleased(this);
        }

        /// <summary>
        /// Called every frame while StayGrab is active
        /// </summary>
        protected virtual void OnGrabStay()
        {

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
        protected Stack<BaseGrabber> activeGrabbers = new Stack<BaseGrabber>();

        protected virtual void Update()
        {
            if (prevGrabState != GrabState && OnGrabStateChange != null)
                OnGrabStateChange(this);

            if (prevContactState != ContactState && OnContactStateChange != null)
                OnContactStateChange(this);

            prevGrabState = GrabState;
            prevContactState = ContactState;
        }

        private GrabStateEnum prevGrabState = GrabStateEnum.Inactive;
        private GrabStateEnum prevContactState = GrabStateEnum.Inactive;
    }
}