// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
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
        public static bool TryGetPointerEndpoint<T>(Handedness handedness, InputSourceType inputType, out Vector3 endPoint) where T : IMixedRealityPointer
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
                if (pointer.Controller?.ControllerHandedness.IsMatch(handedness) == true)
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
                    && pointer.Controller?.ControllerHandedness.IsMatch(handedness) == true)
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
                if (pointer.Controller?.ControllerHandedness.IsMatch(handedness) == true
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
            HashSet<IMixedRealityInputSource> inputSources = CoreServices.InputSystem?.DetectedInputSources;

            if (inputSources == null)
            {
                yield break;
            }

            foreach (IMixedRealityInputSource inputSource in inputSources)
            {
                foreach (IMixedRealityPointer pointer in inputSource.Pointers)
                {
                    yield return pointer;
                }
            }
        }

        /// <summary>
        /// Queries input system for the behavior of a given pointer type. See <seealso cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/>.
        /// </summary>
        /// <typeparam name="T">Type of pointer to query</typeparam>
        /// <param name="handedness">Handedness to query</param>
        /// <returns><seealso cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/> for the given pointer type and handedness</returns>
        public static PointerBehavior GetPointerBehavior<T>(Handedness handedness, InputSourceType inputSourceType) where T : class, IMixedRealityPointer
        {
            if (CoreServices.InputSystem?.FocusProvider is IPointerPreferences preferences)
            {
                if (typeof(T) == typeof(GGVPointer))
                {
                    return preferences.GazePointerBehavior;
                }
                return preferences.GetPointerBehavior<T>(handedness, inputSourceType);
            }
            else
            {
                WarnAboutSettingCustomPointerBehaviors();
                return PointerBehavior.Default;
            }
        }

        /// <summary>
        /// Sets the behavior for the hand ray with given handedness
        /// </summary>
        /// <param name="pointerBehavior">Desired <seealso cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/>.</param>
        /// <param name="handedness">Specify handedness to restrict to only right, left hands.</param>
        public static void SetHandRayPointerBehavior(PointerBehavior pointerBehavior, Handedness handedness = Handedness.Any)
        {
            SetPointerBehavior<ShellHandRayPointer>(pointerBehavior, InputSourceType.Hand, handedness);
        }

        /// <summary>
        /// Sets the behavior for the motion controller ray with given handedness
        /// </summary>
        /// <param name="pointerBehavior">Desired <seealso cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/>.</param>
        /// <param name="handedness">Specify handedness to restrict to only right, left.</param>
        public static void SetMotionControllerRayPointerBehavior(PointerBehavior pointerBehavior, Handedness handedness = Handedness.Any)
        {
            SetPointerBehavior<ShellHandRayPointer>(pointerBehavior, InputSourceType.Controller, handedness);
        }

        /// <summary>
        /// Sets the behavior for the grab pointer with given handedness.
        /// </summary>
        /// <param name="pointerBehavior">Desired <seealso cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/>.</param>
        /// <param name="handedness">Specify handedness to restrict to only right, left.</param>
        public static void SetHandGrabPointerBehavior(PointerBehavior pointerBehavior, Handedness handedness = Handedness.Any)
        {
            SetPointerBehavior<SpherePointer>(pointerBehavior, InputSourceType.Hand, handedness);
        }

        /// <summary>
        /// Sets the behavior for the poke pointer with given handedness.
        /// </summary>
        /// <param name="pointerBehavior">Desired <seealso cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/>.</param>
        /// <param name="handedness">Specify handedness to restrict to only right, left.</param>
        public static void SetHandPokePointerBehavior(PointerBehavior pointerBehavior, Handedness handedness = Handedness.Any)
        {
            SetPointerBehavior<PokePointer>(pointerBehavior, InputSourceType.Hand, handedness);
        }

        /// <summary>
        /// Sets the behavior for the gaze pointer.
        /// </summary>
        /// <param name="pointerBehavior">Desired <seealso cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/>.</param>
        public static void SetGazePointerBehavior(PointerBehavior pointerBehavior)
        {
            if (CoreServices.InputSystem.FocusProvider is IPointerPreferences pointerPreferences)
            {
                pointerPreferences.GazePointerBehavior = pointerBehavior;
                foreach (InputSourceType sourceType in Enum.GetValues(typeof(InputSourceType)))
                {
                    pointerPreferences.SetPointerBehavior<GGVPointer>(Handedness.Any, sourceType, pointerBehavior);
                }
            }
            else
            {
                WarnAboutSettingCustomPointerBehaviors();
            }
        }

        /// <summary>
        /// Sets the pointer behavior for pointer of type T, for all input types.
        /// </summary>
        /// <typeparam name="T">All pointer types that equal or derive from this type will be set.</typeparam>
        /// <param name="pointerBehavior">Desired <seealso cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/>.</param>
        /// <param name="handedness">Specify handedness to restrict to only right, left.</param>
        public static void SetPointerBehavior<T>(PointerBehavior pointerBehavior, Handedness handedness = Handedness.Any) where T : class, IMixedRealityPointer
        {
            foreach (InputSourceType type in Enum.GetValues(typeof(InputSourceType)))
            {
                SetPointerBehavior<T>(pointerBehavior, type, handedness);
            }
        }

        /// <summary>
        /// Sets the behavior for the given pointer type and input type.
        /// </summary>
        /// <typeparam name="T">All pointer types that equal or derive from this type will be set.</typeparam>
        /// <param name="pointerBehavior">Desired <seealso cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/>.</param>
        /// <param name="sourceType">Allows setting different behaviors for different input types (hands, controllers, etc.)</param>
        /// <param name="handedness">Specify handedness to restrict to only right, left.</param>
        public static void SetPointerBehavior<T>(PointerBehavior pointerBehavior, InputSourceType sourceType, Handedness handedness = Handedness.Any) where T : class, IMixedRealityPointer
        {
            if (CoreServices.InputSystem.FocusProvider is IPointerPreferences preferences)
            {
                preferences.SetPointerBehavior<T>(handedness, sourceType, pointerBehavior);
            }
            else
            {
                WarnAboutSettingCustomPointerBehaviors();
            }
        }

        private static void WarnAboutSettingCustomPointerBehaviors()
        {
            Debug.LogWarning("Setting custom pointer behaviors only works if the input system is using the default MRTK focus provider. " +
                "Are you using a custom Focus Provider that doesn't implement IPointerPreferences?");
        }

    }
}
