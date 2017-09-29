using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRTK.Grabbables
{
    /// <summary>
    /// class responsible for two hand scale. Objects with a child of this class attached 
    /// </summary>
    public abstract class BaseScalable : MonoBehaviour
    {
        protected virtual void Awake()
        {
            if (grabbable == null)
                grabbable = gameObject.GetComponent<BaseGrabbable>();
        }

        /// <summary>
        /// scale needs to subscribe to grab events in order to add more scalars to the list of scalars
        /// </summary>
        protected virtual void OnEnable()
        {
            grabbable.OnGrabbed += OnGrabbed;
        }

        protected virtual void OnDisable()
        {
            grabbable.OnGrabbed -= OnGrabbed;
        }

        /// <summary>
        /// Taking a snap shot of scale at the moment of grab is important so that we can perform the scale relative to the original size of the game object
        /// </summary>
        protected virtual void SnapShotOfScale()
        {
            snapShotOfScaleVec = transform.localScale;
        }

        /// <summary>
        /// We have two options when we attempt to scale: the first is by velocity and the second is based on the distance between the 
        /// two+ grabbers
        /// </summary>
        public void AttemptScale()
        {
            BaseGrabber[] activeGrabbers = grabbable.ActiveGrabbers;
            if (activeGrabbers.Length >= minScalarNumForScale)
            {
                //Velocity
                //Multiply scale of this scalable object by the velocity of scalar1 and scalar2 (or however many)
                if (scaleByVelocity)
                {
                    foreach (BaseGrabber grabber in grabbable.ActiveGrabbers)
                    {
                        int i = 0;
                        Debug.Log("Velocity of scalar obj " + MotionControllerInfoTemp.GetVelocity(grabber));
                        //Add Velocity scale functionality here
                        i++;
                    }

                    //Distance
                    //snapshot a standard distance that the controls are when the scalable object is engaged
                    //That standard distance between controllers corresponds to the localScale * scaleMultiplier
                    if (scaleByDistance)
                    {
                        if (activeGrabbers.Length == 2)
                        {
                            float dist = Vector3.Distance(activeGrabbers[0].GrabHandle.position, activeGrabbers[1].GrabHandle.position);
                            snapShotDistance = dist;
                            snapShotOfScale = transform.localScale.x;
                            currentlyScaling = true;
                            StartCoroutine(PerformScaling());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adding a grabber object to the list of scalars means adding it to the list of scalars and always attempting a scale if there are enough scalars attached
        /// </summary>
        /// <param name="grabber"></param>
        public void OnGrabbed(BaseGrabbable grabbable)
        {
            if (!currentlyScaling)
            {
                AttemptScale();
            }
        }

        /// <summary>
        /// scaling can be amplified by increasing the scaling mulitplier 
        /// scaling functionality can also be modified by recording a distance from the user. 
        /// (For example, an object that is further away might scale up more because it is further away from the user)
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator PerformScaling()
        {
            currentlyScaling = true;

            while (currentlyScaling)
            {
                // Whoops, we've been let go
                if (grabbable.GrabState == GrabStateEnum.Inactive)
                {
                    currentlyScaling = false;
                    yield break;
                }

                BaseGrabber[] activeGrabbers = grabbable.ActiveGrabbers;

                // If enough grabbers have grabbed
                if (activeGrabbers.Length >= minScalarNumForScale)
                {
                    float currDistance = Vector3.Distance(activeGrabbers[0].GrabHandle.position, activeGrabbers[1].GrabHandle.position);
                    transform.localScale = Vector3.one * ((currDistance / snapShotDistance) * snapShotOfScale) /*scaleMultiplier * distFromUser*/;
                }
                yield return 0;
            }
            currentlyScaling = false;
            yield return null;
        }

        [Range(1, 5)]
        private float scaleMultiplier = 1.0f;
        [SerializeField]
        public bool scaleByVelocity = false;
        [SerializeField]
        private bool scaleByDistance = true;
        private bool readyToScale;
        private Vector3 snapShotOfScaleVec;
        private float snapShotOfScale;
        private int minScalarNumForScale = 2;
        private bool currentlyScaling;
        private float snapShotDistance;

        [SerializeField]
        private BaseGrabbable grabbable;
    }
}
