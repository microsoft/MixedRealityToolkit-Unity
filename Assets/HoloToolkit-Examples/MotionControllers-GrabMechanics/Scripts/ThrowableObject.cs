using UnityEngine;

/// <summary>
/// Extends its behaviour from BaseThrowable. This is a non-abstract script that's actually attached to throwable object
/// This script will not work without a grab script attached to the same gameObject
/// </summary>

public class ThrowableObject : BaseThrowable
{
    public override void Throw(GameObject grabber)
    {
        base.Throw(grabber);
        GetComponent<Rigidbody>().velocity = MotionControllerInfoTemp.GetVelocity(grabber)* grabber.GetComponent<Grabber>().Strength * ThrowMultiplier; 
        //GetComponent<Rigidbody>().velocity = -MotionControllerInfoTemp.AngularVelocity(grabber.transform.rotation.eulerAngles);

        if (GetComponent<BaseThrowable>().ZeroGravityThrow)
        {
            GetComponent<Rigidbody>().useGravity = false;
        }
    }

}
