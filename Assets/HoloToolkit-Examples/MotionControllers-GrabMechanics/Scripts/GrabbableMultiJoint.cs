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

            foreach (Grabber activeGrabber in activeGrabbers)
            {
                averagePosition = Vector3.Lerp(averagePosition, activeGrabber.GrabHandle.position, weightPerGrabber);
                averageRotation = Quaternion.Lerp(averageRotation, activeGrabber.GrabHandle.rotation, weightPerGrabber);
            }

            transform.position = Vector3.Lerp(transform.position, averagePosition, Time.deltaTime * blendSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, averageRotation, Time.deltaTime * blendSpeed);
        }

        [SerializeField]
        private float blendSpeed = 10f;
    }
}