// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Examples.Grabbables
{
    /// <summary>
    /// Extends its behaviour from BaseThrowable. This is a non-abstract script that can be attached to throwable object
    /// This script will not work without a grab script attached to the same gameObject
    /// </summary>
    public class ThrowableObject : BaseThrowable
    {
        public override void Throw(BaseGrabbable grabbable)
        {
            base.Throw(grabbable);
            //Vector3 vel = grabbable.GetAverageVelocity();
            Vector3 vel = LatestControllerThrowVelocity;
            Vector3 angVel = LatestControllerThrowAngularVelocity;
            if (GetComponent<GrabbableFixedJoint>() || GetComponent<GrabbableSpringJoint>())
            {
                StartCoroutine(ThrowDelay(vel, angVel, grabbable));
            }
            else
            {
                GetComponent<Rigidbody>().velocity = vel * ThrowMultiplier;
                GetComponent<Rigidbody>().angularVelocity = angVel;
                if (ZeroGravityThrow)
                {
                    grabbable.GetComponent<Rigidbody>().useGravity = false;
                }
            }
        }

        private IEnumerator ThrowDelay(Vector3 vel, Vector3 angVel, BaseGrabbable grabbable)
        {
            yield return null;
            GetComponent<Rigidbody>().velocity = vel * ThrowMultiplier;
            GetComponent<Rigidbody>().angularVelocity = angVel;
            if (ZeroGravityThrow)
            {
                grabbable.GetComponent<Rigidbody>().useGravity = false;
            }
        }
    }
}
