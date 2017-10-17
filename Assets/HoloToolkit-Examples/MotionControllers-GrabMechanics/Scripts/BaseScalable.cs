// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Examples.Grabbables
{
    /// <summary>
    /// class responsible for two hand scale. Objects with a child of this class attached 
    /// </summary>
    public abstract class BaseScalable : MonoBehaviour
    {
        [SerializeField]
        private bool scaleByDistance = true;

        [SerializeField]
        private BaseGrabbable grabbable;

        private bool readyToScale;
        private float snapShotOfScale;
        private int minScalarNumForScale = 2;
        private bool currentlyScaling;
        private float snapShotDistance;

        protected virtual void Awake()
        {
            if (grabbable == null)
            {
                grabbable = gameObject.GetComponent<BaseGrabbable>();
            }
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
        /// We have two options when we attempt to scale: the first is by velocity and the second is based on the distance between the 
        /// two+ grabbers
        /// </summary>
        public void AttemptScale()
        {
            Debug.Log("Attempt scale");
            BaseGrabber[] activeGrabbers = GetComponent<BaseGrabbable>().ActiveGrabbers;

            if (GetComponent<BaseGrabbable>().ActiveGrabbers.Length >= minScalarNumForScale)
            {
                // Distance
                // snapshot a standard distance that the controls are when the scalable object is engaged
                // That standard distance between controllers corresponds to the localScale * scaleMultiplier
                if (scaleByDistance)
                {
                    if (activeGrabbers.Length >= minScalarNumForScale)
                    {
                        //later this should be average distance between all controllers attached.
                        float dist = Vector3.Distance(activeGrabbers[0].GrabHandle.position, activeGrabbers[1].GrabHandle.position);
                        snapShotDistance = dist;
                        //TODO: scale should not be recorded from x axis alone
                        snapShotOfScale = transform.localScale.x;
                        currentlyScaling = true;
                        StartCoroutine(PerformScaling());
                    }
                }
            }
        }

        /// <summary>
        /// Adding a grabber object to the list of scalars means adding it to the list of scalars and always attempting a scale if there are enough scalars attached
        /// </summary>
        /// <param name="baseGrab"></param>
        public void OnGrabbed(BaseGrabbable baseGrab)
        {
            if (!currentlyScaling)
            {
                AttemptScale();
            }
        }

        /// <summary>
        /// scaling can be amplified by increasing the scaling multiplier 
        /// scaling functionality can also be modified by recording a distance from the user. 
        /// (For example, an object that is further away might scale up more because it is further away from the user)
        /// </summary>
        public virtual IEnumerator PerformScaling()
        {
            currentlyScaling = true;

            while (currentlyScaling)
            {
                //  let go
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
                    transform.localScale = Vector3.one * ((currDistance / snapShotDistance) * snapShotOfScale)  /*scaleMultiplier */ /* distFromUser*/;

                }

                yield return 0;
            }

            currentlyScaling = false;
        }
    }
}
