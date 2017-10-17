// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Examples.Grabbables
{
    /// <summary>
    /// This type of grab creates a temporary fixed joint to attach the grabbed object to the grabber
    /// The fixed joint properties can be assigned here, because the joint will not be attached/visible until runtime
    /// </summary>
    public class GrabbableFixedJoint : BaseGrabbable
    {
        // expose the joint variables here for editing because the joint is added/destroyed at runtime
        // to understand how these variables work in greater depth see documentation for spring joint and fixed joint
        [SerializeField]
        protected float breakForce = 20;

        [SerializeField]
        protected float breakTorque = 20;

        [SerializeField]
        protected float tolerance = 0.01f;

        [SerializeField]
        protected Vector3 jointAnchor;

        [SerializeField]
        protected float minDistance;

        [SerializeField]
        protected float maxDistance;

        protected override void AttachToGrabber(BaseGrabber grabber)
        {
            base.AttachToGrabber(grabber);
            FixedJoint joint = GetComponent<FixedJoint>();
            if (joint == null)
            {
                joint = gameObject.AddComponent<FixedJoint>();
            }
            joint.connectedBody = grabber.GetComponent<Rigidbody>();
            joint.anchor = jointAnchor;
            joint.breakForce = breakForce;
            joint.breakTorque = breakTorque;
        }

        protected override void DetachFromGrabber(BaseGrabber grabber)
        {
            base.DetachFromGrabber(grabber);
            FixedJoint joint = GetComponent<FixedJoint>();
            if (joint != null)
            {
                joint.connectedBody = null;
                //DestroyImmediate(joint);
                StartCoroutine(DestroyJointAfterDelay(joint));
            }
        }

        protected IEnumerator DestroyJointAfterDelay(FixedJoint joint)
        {
            yield return null;
            if (GrabState == GrabStateEnum.Inactive)
            {
                DestroyImmediate(joint);
            }
            yield return null;
        }
    }
}
