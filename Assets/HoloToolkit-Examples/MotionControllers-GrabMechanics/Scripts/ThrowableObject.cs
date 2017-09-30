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
            ////////////// Debug.Log("Grabbable object velocity just before BASE throw " + grabbable.AAAAvgVelocity);
            Debug.Log("Grabbable object velocity just before BASE throw " + grabbable.GetAverageVelocity());
            base.Throw(grabbable);
            Debug.Log("Grabbable object velocity just before throw "+ grabbable.GetAverageVelocity());
            GetComponent<Rigidbody>().velocity = grabbable.GetAverageVelocity() * ThrowMultiplier;
            Debug.Log("We're throwing : " + grabbable.GetAverageVelocity());
            //GetComponent<BaseGrabbable>().AvgVelocity = Vector3.zero;
            if (ZeroGravityThrow)
            {
                Debug.Log("This is a zero gravity throw");
                //grabbable.GetComponent<Rigidbody>().useGravity = false;



                grabbable.GetComponent<Rigidbody>().useGravity = false;
                Debug.Log("grabbable ------GRAVITY : " + GetComponent<Rigidbody>().useGravity);
                //Debug.Break();
            }
            //Debug.Break();
        }

        private void Update()
        {
            if (Thrown)
            {
                f += 0.01f;
                if (f < 1)
                {
                    //GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + new Vector3((1.1f * LeftRightCurveOverTime.Evaluate(f)), (1.1f * UpDownCurveOverTime.Evaluate(f)), (1.1f * LeftRightCurveOverTime.Evaluate(f)));
                }
            }
        }

        private float f = 0;
    }
}