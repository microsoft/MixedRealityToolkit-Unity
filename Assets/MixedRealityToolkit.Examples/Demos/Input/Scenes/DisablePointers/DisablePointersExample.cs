using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This example demonstrates how to turn pointers on and off by 
    /// specifying custom behaviors.
    /// </summary>
    public class DisablePointersExample : MonoBehaviour
    {
        public Interactable RayToggle;
        public Interactable GrabToggle;
        public Interactable GazeToggle;
        public Interactable PokeToggle;

        public void SetRayEnabled(bool isEnabled)
        {
            PointerUtils.SetHandRayPointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.AlwaysOff,
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
            PointerUtils.SetMotionControllerRayPointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetGazePointerBehavior(PointerBehavior.Default);
        }

        public void SetFingerOnly()
        {
            PointerUtils.SetHandPokePointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetHandGrabPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);
        }

        public void SetHoloLens1()
        {
            PointerUtils.SetHandPokePointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetHandGrabPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
            PointerUtils.SetGazePointerBehavior(PointerBehavior.Default);
        }

        public void SetHoloLens2()
        {
            PointerUtils.SetHandPokePointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetHandGrabPointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetHandRayPointerBehavior(PointerBehavior.Default, Handedness.Any);
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
            SetToggleHelper<LinePointer>(RayToggle, "RayToggle");
            SetToggleHelper<SpherePointer>(GrabToggle, "GrabToggle");
            SetToggleHelper<PokePointer>(PokeToggle, "PokeToggle");
            SetToggleHelper<GGVPointer>(GazeToggle, "GazeToggle");
        }

        private void SetToggleHelper<T>(Interactable toggle, string toggleName) where T : class, IMixedRealityPointer
        {
            if (toggle == null)
            {
                Debug.LogWarning($"Button {toggleName} is null on gameobject {gameObject.name}. Did you forget to set it?");
            }
            else
            {
                toggle.SetToggled(PointerUtils.GetPointerBehavior<T>(Handedness.Any, InputSourceType.Hand) != PointerBehavior.AlwaysOff);
            }
        }

    }

}