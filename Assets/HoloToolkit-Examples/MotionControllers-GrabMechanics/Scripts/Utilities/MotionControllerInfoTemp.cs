using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MotionControllerInfoTemp  {


    private static Vector3 current;
    private static Vector3 previous;

    public static Vector3 GetVelocity(GameObject go)
    {
        var velocity = Vector3.zero;
        if (go.GetComponent<BaseGrabber>())
        {
            current = go.GetComponent<BaseGrabber>().GetCurrentPosition();
            previous = go.GetComponent<BaseGrabber>().GetPreviousPosition();
            velocity = (current - previous) / Time.deltaTime;            
        }
        return velocity;
    }




}
