// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Utility class to control <see cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/> of pointers.
    /// Hook up buttons to the public functions to turn rays on and off.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/PointerBehaviorControls")]
    public class PointerBehaviorControls : MonoBehaviour
    {
        /// <summary>
        /// Toggles a pointer's "enabled" behavior. If a pointer's <see cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/> is Default or AlwaysOn,
        /// set it to AlwaysOff. Otherwise, set the pointer's behavior to Default.
        /// Will set this state for all matching pointers.
        /// </summary>
        /// <typeparam name="T">Type of pointer to set</typeparam>
        /// <param name="inputType">Input type of pointer to set</param>
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

        /// <summary>
        /// If hand ray <see cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/> is AlwaysOn or Default, set it to off.
        /// Otherwise, set behavior to default
        /// </summary>
        public void ToggleHandRayEnabled()
        {
            TogglePointerEnabled<ShellHandRayPointer>(InputSourceType.Hand);
        }

        /// <summary>
        /// If controller ray <see cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/> is AlwaysOn or Default, set it to off.
        /// Otherwise, set behavior to default
        /// </summary>
        public void ToggleControllerRayEnabled()
        {
            TogglePointerEnabled<ShellHandRayPointer>(InputSourceType.Controller);
        }

        /// <summary>
        /// If hand grab pointer <see cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/> is AlwaysOn or Default, set it to off.
        /// Otherwise, set behavior to default
        /// </summary>
        public void ToggleHandGrabEnabled()
        {
            TogglePointerEnabled<SpherePointer>(InputSourceType.Hand);
        }

        /// <summary>
        /// If finger poke pointer <see cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/> is AlwaysOn or Default, set it to off.
        /// Otherwise, set behavior to default
        /// </summary>
        public void ToggleHandPokeEnabled()
        {
            TogglePointerEnabled<PokePointer>(InputSourceType.Hand);
        }

        /// <summary>
        /// Sets the <see cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/> for all hand rays.
        /// <param name="isEnabled">If true, behavior will be set to Default.
        /// Otherwise it will be set to AlwaysOff</param>
        /// </summary>
        public void SetHandRayEnabled(bool isEnabled)
        {
            PointerUtils.SetHandRayPointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff,
                Handedness.Any);
        }

        /// <summary>
        /// Sets the <see cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/> for all controller rays to be AlwaysOff
        /// <param name="isEnabled">If true, behavior will be set to Default.
        /// Otherwise it will be set to AlwaysOff</param>
        /// </summary>
        public void SetControllerRayEnabled(bool isEnabled)
        {
            PointerUtils.SetMotionControllerRayPointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff,
                Handedness.Any);
        }

        /// <summary>
        /// Sets the <see cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/> for the gaze pointer to be AlwaysOff
        /// <param name="isEnabled">If true, behavior will be set to Default.
        /// Otherwise it will be set to AlwaysOff</param>
        /// </summary>
        public void SetGazeEnabled(bool isEnabled)
        {
            PointerUtils.SetGazePointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff);
        }

        /// <summary>
        /// Sets the <see cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/> for the grab pointer to be AlwaysOff
        /// <param name="isEnabled">If true, behavior will be set to Default.
        /// Otherwise it will be set to AlwaysOff</param>
        /// </summary>
        public void SetGrabEnabled(bool isEnabled)
        {
            PointerUtils.SetHandGrabPointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff, Handedness.Any);
        }

        /// <summary>
        /// Sets the <see cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/> for the poke pointer to be AlwaysOff
        /// <param name="isEnabled">If true, behavior will be set to Default.
        /// Otherwise it will be set to AlwaysOff</param>
        /// </summary>
        public void SetPokeEnabled(bool isEnabled)
        {
            PointerUtils.SetHandPokePointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff, Handedness.Any);
        }

        /// <summary>
        /// Sets pointer states to mimic traditional vr behavior.
        /// PokePointer will be off
        /// GrabPointer will be off
        /// HandRayPointer will be off
        /// MotionControllerRayPointer will be Default
        /// GazePointef will be off
        /// </summary>
        public void SetVR()
        {
            PointerUtils.SetHandPokePointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetHandGrabPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetMotionControllerRayPointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);
        }

        /// <summary>
        /// Sets pointer states to turn off all but the poke pointer
        /// </summary>
        public void SetFingerOnly()
        {
            PointerUtils.SetHandPokePointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetHandGrabPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetMotionControllerRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);
        }

        /// <summary>
        /// Sets pointer behavior to mimic HoloLens 1 interactions, useful
        /// for using HoloLens 1 interactions on HoloLens 2.
        /// PokePointer will be off
        /// GrabPointer will be off
        /// HandRayPointer will be off
        /// MotionControllerRayPointer will be off
        /// GazePointer will be Default
        /// </summary>
        public void SetHoloLens1()
        {
            PointerUtils.SetHandPokePointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetHandGrabPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetMotionControllerRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetGazePointerBehavior(PointerBehavior.Default);
        }

        /// <summary>
        /// Sets pointer behavior to mimic HoloLens 2
        /// PokePointer will be Default
        /// GrabPointer will be Default
        /// HandRayPointer will be Default
        /// MotionControllerRayPointer will be off
        /// GazePointer will be Off
        /// </summary>
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
