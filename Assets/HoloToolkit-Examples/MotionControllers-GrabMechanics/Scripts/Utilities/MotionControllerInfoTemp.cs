using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MotionControllerInfoTemp  {


    private static Vector3 current;
    private static Vector3 previous;

    public static Vector3 GetVelocity(Grabber grabber)
    {
        current = grabber.GetCurrentPosition();
        previous = grabber.GetPreviousPosition();
        var velocity = (current - previous) / Time.deltaTime;
        //Debug.Log("Controller Velocity is: " + velocity);
        return velocity;
    }




}
