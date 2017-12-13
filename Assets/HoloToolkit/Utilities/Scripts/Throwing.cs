// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif
#else
using UnityEngine.VR;
#if UNITY_WSA
using UnityEngine.VR.WSA.Input;
#endif
#endif

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

#if UNITY_WSA
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
#endif

#if UNITY_2017_2_OR_NEWER
    public static bool TryGetNodeState(XRNode node, out XRNodeState nodeState)
    {
        List<XRNodeState> nodeStates = new List<XRNodeState>();
#else
    public static bool TryGetNodeState(VRNode node, out VRNodeState nodeState)
    {
        List<VRNodeState> nodeStates = new List<VRNodeState>();
#endif
        InputTracking.GetNodeStates(nodeStates);
        nodeState = nodeStates.Where(p => p.nodeType == node).FirstOrDefault();

        return nodeState.tracked;
    }

#if UNITY_2017_2_OR_NEWER
    public static bool TryGetThrownObjectVelAngVel(XRNode throwingController, Vector3 thrownObjectCenterOfMass, out Vector3 objectVelocity, out Vector3 objectAngularVelocity)
    {
        XRNodeState throwingControllerState;
#else
    public static bool TryGetThrownObjectVelAngVel(VRNode throwingController, Vector3 thrownObjectCenterOfMass, out Vector3 objectVelocity, out Vector3 objectAngularVelocity)
    {
        VRNodeState throwingControllerState;
#endif
        if (!TryGetNodeState(throwingController, out throwingControllerState))
        { 
            objectVelocity = objectAngularVelocity = default(Vector3);
            return false;
        }

        return TryGetThrownObjectVelAngVel(throwingControllerState, thrownObjectCenterOfMass, out objectVelocity, out objectAngularVelocity);
    }

#if UNITY_2017_2_OR_NEWER
    public static bool TryGetThrownObjectVelAngVel(XRNodeState throwingControllerState, Vector3 thrownObjectCenterOfMass, out Vector3 objectVelocity, out Vector3 objectAngularVelocity)
    {
#else
    public static bool TryGetThrownObjectVelAngVel(VRNodeState throwingControllerState, Vector3 thrownObjectCenterOfMass, out Vector3 objectVelocity, out Vector3 objectAngularVelocity)
    {
#endif
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

#if UNITY_WSA
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
#endif

#if UNITY_2017_2_OR_NEWER
    public static bool TryThrow(this Rigidbody rb, XRNodeState throwingConctoller)
    {
#else
    public static bool TryThrow(this Rigidbody rb, VRNodeState throwingConctoller)
    {
#endif
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

#if UNITY_2017_2_OR_NEWER
        public static bool TryThrow(this Rigidbody rb, XRNode throwingConctoller)
    {
#else
    public static bool TryThrow(this Rigidbody rb, VRNode throwingConctoller)
    {
#endif
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
