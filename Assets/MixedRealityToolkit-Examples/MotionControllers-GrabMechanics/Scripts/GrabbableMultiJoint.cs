// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.Grabbables
{
    public class GrabbableMultiJoint : BaseGrabbable
    {
        [SerializeField]
        private float blendSpeed = 10f;

        protected override void OnGrabStay()
        {
            base.OnGrabStay();
            Vector3 averagePosition = transform.position;
            Quaternion averageRotation = transform.rotation;
            int numGrabbers = activeGrabbers.Count;
            float weightPerGrabber = 1f / numGrabbers;

            for (var i = 0; i < activeGrabbers.Count; i++)
            {
                var activeGrabber = (Grabber)activeGrabbers[i];
                averagePosition = Vector3.Lerp(averagePosition, activeGrabber.GrabHandle.position, weightPerGrabber);
                averageRotation = Quaternion.Lerp(averageRotation, activeGrabber.GrabHandle.rotation, weightPerGrabber);
            }

            transform.position = Vector3.Lerp(transform.position, averagePosition, Time.deltaTime * blendSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, averageRotation, Time.deltaTime * blendSpeed);
        }

        //the next three functions provide basic behaviour. Extend from this base script in order to provide more specific functionality.

        protected override void AttachToGrabber(BaseGrabber grabber)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            if (!activeGrabbers.Contains(grabber))
            {
                activeGrabbers.Add(grabber);
            }
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
    }
}
