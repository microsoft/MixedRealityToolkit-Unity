using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MRTK;
using MRTK.Grabbables;

public static class ExtensionMethod  {

    public static ControllerReleaseData GetThrowReleasedVelocityAndAngularVelocity(this Rigidbody rb, Vector3 centerOfMassOfRigidbody, Pose ControllerPose)
    {
        Vector3 vel = Vector3.zero;
        Vector3 angVel = Vector3.zero;



        return new ControllerReleaseData(vel, angVel);
    }



}
