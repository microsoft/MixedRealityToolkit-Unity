using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This example demonstrates how to turn pointers on and off by 
    /// specifying custom behaviors.
    /// </summary>
    public class TurnPointersOnOff : MonoBehaviour
    {
        public Interactable RayToggle;
        public Interactable GrabToggle;
        public Interactable GazeToggle;
        public Interactable PokeToggle;

        public void SetRayEnabled(bool isEnabled)
        {
            PointerUtils.SetRayPointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff,
                Handedness.Any);
        }

        public void SetGazeEnabled(bool isEnabled)
        {
            PointerUtils.SetGazePointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff);
        }

        public void SetGrabEnabled(bool isEnabled)
        {
            PointerUtils.SetGrabPointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff, Handedness.Any);
        }

        public void SetPokeEnabled(bool isEnabled)
        {
            PointerUtils.SetPokePointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff, Handedness.Any);
        }

        public void SetVR()
        {
            PointerUtils.SetPokePointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetGrabPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetRayPointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetGazePointerBehavior(PointerBehavior.Default);
        }

        public void SetFingerOnly()
        {
            PointerUtils.SetPokePointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetGrabPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);
        }

        public void SetHoloLens1()
        {
            PointerUtils.SetPokePointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetGrabPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetGazePointerBehavior(PointerBehavior.Default);
        }

        public void SetHoloLens2()
        {
            PointerUtils.SetPokePointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetGrabPointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetRayPointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);
        }


        void Start()
        {
            ResetExample();
        }

        public void ResetExample()
        {
            IMixedRealityCapabilityCheck capabilityChecker = CoreServices.InputSystem as IMixedRealityCapabilityCheck;
            if (capabilityChecker != null)
            {
                if (capabilityChecker.CheckCapability(MixedRealityCapability.ArticulatedHand))
                {
                    SetHoloLens2();
                }
                else if (capabilityChecker.CheckCapability(MixedRealityCapability.MotionController))
                {
                    SetVR();
                }
                else
                {
                    SetHoloLens1();
                }
            }
            else
            {
                Debug.LogWarning("Input system does not implement IMixedRealityCapabilityCheck, not setting to any preset interaction");
            }
        }

        void Update()
        {
            SetToggleHelper(RayToggle, "RayToggle", typeof(LinePointer));
            SetToggleHelper(GrabToggle, "GrabToggle", typeof(SpherePointer));
            SetToggleHelper(PokeToggle, "PokeToggle", typeof(PokePointer));
            SetToggleHelper(GazeToggle, "GazeToggle", typeof(GGVPointer));
        }

        private void SetToggleHelper(Interactable toggle, string toggleName, Type toggleType)
        {
            if (toggle == null)
            {
                Debug.LogWarning($"Button {toggleName} is null on gameobject {gameObject.name}. Did you forget to set it?");
            }
            else
            {
                toggle.SetToggled(PointerUtils.GetPointerBehavior(toggleType, Handedness.Any) != PointerBehavior.AlwaysOff);
            }
        }

    }

}