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
            GetComponent<Rigidbody>().velocity = grabbable.GetAverageVelocity() * ThrowMultiplier;
            if (ZeroGravityThrow)
            {
                grabbable.GetComponent<Rigidbody>().useGravity = false;
            }            
        }
    }
}