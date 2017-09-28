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
        if (GetComponent<BaseThrowable>() != null)
        {
            ///////////////VRTK////////////////////////GetComponent<Rigidbody>().velocity = (VRTK_DeviceFinder.GetControllerVelocity(grabber.gameObject)) * grabber.Strength * GetComponent<OC_ThrowableObject>().ThrowMultiplier;
            ///////////////VRTK/////////////////////*GetComponent<Rigidbody>().angularVelocity = VRTK_DeviceFinder.GetControllerAngularVelocity(grabber.gameObject);*/
            GetComponent<Rigidbody>().velocity = MotionControllerInfoTemp.GetVelocity(grabber.GetComponent<Grabber>());
            //GetComponent<Rigidbody>().velocity = - MotionControllerInfoTemp.AngularVelocity (grabber.transform.rotation.eulerAngles);

            if (GetComponent<BaseThrowable>().ZeroGravityThrow)
            {
                GetComponent<Rigidbody>().useGravity = false;
            }

            Debug.Log("THROWING. Veloctiy = " + GetComponent<Rigidbody>().velocity);
        }
    }

}
