// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// A class with general purpose extensions methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns the absolute duration of the curve from first to last key frame
        /// </summary>
        /// <param name="curve">The animation curve to check duration of.</param>
        /// <returns>Returns 0 if the curve is null or has less than 1 frame, otherwise returns time difference between first and last frame.</returns>
        public static float Duration(this AnimationCurve curve)
        {
            if (curve == null || curve.length <= 1)
            {
                return 0.0f;
            }

            return Mathf.Abs(curve[curve.length - 1].time - curve[0].time);
        }

        /// <summary>
        /// Determines whether or not a ray is valid.
        /// </summary>
        /// <param name="ray">The ray being tested.</param>
        /// <returns>True if the ray is valid, false otherwise.</returns>
        public static bool IsValid(this Ray ray)
        {
            return (ray.direction != Vector3.zero);
        }

        #region GameObject

        /// <summary>
        /// Determines whether or not a game object's layer is included in the specified layer mask.
        /// </summary>
        /// <param name="gameObject">The game object whose layer to test.</param>
        /// <param name="layerMask">The layer mask to test against.</param>
        /// <returns>True if <paramref name="gameObject"/>'s layer is included in <paramref name="layerMask"/>, false otherwise.</returns>
        public static bool IsInLayerMask(this GameObject gameObject, LayerMask layerMask)
        {
            LayerMask gameObjectMask = (1 << gameObject.layer);
            return ((gameObjectMask & layerMask) == gameObjectMask);
        }

        #endregion
    }
}
