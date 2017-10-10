using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;


namespace MRTK.Grabbables
{
    /// <summary>
    /// Intended usage: scripts that inherit from this can be attached to the controller, or any object with a collider 
    /// that needs to be grabbing or carrying other objects. 
    /// </summary>
    public abstract class BaseGrabber : MonoBehaviour
    {
        public Action<BaseGrabber> OnGrabStateChange;
        public Action<BaseGrabber> OnContactStateChange;

        public InteractionSourceHandedness Handedness { get { return handedness; } set { handedness = value; } }


        public GrabStateEnum GrabState
        {
            get
            {
                if (grabbedObjects.Count > 1)
                    return GrabStateEnum.Multi;
                else if (grabbedObjects.Count > 0)
                    return GrabStateEnum.Single;
                else
                    return GrabStateEnum.Inactive;
            }
        }

        public GrabStateEnum ContactState
        {
            get
            {
                if (contactObjects.Count > 1)
                    return GrabStateEnum.Multi;
                else if (contactObjects.Count > 0)
                    return GrabStateEnum.Single;
                else
                    return GrabStateEnum.Inactive;
            }
        }

        /// <summary>
        /// If not grabattachpoint is specified, use the gameobject transform by default
        /// </summary>
        public Transform GrabHandle
        {
            get
            {
                return grabAttachSpot != null ? grabAttachSpot : transform;
            }
        }

        public float Strength { get { return strength; } }

        public bool IsGrabbing (BaseGrabbable grabbable)
        {
            return grabbedObjects.Contains(grabbable);
        }

        /// <summary>
        /// Attempts to transfer ownership of grabbable object to another grabber
        /// Can override to 'lock' objects to a grabber, if desired
        /// </summary>
        /// <param name="baseGrabbable"></param>
        /// <param name="grabber"></param>
        /// <returns></returns>
        public virtual bool CanTransferOwnershipTo(BaseGrabbable grabbable, BaseGrabber otherGrabber)
        {
            Debug.Log("Transferring ownership of " + grabbable.name + " to grabber " + otherGrabber.name);
            grabbedObjects.Remove(grabbable);
            return true;
        }

        /// <summary>
        /// If the correct grabbing button is pressed, we add to grabbedObjects.
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void GrabStart()
        {
            // Clean out the list of available objects list
            for (int i = contactObjects.Count - 1; i >= 0; i--)
            {
                if ((contactObjects[i] == null || !contactObjects[i].isActiveAndEnabled) && !grabbedObjects.Contains(contactObjects[i]))
                    contactObjects.RemoveAt(i);
            }
            // If there are any left after pruning
            if (contactObjects.Count > 0)
            {
                // Sort by distance and try to grab the closest
                SortAvailable();
                BaseGrabbable closestAvailable = contactObjects[0];
                if (closestAvailable.TryGrabWith(this))
                {
                    grabbedObjects.Add(contactObjects[0]);
                }
            }
        }

        /// <summary>
        /// If the correct grabbing button is pressed, we set the GrabState.
        /// Grab behaviour depends on the combination of GrabState, and a grabbable trigger entered
        /// </summary>
        protected virtual void GrabEnd()
        {            
            grabbedObjects.Clear();
        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {
            grabbedObjects.Clear();
        }

        /// <summary>
        /// Adds a grabbable object to the list of available objects
        /// </summary>
        /// <param name="availableObject"></param>
        protected void AddContact(BaseGrabbable availableObject)
        {
            if (!contactObjects.Contains(availableObject))
            {
                contactObjects.Add(availableObject);
                availableObject.AddContact(this);
            }
        }

        /// <summary>
        /// Removes a grabbable object from the list of available objects
        /// </summary>
        /// <param name="availableObject"></param>

        protected void RemoveContact (BaseGrabbable availableObject)
        {
            contactObjects.Remove(availableObject);
            availableObject.RemoveContact(this);

            if (contactObjects.Contains (availableObject))
            {

            }
        }

        /// <summary>
        /// Sorts by distance from grab point to grab handle by default
        /// </summary>
        protected virtual void SortAvailable ()
        {
            contactObjects.Sort (delegate (BaseGrabbable b1, BaseGrabbable b2) {
                return Vector3.Distance(b1.GrabPoint, GrabHandle.position).CompareTo(Vector3.Distance(b1.GrabPoint, GrabHandle.position));
            });
        }

        void Update()
        {
            #if UNITY_EDITOR
            if (UnityEditor.Selection.activeGameObject == gameObject)
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    if (GrabState == GrabStateEnum.Inactive)
                    {
                        Debug.Log("Grab start");
                        GrabStart();
                    }
                    else
                    {
                        Debug.Log("Grab end");
                        GrabEnd();
                    }
                }
            }
            #endif


            if (prevGrabState != GrabState && OnGrabStateChange != null)
            {
                Debug.Log("Calling on grab change in grabber");
                OnGrabStateChange(this);
            }

            if (prevContactState != ContactState && OnContactStateChange != null)
            {
                Debug.Log("Calling on contact change in grabber");
                OnContactStateChange(this);
            }

            prevGrabState = GrabState;
            prevContactState = ContactState;
        }


        //variable declaration
        [SerializeField]
        protected Transform grabAttachSpot;
        [SerializeField]
        protected float strength = 1.0f;
        
        protected HashSet<BaseGrabbable> grabbedObjects = new HashSet<BaseGrabbable>();
        protected List<BaseGrabbable> contactObjects = new List<BaseGrabbable>();

        protected float grabForgivenessRadius;

        private GrabStateEnum prevGrabState = GrabStateEnum.Inactive;
        private GrabStateEnum prevContactState = GrabStateEnum.Inactive;               
        [SerializeField]
        protected InteractionSourceHandedness handedness;
    }
}