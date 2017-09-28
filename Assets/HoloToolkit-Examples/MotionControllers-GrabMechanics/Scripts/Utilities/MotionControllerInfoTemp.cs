using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRTK.Grabbables {
    public static class MotionControllerInfoTemp
    {


        private static Vector3 current;
        private static Vector3 previous;

        public static Vector3 GetVelocity(BaseGrabber grabbable)
        {
            var velocity = Vector3.zero;
            current = grabbable.GetCurrentPosition();
            previous = grabbable.GetPreviousPosition();
            velocity = (current - previous) / Time.deltaTime;
            return velocity;
        }
    }
}
