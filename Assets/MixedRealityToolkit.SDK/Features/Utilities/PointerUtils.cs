// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public static class PointerUtils
    {
        /// <summary>
        /// Tries to get the end point of a hand ray.
        /// If no hand ray of given handedness is found, returns false and sets result to zero.
        /// </summary>
        /// <param name="handedness">Handedness of ray</param>
        /// <param name="endPoint">The output position</param>
        /// <returns>True if pointer found, false otherwise. If not found, endPoint is set to zero</returns>
        public static bool TryGetHandRayEndPoint(Handedness handedness, out Vector3 endPoint)
        {
            return TryGetPointerEndpoint<LinePointer>(handedness, InputSourceType.Hand, out endPoint);
        }

        /// <summary>
        /// Tries to get the end point of a motion controller.
        /// If no pointer of given handedness is found, returns false and sets result to zero.
        /// </summary>
        /// <param name="handedness">Handedness of ray</param>
        /// <param name="endPoint">The output position</param>
        /// <returns>True if pointer found, false otherwise. If not found, endPoint is set to zero</returns>
        public static bool TryGetMotionControllerEndPoint(Handedness handedness, out Vector3 endPoint)
        {
            return TryGetPointerEndpoint<LinePointer>(handedness, InputSourceType.Controller, out endPoint);
        }

        /// <summary>
        /// Tries to get the end point of a pointer by source type and handedness.
        /// If no pointer of given handedness is found, returns false and sets result to zero.
        /// </summary>
        /// <typeparam name="T">Type of pointer to query</typeparam>
        /// <param name="handedness">Handedness of pointer</param>
        /// <param name="inputType">Input type of pointer</param>
        /// <param name="endPoint">Output point position</param>
        /// <returns>True if pointer found, false otherwise. If not found, endPoint is set to zero</returns>
        public static bool TryGetPointerEndpoint<T>(Handedness handedness, InputSourceType inputType, out Vector3 endPoint) where T: IMixedRealityPointer
        {
            foreach (var pointer in GetPointers<IMixedRealityPointer>(handedness, inputType))
            {
                FocusDetails? details = pointer?.Result?.Details;
                if (details.HasValue)
                {
                    endPoint = details.Value.Point;
                    return true;
                }
            }
            endPoint = Vector3.zero;
            return false;
        }

        /// <summary>
        /// Tries to get the end point of a pointer of a pointer type and handedness.
        /// If no pointer of given handedness is found, returns false and sets result to zero.
        /// </summary>
        /// <typeparam name="T">Type of pointer to query</typeparam>
        /// <param name="handedness">Handedness of pointer</param>
        /// <param name="endPoint">The output point position</param>
        /// <returns>True if pointer found, false otherwise. If not found, endPoint is set to zero</returns>
        public static bool TryGetPointerEndpoint<T>(Handedness handedness, out Vector3 endPoint) where T : class, IMixedRealityPointer
        {
            T pointer = GetPointer<T>(handedness);
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
        /// Find the first detected pointer of the given type with matching handedness.
        /// </summary>
        public static T GetPointer<T>(Handedness handedness) where T : class, IMixedRealityPointer
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

        /// <summary>
        /// Returns iterator over all pointers of specific type, with specific handedness.
        /// </summary>
        /// <typeparam name="T">Return only pointers with this input type</typeparam>
        /// <param name="handedness">Handedness of pointer</param>
        /// <returns>Iterator over all pointers of specific type, with specific handedness</returns>
        public static IEnumerable<T> GetPointers<T>(Handedness handedness = Handedness.Any) where T : IMixedRealityPointer
        {
            foreach (var pointer in GetPointers())
            {
                if (pointer is T pointerConcrete
                    && (pointer.Controller?.ControllerHandedness & handedness) != 0)
                {
                    yield return pointerConcrete;
                }
            }
        }

        /// <summary>
        /// Returns all pointers with given handedness and input type.
        /// </summary>
        /// <param name="handedness">Handedness of pointer</param>
        /// <param name="sourceType">Only return pointers of this input source type</param>
        /// <returns>Iterator over all pointers that match the source type, with specific handedness</returns>
        public static IEnumerable<T> GetPointers<T>(Handedness handedness, InputSourceType sourceType) where T : IMixedRealityPointer
        {
            foreach (var pointer in GetPointers<T>(handedness))
            {
                if ((pointer.Controller?.ControllerHandedness & handedness) != 0
                    && pointer.InputSourceParent.SourceType == sourceType)
                {
                    yield return pointer;
                }
            }
        }

        /// <summary>
        /// Iterate over all pointers in the input system. May contain duplicates.
        /// </summary>
        public static IEnumerable<IMixedRealityPointer> GetPointers()
        {
            foreach (var inputSource in CoreServices.InputSystem.DetectedInputSources)
            {
                foreach (var pointer in inputSource.Pointers)
                {
                    yield return pointer;
                }
            }
        }

        /// <summary>
        /// Gets the physics ray based on the interaction enabled pointer assosiciated with the head gaze.
        /// </summary>
        /// <param name="ray">The physics ray for the pointer</param>
        /// <returns>
        /// True if the ray contains valid data, false otherwise.
        /// </returns>
        public static bool TryGetHeadGazeRay(out Ray ray)
        {
            return TryGetRay(InputSourceType.Head, Handedness.None, out ray);
        }

        /// <summary>
        /// Gets the physics ray based on the interaction enabled pointer assosiciated with the eye gaze.
        /// </summary>
        /// <param name="ray">The physics ray for the pointer</param>
        /// <returns>
        /// True if the ray contains valid data, false otherwise.
        /// </returns>
        public static bool TryGetEyeGazeRay(out Ray ray)
        {
            return TryGetRay(InputSourceType.Eyes, Handedness.None, out ray);
        }

        /// <summary>
        /// Gets the physics ray based on the interaction enabled pointer assosiciated with the user's gaze
        /// and reports the type of input (head or eyes) of the active gaze.
        /// </summary>
        /// <param name="ray">The physics ray for the pointer</param>
        /// <param name="sourceType">The type of input source</param>
        /// <returns>
        /// True if the ray contains valid data, false otherwise.
        /// </returns>
        public static bool TryGetGazeRay(out Ray ray, out InputSourceType sourceType)
        {
            ray = new Ray();
            sourceType = InputSourceType.Other;

            // First, try to get the eye gaze ray
            if (TryGetEyeGazeRay(out ray))
            {
                sourceType = InputSourceType.Eyes;
                return true;
            }

            // Then try to get the head gaze ray
            if (TryGetHeadGazeRay(out ray))
            {
                sourceType = InputSourceType.Head;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the physics ray based on the interaction enabled pointer assosiciated with the user's hand.
        /// </summary>
        /// <param name="hand">The <see cref="Handedness"/> of the hand</param>
        /// <param name="ray">The physics ray for the pointer</param>
        /// <returns>
        /// True if the ray contains valid data, false otherwise.
        /// </returns>
        public static bool TryGetHandRay(Handedness hand, out Ray ray)
        {
            return TryGetRay(InputSourceType.Hand, hand, out ray);
        }

        /// <summary>
        /// Gets the physics ray based on the interaction enabled pointer assosiciated with the
        /// motion controller.
        /// </summary>
        /// <param name="hand">The <see cref="Handedness"/> of the motion controller</param>
        /// <param name="ray">The physics ray for the pointer</param>
        /// <returns>
        /// True if the ray contains valid data, false otherwise.
        /// </returns>
        public static bool TryGetMotionControllerRay(Handedness hand, out Ray ray)

        {
            return TryGetRay(InputSourceType.Controller, hand, out ray);
        }

        /// <summary>
        /// Gets the physics ray based on the interaction enabled pointer assosiciated with the user's hand
        /// or a motion controller.
        /// </summary>
        /// <param name="hand">The <see cref="Handedness"/> of the user's hand or motion controller</param>
        /// <param name="ray">The physics ray for the pointer</param>
        /// <param name="sourceType">The type of input source</param>
        /// <returns></returns>
        public static bool TryGetHandOrControllerRay(Handedness hand, out Ray ray, out InputSourceType sourceType)
        {
            ray = new Ray();
            sourceType = InputSourceType.Other;

            // First, try to get the hand ray
            if (TryGetHandRay(hand, out ray))
            {
                sourceType = InputSourceType.Hand;
                return true;
            }

            // Then try to get the controller ray
            if (TryGetMotionControllerRay(hand, out ray))
            {
                sourceType = InputSourceType.Controller;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the physics ray based on the interaction enabled pointer assosiciated with the
        /// desired input source type.
        /// </summary>
        /// <param name="sourceType">The type of input source</param>
        /// <param name="hand">The <see cref="Handedness"/> of the input source</param>
        /// <param name="ray">The physics ray for the pointer</param>
        /// <returns>
        /// True if the ray contains valid data, false otherwise.
        /// </returns>
        public static bool TryGetRay(InputSourceType sourceType, Handedness hand, out Ray ray)
        {
            ray = new Ray();

            foreach (IMixedRealityPointer pointer in PointerUtils.GetPointers<IMixedRealityPointer>(hand, sourceType))
            {
                if (pointer.IsInteractionEnabled)
                {
                    ray.origin = pointer.Position;
                    ray.direction = GetDirection(pointer.Rotation);
                    return true;
                }
            }

            return false;
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