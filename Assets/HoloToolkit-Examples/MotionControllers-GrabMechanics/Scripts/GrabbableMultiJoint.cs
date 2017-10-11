using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRTK.Grabbables
{
    public class GrabbableMultiJoint : BaseGrabbable
    {
        protected override void OnGrabStay()
        {
            base.OnGrabStay();
            Vector3 averagePosition = transform.position;
            Quaternion averageRotation = transform.rotation;
            int numGrabbers = activeGrabbers.Count;
            float weightPerGrabber = 1f / numGrabbers;

            //if (numGrabbers > 1)
            //{
                foreach (Grabber activeGrabber in activeGrabbers)
                {
                    averagePosition = Vector3.Lerp(averagePosition, activeGrabber.GrabHandle.position, weightPerGrabber);
                    averageRotation = Quaternion.Lerp(averageRotation, activeGrabber.GrabHandle.rotation, weightPerGrabber);
                }

                transform.position = Vector3.Lerp(transform.position, averagePosition, Time.deltaTime * blendSpeed);
                transform.rotation = Quaternion.Lerp(transform.rotation, averageRotation, Time.deltaTime * blendSpeed);
            //}
        }

        //the next three functions provide basic behaviour. Extend from this base script in order to provide more specific functionality.
        protected override void AttachToGrabber(BaseGrabber grabber)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            if(!activeGrabbers.Contains(grabber))
                activeGrabbers.Add(grabber);
        }

        protected override void DetachFromGrabber(BaseGrabber grabber)
        {
            Debug.Log("Detaching form grabber");

        }

        protected override void Update()
        {
            base.Update();
            if (GrabState == GrabStateEnum.Inactive && activeGrabbers.Count == 0)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Rigidbody>().useGravity = true;
            }
        }


        [SerializeField]
        private float blendSpeed = 10f;
    }
}