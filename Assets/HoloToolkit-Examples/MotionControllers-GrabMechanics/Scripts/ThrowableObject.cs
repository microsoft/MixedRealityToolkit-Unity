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
            GetComponent<Rigidbody>().velocity = MotionControllerInfoTemp.GetVelocity(grabbable.GrabberPrimary) * grabbable.GrabberPrimary.Strength * ThrowMultiplier;
            //GetComponent<Rigidbody>().velocity = -MotionControllerInfoTemp.AngularVelocity(grabber.transform.rotation.eulerAngles);

            if (ZeroGravityThrow)
            {
                GetComponent<Rigidbody>().useGravity = false;
            }
        }
    }
}