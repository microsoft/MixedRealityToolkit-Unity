using System.Collections;
using UnityEngine;

namespace MRTK.Grabbables
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
            if (GetComponent<GrabbableFixedJoint>() || GetComponent<GrabbableSpringJoint>()) {
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

        IEnumerator ThrowDelay(Vector3 vel, Vector3 angVel, BaseGrabbable grabbable)
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