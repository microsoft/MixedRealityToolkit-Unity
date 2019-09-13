// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Experimental.XR;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public static class RayUtilities
    {
        /// <summary>
        /// Gets the <seealso cref="UnityEngine.Ray"/> representing the position and direction of the user's head.
        /// </summary>
        /// <returns>The <seealso cref="UnityEngine.Ray"/> for the pointer</param>
        public static Ray GetHeadGazeRay()
        {
            return new Ray(CameraCache.Main.transform.position, CameraCache.Main.transform.forward);
        }

        /// <summary>
        /// Gets the <seealso cref="UnityEngine.Ray"/> representing the position and direction of the user's eyes.
        /// </summary>
        /// <param name="ray">The <seealso cref="UnityEngine.Ray"/> for the pointer</param>
        /// <returns>True if the ray contains valid data, false otherwise.</param>
        public static bool TryGetEyeGazeRay(out Ray ray)
        {
            ray = new Ray();

            IMixedRealityEyeGazeProvider eyeGazeProvider = CoreServices.InputSystem?.EyeGazeProvider;
            if (eyeGazeProvider == null) { return false; }

            ray.origin = eyeGazeProvider.GazeOrigin;
            ray.direction = eyeGazeProvider.GazeDirection;
            return true;
        }

        ///// <summary>
        ///// Gets the <seealso cref="UnityEngine.Ray"/> based on the interaction enabled pointer assosiciated with the user's gaze
        ///// and reports the type of input (head or eyes) of the active gaze.
        ///// </summary>
        ///// <param name="ray">The <seealso cref="UnityEngine.Ray"/> for the pointer</param>
        ///// <param name="sourceType">The type of input source</param>
        ///// <returns>
        ///// True if the ray contains valid data, false otherwise.
        ///// </returns>
        //public static bool TryGetGazeRay(out Ray ray, out InputSourceType sourceType)
        //{
        //    ray = new Ray();
        //    sourceType = InputSourceType.Other;

        //    // First, try to get the eye gaze ray
        //    if (TryGetEyeGazeRay(out ray))
        //    {
        //        sourceType = InputSourceType.Eyes;
        //        return true;
        //    }

        //    // Then try to get the head gaze ray
        //    if (TryGetHeadGazeRay(out ray))
        //    {
        //        sourceType = InputSourceType.Head;
        //        return true;
        //    }

        //    return false;
        //}

        /// <summary>
        /// Gets the <seealso cref="UnityEngine.Ray"/> based on the active pointer assosiciated with the user's hand.
        /// </summary>
        /// <param name="hand">The handedness of the hand</param>
        /// <param name="ray">The <seealso cref="UnityEngine.Ray"/> for the pointer</param>
        /// <returns>
        /// True if the ray contains valid data, false otherwise.
        /// </returns>
        public static bool TryGetHandRay(Handedness hand, out Ray ray)
        {
            // todo
            ray = new Ray();
            return false;
        }

        /// <summary>
        /// Gets the <seealso cref="UnityEngine.Ray"/> based on the active pointer assosiciated with the
        /// motion controller.
        /// </summary>
        /// <param name="hand">The handedness of the motion controller</param>
        /// <param name="ray">The <seealso cref="UnityEngine.Ray"/> for the pointer</param>
        /// <returns>
        /// True if the ray contains valid data, false otherwise.
        /// </returns>
        public static bool TryGetMotionControllerRay(Handedness hand, out Ray ray)
        {
            // todo
            ray = new Ray();
            return false;
        }

    ///// <summary>
    ///// Gets the <seealso cref="UnityEngine.Ray"/> based on the active pointer assosiciated with the user's hand
    ///// or a motion controller.
    ///// </summary>
    ///// <param name="hand">The handedness of the user's hand or motion controller</param>
    ///// <param name="ray">The <seealso cref="UnityEngine.Ray"/> for the pointer</param>
    ///// <param name="sourceType">The type of input source</param>
    ///// <returns>
    ///// True if the ray contains valid data, false otherwise.
    ///// </returns>
    //public static bool TryGetHandOrControllerRay(Handedness hand, out Ray ray, out InputSourceType sourceType)
    //{
    //    ray = new Ray();
    //    sourceType = InputSourceType.Other;

    //    // First, try to get the hand ray
    //    if (TryGetHandRay(hand, out ray))
    //    {
    //        sourceType = InputSourceType.Hand;
    //        return true;
    //    }

    //    // Then try to get the controller ray
    //    if (TryGetMotionControllerRay(hand, out ray))
    //    {
    //        sourceType = InputSourceType.Controller;
    //        return true;
    //    }

    //    return false;
    //}

    ///// <summary>
    ///// Gets the<seealso cref="UnityEngine.Ray"/> based on the active pointer assosiciated with the
    ///// desired input source type.
    ///// </summary>
    ///// <param name="sourceType">The type of input source</param>
    ///// <param name="hand">The handedness of the input source</param>
    ///// <param name="ray">The <seealso cref="UnityEngine.Ray"/> for the pointer</param>
    ///// <returns>
    ///// True if the ray contains valid data, false otherwise.
    ///// </returns>
    //public static bool TryGetRay(InputSourceType sourceType, Handedness hand, out Ray ray)
    //{
    //    ray = new Ray();

    //    //foreach (IMixedRealityPointer pointer in PointerUtils.GetPointers<IMixedRealityPointer>(hand, sourceType))
    //    //{
    //    //    if (pointer.IsInteractionEnabled)
    //    //    {
    //    //        ray.origin = pointer.Position;
    //    //        ray.direction = GetDirection(pointer.Rotation);
    //    //        return true;
    //    //    }
    //    //}

    //    return false;
    //}

    public static bool TryGetRay(InputSourceType sourceType, Handedness hand, out Ray ray)
        {
            bool success = false;

            switch (sourceType)
            {
                case InputSourceType.Head:
                    // The head does not have a handedness, so we ignore the hand parameter.
                    ray = GetHeadGazeRay();
                    success = true;
                    break;

                case InputSourceType.Eyes:
                    // The eyes do not have a handedness, so we ignore the hand parameter.
                    success = TryGetEyeGazeRay(out ray);
                    break;

                case InputSourceType.Hand:
                    success = TryGetHandRay(hand, out ray);
                    break;

                case InputSourceType.Controller:
                    success = TryGetMotionControllerRay(hand, out ray);
                    break;

                default:
                    Debug.Log($"It is not supported to get the ray for {sourceType} sources.");
                    ray = new Ray();
                    success = false;
                    break;
            }

            return success;
        }

        /// <summary>
        /// Calculates the direction vector from a rotation.
        /// </summary>
        /// <param name="rotation">Quaternion representing the rotation of the object.</param>
        /// <returns>
        /// Normalized Vector3 representing the direction vector.
        /// </returns>
        public static Vector3 GetDirection(Quaternion rotation)
        {
            return (rotation * Vector3.forward).normalized;
        }
    }
}
