// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Utility class to control <see cref="PointerBehavior"/> of pointers.
    /// Hook up buttons to the public functions to turn rays on and off.
    /// </summary>
    public class PointerBehaviorControls : MonoBehaviour
    {
        public void TogglePointerEnabled<T>(InputSourceType inputType) where T : class, IMixedRealityPointer
        {
            PointerBehavior oldBehavior = PointerUtils.GetPointerBehavior<T>(Handedness.Any, inputType);
            PointerBehavior newBehavior;
            if (oldBehavior == PointerBehavior.AlwaysOff)
            {
                newBehavior = PointerBehavior.Default;
            }
            else
            {
                newBehavior = PointerBehavior.AlwaysOff;
            }
            PointerUtils.SetPointerBehavior<T>(newBehavior, inputType);
        }

        public void ToggleHandRayEnabled()
        {
            TogglePointerEnabled<ShellHandRayPointer>(InputSourceType.Hand);
        }

        public void ToggleControllerRayEnabled()
        {
            TogglePointerEnabled<ShellHandRayPointer>(InputSourceType.Controller);
        }

        public void ToggleHandGrabEnabled()
        {
            TogglePointerEnabled<SpherePointer>(InputSourceType.Hand);
        }

        public void ToggleHandPokeEnabled()
        {
            TogglePointerEnabled<PokePointer>(InputSourceType.Hand);
        }

        public void SetHandRayEnabled(bool isEnabled)
        {
            PointerUtils.SetHandRayPointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff,
                Handedness.Any);
        }

        public void SetRightHandRayEnabled(bool isEnabled)
        {
            PointerUtils.SetHandRayPointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff,
                Handedness.Right);
        }

        public void SetLeftHandRayEnabled(bool isEnabled)
        {
            PointerUtils.SetHandRayPointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff,
                Handedness.Left);
        }

        public void SetControllerRayEnabled(bool isEnabled)
        {
            PointerUtils.SetMotionControllerRayPointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff,
                Handedness.Any);
        }

        public void SetGazeEnabled(bool isEnabled)
        {
            PointerUtils.SetGazePointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff);
        }

        public void SetGrabEnabled(bool isEnabled)
        {
            PointerUtils.SetHandGrabPointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff, Handedness.Any);
        }

        public void SetPokeEnabled(bool isEnabled)
        {
            PointerUtils.SetHandPokePointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff, Handedness.Any);
        }

        public void SetVR()
        {
            PointerUtils.SetHandPokePointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetHandGrabPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetMotionControllerRayPointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);
        }

        public void SetFingerOnly()
        {
            PointerUtils.SetHandPokePointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetHandGrabPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetMotionControllerRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);
        }

        public void SetHoloLens1()
        {
            PointerUtils.SetHandPokePointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetHandGrabPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetMotionControllerRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetGazePointerBehavior(PointerBehavior.Default);
        }

        public void SetHoloLens2()
        {
            PointerUtils.SetHandPokePointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetHandGrabPointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetHandRayPointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetMotionControllerRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);
        }
    }
}
