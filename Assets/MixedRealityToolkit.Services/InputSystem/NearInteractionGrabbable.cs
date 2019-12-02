// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Add a NearInteractionGrabbable component to any GameObject that has a collidable
    /// on it in order to make that collidable near grabbable.
    /// 
    /// Any IMixedRealityNearPointer will then dispatch pointer events
    /// to the closest near grabbable objects.
    ///
    /// Additionally, the near pointer will send focus enter and exit events when the 
    /// decorated object is the closest object to the near pointer
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Services/NearInteractionGrabbable")]
    public class NearInteractionGrabbable : MonoBehaviour
    {
        [Tooltip("Check to show a tether from the position where object was grabbed to the hand when manipulating. Useful for things like bounding boxes where resizing/rotating might be constrained.")]
        public bool ShowTetherWhenManipulating = false;

        void OnEnable()
        {
            // As of https://docs.unity3d.com/ScriptReference/Physics.ClosestPoint.html
            // ClosestPoint call will only work on specific types of colliders.
            // Using incorrect type of collider will emit warning from FocusProvider, 
            // but grab behavior will be broken at this point.
            // Emit exception on initialization, when we know grab interaction is used 
            // on this object to make an error clearly visible.

            var collider = gameObject.GetComponent<Collider>();

            if((collider as BoxCollider) == null && 
                (collider as CapsuleCollider) == null &&
                (collider as SphereCollider) == null &&
                ((collider as MeshCollider) == null || (collider as MeshCollider).convex == false))
            {
                Debug.LogException(new InvalidOperationException("NearInteractionGrabbable requires a " +
                    "BoxCollider, SphereCollider, CapsuleCollider or a convex MeshCollider on an object. " +
                    "Otherwise grab interaction will not work correctly."));
            }
        }
    }
}