// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;
namespace HoloToolkit.Unity.InputModule.Examples.Grabbables
{
    /// <summary>
    /// This type of grab creates a temporary spring joint to attach the grabbed object to the grabber
    /// The fixed joint properties can be assigned here, because the joint will not be created until runtime
    /// </summary>
    public class GrabbableSpringJoint : BaseGrabbable
    {
        //expose the joint variables here for editing because the joint is added/destroyed at runtime
        // to understand how these variables work in greater depth see unity documentation for spring joint and fixed joint
        [SerializeField]
        protected float spring;
        [SerializeField]
        protected float damper;
        [SerializeField]
        protected float breakForce;
        [SerializeField]
        protected float breakTorque;
        [SerializeField]
        protected float tolerance;
        [SerializeField]
        protected Vector3 joint_anchor;
        [SerializeField]
        protected float minDistance;
        [SerializeField]
        protected float maxDistance;

        protected override void AttachToGrabber(BaseGrabber grabber)
        {
            base.AttachToGrabber(grabber);
            SpringJoint joint = gameObject.GetComponent<SpringJoint>();
            if (joint == null)
            {
                joint = gameObject.AddComponent<SpringJoint>();
            }
            joint.connectedBody = grabber.GetComponent<Rigidbody>();
            joint.anchor = new Vector3(0, 0.01f, 0.01f);
            joint.tolerance = tolerance;
            joint.breakForce = breakForce;
            joint.breakTorque = breakTorque;
            joint.spring = spring;
            joint.damper = damper;
        }

        protected override void DetachFromGrabber(BaseGrabber grabber)
        {
            base.DetachFromGrabber(grabber);
            SpringJoint joint = gameObject.GetComponent<SpringJoint>();
            if (joint != null)
            {
                joint.connectedBody = null;
                //Destroy(joint);
                StartCoroutine(DestroyJointAfterDelay(joint));
            }
        }

        protected IEnumerator DestroyJointAfterDelay (SpringJoint joint)
        {
            yield return null;
            if (GrabState == GrabStateEnum.Inactive)
            {
                Destroy(joint);
            }
        }
    }
}