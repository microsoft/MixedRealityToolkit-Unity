// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

        [Tooltip("Used to designate this interaction grabbable as a bounds handle")]
        public bool IsBoundsHandles = false;

        void OnEnable()
        {
            // As of https://docs.unity3d.com/ScriptReference/Physics.ClosestPoint.html
            // ClosestPoint call will only work on specific types of colliders.
            // Using incorrect type of collider will emit warning from FocusProvider, 
            // but grab behavior will be broken at this point.
            // Emit exception on initialization, when we know grab interaction is used 
            // on this object to make an error clearly visible.

            // Note that there can be multiple colliders on an object - as long as one
            // of them are of the valid type, this object will work with NearInteractionGrabbable
            Collider[] colliders = gameObject.GetComponents<Collider>();
            bool containsValidCollider = false;
            for (int i = 0; i < colliders.Length && !containsValidCollider; i++)
            {
                Collider collider = colliders[i];
                containsValidCollider =
                    (collider is BoxCollider) ||
                    (collider is CapsuleCollider) ||
                    (collider is SphereCollider) ||
                    (collider is MeshCollider && (collider as MeshCollider).convex);
            }

            if (!containsValidCollider)
            {
                Debug.LogError("NearInteractionGrabbable requires a " +
                    "BoxCollider, SphereCollider, CapsuleCollider or a convex MeshCollider on an object. " +
                    "Otherwise grab interaction will not work correctly.");
            }
        }
    }
}