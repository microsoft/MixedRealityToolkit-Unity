using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MRTK;
using MRTK.Grabbables;
using UnityEngine.XR.WSA.Input;

public static class ExtensionMethods  {

    public static ControllerReleaseData GetThrowReleasedVelocityAndAngularVelocity(this Rigidbody rb, Vector3 centerOfMassOfRigidbody, InteractionSourcePose poseInfo)
    {
        Vector3 setVel = default(Vector3);
        Vector3 angVel = default(Vector3);
        Vector3 controllerPos = default(Vector3);
        Vector3 controllerVelocity = default(Vector3);
        Vector3 controllerAngularVelocity = default(Vector3);
        poseInfo.TryGetPosition(out controllerPos);
        poseInfo.TryGetVelocity(out controllerVelocity);
        poseInfo.TryGetAngularVelocity(out controllerAngularVelocity);
        float dist = Vector3.Distance(centerOfMassOfRigidbody, controllerPos);

        //vel = velocityOfController + angularVelOfController * distBetween grabbable center of mass and controllerPos;
        //setVel = controllerVelocity + controllerAngularVelocity; //+ (controllerAngularVelocity /* *-1*/) * dist;
        setVel = controllerVelocity;
        Debug.Log(" SetVal = ControllerVelocity ( " + controllerVelocity + ") controllerAngVel ((" + controllerAngularVelocity + ") * -1 )" + "  * dist (" + dist+")");

        return new ControllerReleaseData(setVel, angVel);
    }



}
