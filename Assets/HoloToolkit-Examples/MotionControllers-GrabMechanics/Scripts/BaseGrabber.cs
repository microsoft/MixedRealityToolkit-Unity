using System.Collections.Generic;
using UnityEngine;

namespace MRTK.Grabbables
{
    /// <summary>
    /// Intended usage: scripts that inherit from this can be attached to the controller, or any object with a collider 
    /// that needs to grabbing or carrying other objects. 
    /// </summary>
    public abstract class BaseGrabber : MonoBehaviour
    {
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

        public GrabStateEnum HoverState
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

        public BaseGrabbable GrabbedObjectPrimary
        {
            get
            {
                return (grabbedObjects.Count > 0) ? grabbedObjects.Peek() : null;
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

        public float Strength { get { return strength; } set { strength = value; } }

        public bool IsGrabbing (BaseGrabbable grabbable)
        {
            return grabbedObjects.Contains(grabbable);
        }

        /// <summary>
        /// If the correct grabbing button is pressed, we set the GrabActive to true.
        /// Grab behaviour depends on the combination of grabactive being true, and a grabbable trigger entered
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void GrabStart()
        {
            // Clean out the list of available objects list
            for (int i = contactObjects.Count - 1; i >= 0; i--)
            {
                if (contactObjects[i] == null || !contactObjects[i].isActiveAndEnabled)
                    contactObjects.RemoveAt(i);
            }
            // If there are any left after pruning
            if (contactObjects.Count > 0)
            {
                // Sort by distance and try to grab the closest
                SortAvailable();
                BaseGrabbable closestAvailable = contactObjects[0];
                if (closestAvailable.TryToGrabWith(this))
                {
                    grabbedObjects.Push(contactObjects[0]);
                }
            }
        }

        /// <summary>
        /// If the correct grabbing button is pressed, we set the GrabActive to true.
        /// Grab behaviour depends on the combination of grabactive being true, and a grabbable trigger entered
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
                contactObjects.Add(availableObject);
        }

        /// <summary>
        /// Removes a grabbable object from the list of available objects
        /// </summary>
        /// <param name="availableObject"></param>

        protected void RemoveContact (BaseGrabbable availableObject)
        {
            contactObjects.Remove(availableObject);
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

        public Vector3 GetCurrentPosition()
        {
            return currPos;
        }

        public Vector3 GetPreviousPosition()
        {
            return prevPos;
        }

        void Update()
        {
            currPos = transform.position;
        }

        void LateUpdate()
        {
            prevPos = transform.position;
        }

        //variable declaration
        [SerializeField]
        protected Transform grabAttachSpot;
        protected float grabForgivenessRadius;

        private Stack<BaseGrabbable> grabbedObjects = new Stack<BaseGrabbable>();
        private List<BaseGrabbable> contactObjects = new List<BaseGrabbable>();

        private Rigidbody rb;
        private GameObject myGrabbedObject;
        private float scaleMulitplier;
        private Vector3 attachPoint;
        [SerializeField]
        private float strength = 1.0f;
        private Vector3 currPos;
        private Vector3 prevPos;
    }
}