// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public static class PointerUtils
    {

        /// <summary>
        /// Tries to get the end point of a hand ray
        /// If no hand ray of given handedness is found, returns false and sets result to zero.
        /// </summary>
        /// <param name="handedness"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static bool TryGetHandRayEndPoint(Handedness handedness, out Vector3 endPoint)
        {
            return TryGetPointerEndpoint<LinePointer>(handedness, out endPoint);
        }

        /// <summary>
        /// Tries to get the end point of a line pointer
        /// If no pointer of given handedness is found, returns false and sets result to zero.
        /// </summary>
        /// <param name="handedness"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static bool TryGetLinePointerEndPoint(Handedness handedness, out Vector3 endPoint)
        {
            return TryGetPointerEndpoint<LinePointer>(handedness, out endPoint);
        }

        /// <summary>
        /// Tries to get the end point of a pointer of a given type and handedness.
        /// If no pointer of given handedness is found, returns false and sets result to zero.
        /// </summary>
        /// <typeparam name="T">Type of pointer to query</typeparam>
        /// <param name="handedness">Handedness of pointer</param>
        /// <param name="endPoint">The output point position</param>
        /// <returns>true if pointer found, false otherwise. If not found, endPoint is set to zero</returns>
        public static bool TryGetPointerEndpoint<T>(Handedness handedness, out Vector3 endPoint) where T : class, IMixedRealityPointer
        {
            T pointer = FindPointer<T>(handedness);
            FocusDetails? details = pointer?.Result?.Details;
            if (!details.HasValue)
            {
                endPoint = Vector3.zero;
                return false;
            }
            endPoint = details.Value.Point;
            return true;
        }

        /// <summary>
        /// Find the first detected line pointer with matching handedness.
        /// </summary>
        /// <remarks>
        /// The given handedness should be either Handedness.Left or Handedness.Right.
        /// </remarks>
        public static IMixedRealityPointer FindLinePointer(Handedness handedness)
        {
            return FindPointer<LinePointer>(handedness);
        }

        /// <summary>
        /// Find the first detected pointer of the given type with matching handedness.
        /// </summary>
        public static T FindPointer<T>(Handedness handedness) where T : class, IMixedRealityPointer
        {
            foreach (var pointer in CoreServices.InputSystem.FocusProvider.GetPointers<T>())
            {
                if ((pointer.Controller?.ControllerHandedness & handedness) != 0)
                {
                    return pointer;
                }
            }
            return null;
        }
    }

}