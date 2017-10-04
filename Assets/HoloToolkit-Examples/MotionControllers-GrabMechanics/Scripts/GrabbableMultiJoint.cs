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
            //GetComponent<Rigidbody>().isKinematic = true;
            Vector3 averagePosition = transform.position;
            Quaternion averageRotation = transform.rotation;
            int numGrabbers = activeGrabbers.Count;
            float weightPerGrabber = 1f / numGrabbers;

            foreach (Grabber activeGrabber in activeGrabbers)
            {
                averagePosition = Vector3.Lerp(averagePosition, activeGrabber.GrabHandle.position, weightPerGrabber);
                averageRotation = Quaternion.Lerp(averageRotation, activeGrabber.GrabHandle.rotation, weightPerGrabber);
            }

            transform.position = Vector3.Lerp(transform.position, averagePosition, Time.deltaTime * blendSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, averageRotation, Time.deltaTime * blendSpeed);
            //GetComponent<Rigidbody>().isKinematic = false;
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
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;
        }


        [SerializeField]
        private float blendSpeed = 10f;


    }



}