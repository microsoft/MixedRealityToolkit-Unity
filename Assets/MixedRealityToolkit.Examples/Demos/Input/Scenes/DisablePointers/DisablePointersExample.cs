using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Reflection;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This example demonstrates how to turn pointers on and off by 
    /// specifying custom behaviors.
    /// </summary>
    [RequireComponent(typeof(PointerBehaviorControls))]
    [AddComponentMenu("Scripts/MRTK/Examples/DisablePointersExample")]
    public class DisablePointersExample : MonoBehaviour
    {
        public Interactable GazeToggle;
        public Interactable GrabToggle;
        public Interactable PokeToggle;
        public Interactable HandRayToggle;
        public Interactable ControllerRayToggle;
        void Start()
        {
            ResetExample();
        }

        public void ResetExample()
        {
            var pointerControl = GetComponent<PointerBehaviorControls>();
            IMixedRealityCapabilityCheck capabilityChecker = CoreServices.InputSystem as IMixedRealityCapabilityCheck;
            if (capabilityChecker != null)
            {
                if (capabilityChecker.CheckCapability(MixedRealityCapability.ArticulatedHand))
                {
                    pointerControl.SetHoloLens2();
                }
                else if (capabilityChecker.CheckCapability(MixedRealityCapability.MotionController))
                {
                    pointerControl.SetVR();
                }
                else
                {
                    pointerControl.SetHoloLens1();
                }
            }
            else
            {
                Debug.LogWarning("Input system does not implement IMixedRealityCapabilityCheck, not setting to any preset interaction");
            }
        }

        void Update()
        {
            SetToggleHelper<ShellHandRayPointer>(HandRayToggle, "HandRayToggle", InputSourceType.Hand);
            SetToggleHelper<ShellHandRayPointer>(ControllerRayToggle, "ControllerRayToggle", InputSourceType.Controller);
            SetToggleHelper<SpherePointer>(GrabToggle, "GrabToggle", InputSourceType.Hand);
            SetToggleHelper<PokePointer>(PokeToggle, "PokeToggle", InputSourceType.Hand);
            SetToggleHelper<GGVPointer>(GazeToggle, "GazeToggle", InputSourceType.Hand);
        }

        private void SetToggleHelper<T>(Interactable toggle, string toggleName, InputSourceType inputType) where T : class, IMixedRealityPointer
        {
            if (toggle == null)
            {
                Debug.LogWarning($"Button {toggleName} is null on GameObject {gameObject.name}. Did you forget to set it?");
            }
            else
            {
                toggle.IsToggled = PointerUtils.GetPointerBehavior<T>(Handedness.Any, inputType) != PointerBehavior.AlwaysOff;
            }
        }
    }

}