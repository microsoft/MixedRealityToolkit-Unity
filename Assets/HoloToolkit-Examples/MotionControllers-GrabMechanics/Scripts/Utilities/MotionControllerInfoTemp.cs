using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRTK.Grabbables {
    public static class MotionControllerInfoTemp
    {
        private static Vector3 current;
        private static Vector3 previous;

        //public static Vector3 GetVelocity(BaseGrabber grabber)
        //{
        //    Debug.Log("Attempting to fire off GetVelocity");
        //    var velocity = Vector3.zero;
        //    current = grabber.GetCurrentPosition();
        //    previous = grabber.GetPreviousPosition();
        //    velocity = (current - previous) / Time.deltaTime;
        //    Debug.Log("velocity on grabber = "+velocity);
        //    return velocity;
        //}
    }
}
