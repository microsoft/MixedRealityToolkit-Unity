// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.WSA.Input;

using System.Collections.Generic;
using System.Linq;

public static class Throwing
{
    public static void GetThrownObjectVelAngVel(Vector3 throwingControllerPos, Vector3 throwingControllerVelocity, Vector3 throwingControllerAngularVelocity,
        Vector3 thrownObjectCenterOfMass, out Vector3 objectVelocity, out Vector3 objectAngularVelocity)
    {
        Vector3 radialVec = thrownObjectCenterOfMass - throwingControllerPos;

        objectVelocity = throwingControllerVelocity + Vector3.Cross(throwingControllerAngularVelocity, radialVec);
        objectAngularVelocity = throwingControllerAngularVelocity;
    }

    public static bool TryGetThrownObjectVelAngVel(InteractionSourcePose throwingControllerPose, Vector3 thrownObjectCenterOfMass, out Vector3 objectVelocity, out Vector3 objectAngularVelocity)
    {
        Vector3 controllerPos, controllerVelocity, controllerAngularVelocity;

        if (!throwingControllerPose.TryGetPosition(out controllerPos) ||
            !throwingControllerPose.TryGetVelocity(out controllerVelocity) ||
            !throwingControllerPose.TryGetAngularVelocity(out controllerAngularVelocity))
        {
            objectVelocity = objectAngularVelocity = default(Vector3);
            return false;
        }

        GetThrownObjectVelAngVel(controllerPos, controllerVelocity, controllerAngularVelocity, thrownObjectCenterOfMass, out objectVelocity, out objectAngularVelocity);
        return true;
    }

    public static bool TryGetNodeState(XRNode node, out XRNodeState nodeState)
    {
        List<XRNodeState> nodeStates = new List<XRNodeState>();
        InputTracking.GetNodeStates(nodeStates);
        nodeState = nodeStates.Where(p => p.nodeType == node).FirstOrDefault();

        return nodeState.tracked;
    }

    public static bool TryGetThrownObjectVelAngVel(XRNode throwingController, Vector3 thrownObjectCenterOfMass, out Vector3 objectVelocity, out Vector3 objectAngularVelocity)
    {
        XRNodeState throwingControllerState;
        if (!TryGetNodeState(throwingController, out throwingControllerState))
        { 
            objectVelocity = objectAngularVelocity = default(Vector3);
            return false;
        }

        return TryGetThrownObjectVelAngVel(throwingControllerState, thrownObjectCenterOfMass, out objectVelocity, out objectAngularVelocity);
    }

    public static bool TryGetThrownObjectVelAngVel(XRNodeState throwingControllerState, Vector3 thrownObjectCenterOfMass, out Vector3 objectVelocity, out Vector3 objectAngularVelocity)
    {
        Vector3 controllerPos, controllerVelocity, controllerAngularVelocity;

        if (!throwingControllerState.TryGetPosition(out controllerPos) ||
            !throwingControllerState.TryGetVelocity(out controllerVelocity) ||
            !throwingControllerState.TryGetAngularVelocity(out controllerAngularVelocity))
        {
            objectVelocity = objectAngularVelocity = default(Vector3);
            return false;
        }

        GetThrownObjectVelAngVel(controllerPos, controllerVelocity, controllerAngularVelocity, thrownObjectCenterOfMass, out objectVelocity, out objectAngularVelocity);
        return true;
    }

    public static bool TryThrow(this Rigidbody rb, InteractionSourcePose throwingConctoller)
    {
        Vector3 velocity, angularVelocity;
        if (!TryGetThrownObjectVelAngVel(throwingConctoller, rb.transform.TransformPoint(rb.centerOfMass), out velocity, out angularVelocity))
        {
            return false;
        }

        rb.angularVelocity = angularVelocity;
        rb.velocity = velocity;
        rb.isKinematic = false;
        return true;
    }

    public static bool TryThrow(this Rigidbody rb, XRNodeState throwingConctoller)
    {
        Vector3 velocity, angularVelocity;
        if (!TryGetThrownObjectVelAngVel(throwingConctoller, rb.transform.TransformPoint(rb.centerOfMass), out velocity, out angularVelocity))
        {
            return false;
        }

        rb.angularVelocity = angularVelocity;
        rb.velocity = velocity;
        rb.isKinematic = false;
        return true;
    }

    public static bool TryThrow(this Rigidbody rb, XRNode throwingConctoller)
    {
        Vector3 velocity, angularVelocity;
        if (!TryGetThrownObjectVelAngVel(throwingConctoller, rb.transform.TransformPoint(rb.centerOfMass), out velocity, out angularVelocity))
        {
            return false;
        }

        rb.angularVelocity = angularVelocity;
        rb.velocity = velocity;
        rb.isKinematic = false;
        return true;
    }
}
