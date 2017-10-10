using System.Collections;
using UnityEngine;

namespace MRTK.Grabbables
{
    /// <summary>
    /// Extends its behaviour from BaseThrowable. This is a non-abstract script that's actually attached to throwable object
    /// This script will not work without a grab script attached to the same gameObject
    /// </summary>


    public class ThrowableObject : BaseThrowable
    {
        public override void Throw(BaseGrabbable grabbable)
        {
            base.Throw(grabbable);
            Vector3 vel = grabbable.GetAverageVelocity();
            Debug.Log("Velocity for GRAB = beofre throw is: " + vel);
            if (GetComponent<GrabbableFixedJoint>() || GetComponent<GrabbableSpringJoint>()) {
                StartCoroutine(ThrowDelay(vel, grabbable));
            }
            else
            {
                GetComponent<Rigidbody>().velocity = vel * ThrowMultiplier;
                if (ZeroGravityThrow)
                {
                    grabbable.GetComponent<Rigidbody>().useGravity = false;
                }
                Debug.Log("Throw happened (no coroutine) ************** OFFICIALLY " + vel * ThrowMultiplier);
            }

        }


        IEnumerator ThrowDelay(Vector3 vel, BaseGrabbable grabbable)
        {
            yield return null;
            GetComponent<Rigidbody>().velocity = vel * ThrowMultiplier;
            if (ZeroGravityThrow)
            {
                grabbable.GetComponent<Rigidbody>().useGravity = false;
            }
            Debug.Log("Throw happened via COROUTINE  ************** OFFICIALLY " + vel *ThrowMultiplier);
            //yield return null;
        }
    }
}