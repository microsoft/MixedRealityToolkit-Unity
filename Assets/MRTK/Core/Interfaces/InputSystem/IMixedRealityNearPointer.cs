// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public interface IMixedRealityNearPointer : IMixedRealityPointer
    {
        /// <summary>
        /// Returns true if the hand is near anything that's grabbable
        /// Currently performs a sphere cast in the direction of the hand ray.
        /// Currently anything that has a collider is considered "Grabbable"
        /// Eventually we need to filter based on things that can respond
        /// to grab events.
        /// </summary>
        bool IsNearObject { get; }

        /// <summary>
        /// For near pointer we may want to draw a tether between the pointer
        /// and the object.
        /// 
        /// The visual grasp point (average of index and thumb) may actually be different from the pointer
        /// position (the palm).
        ///
        /// This method provides a mechanism to get the visual grasp point.
        ///
        /// NOTE: Not all near pointers have a grasp point (for example a poke pointer).
        /// </summary>
        /// <param name="position">Out parameter filled with the grasp position if available, otherwise <see href="https://docs.unity3d.com/ScriptReference/Vector3-zero.html">Vector3.zero</see>.</param>
        /// <returns>True if a grasp point was retrieved, false if not.</returns>
        bool TryGetNearGraspPoint(out Vector3 position);


        /// <summary>
        /// Near pointers often interact with surfaces.
        /// 
        /// This method provides a mechanism to get the distance to the closest surface the near pointer is interacting with.
        /// 
        /// </summary>
        /// <param name="distance">Out parameter filled with the distance along the surface normal from the surface to the pointer if available, otherwise 0.0f.</param>
        /// <returns>True if a distance was retrieved, false if not.</returns>
        bool TryGetDistanceToNearestSurface(out float distance);

        /// <summary>
        /// Near pointers often interact with surfaces.
        /// 
        /// This method provides a mechanism to get the normal of the closest surface the near pointer is interacting with.
        /// 
        /// </summary>
        /// <param name="normal">Out parameter filled with the surface normal if available, otherwise <see href="https://docs.unity3d.com/ScriptReference/Vector3-zero.html">Vector3.zero</see>.</param>
        /// <returns>True if a normal was retrieved, false if not.</returns>
        bool TryGetNormalToNearestSurface(out Vector3 normal);
    }
}