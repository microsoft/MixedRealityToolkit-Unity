using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UX;
using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    [RequireComponent(typeof(InteractorBehaviorControls))]
    [AddComponentMenu("MRTK/Examples/Disable Pointers")]
    public class DisablePointers : MonoBehaviour
    {
        public PressableButton GazeToggle;
        public PressableButton GrabToggle;
        public PressableButton PokeToggle;
        public PressableButton HandRayToggle;
        public PressableButton ControllerRayToggle;

        public PressableButton VRToggle;
        public PressableButton HololensToggle;

        InteractorBehaviorControls interactorBehaviorControls;

        private void Awake()
        {
            SetupSpeechCommand();
            interactorBehaviorControls = GetComponent<InteractorBehaviorControls>();
        }

        private void Start()
        {
            ResetExample();
        }

        private void OnEnable()
        {
            interactorBehaviorControls.onControllerRayToggled += OnControllerRayToggled;
            interactorBehaviorControls.onGazeToggled += OnGazeToggled;
            interactorBehaviorControls.onGrabToggled += OnGrabToggled;
            interactorBehaviorControls.onHandRayToggled += OnHandRayToggled;
            interactorBehaviorControls.onPokeToggled += OnPokeToggled;
        }

        private void OnDisable()
        {
            interactorBehaviorControls.onControllerRayToggled -= OnControllerRayToggled;
            interactorBehaviorControls.onGazeToggled -= OnGazeToggled;
            interactorBehaviorControls.onGrabToggled -= OnGrabToggled;
            interactorBehaviorControls.onHandRayToggled -= OnHandRayToggled;
            interactorBehaviorControls.onPokeToggled -= OnPokeToggled;
        }

        private void SetupSpeechCommand()
        {
            XRSubsystemHelpers.GetFirstSubsystem<KeywordRecognitionSubsystem>().CreateOrGetEventForKeyword("Reset Example").AddListener(ResetExample);
        }

        public void ResetExample()
        {
            if(XRSubsystemHelpers.HandsAggregator != null)
            {
                SetHololensModeActive();
            }
            else
            {
                List<InputDevice> motionControllers = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand, motionControllers);

                if(motionControllers.Count > 0)
                {
                    SetVRModeActive();
                }
            }
        }

        public void SetVRModeActive()
        {
            interactorBehaviorControls.SetVR();

            SetToggleActive(VRToggle, true);
            SetToggleActive(HololensToggle, false);
        }

        public void SetHololensModeActive()
        {
            interactorBehaviorControls.SetHololens();

            SetToggleActive(VRToggle, false);
            SetToggleActive(HololensToggle, true);
        }

        private void OnPokeToggled(bool isActive)
        {
            SetToggleActive(PokeToggle, isActive);
        }

        private void OnHandRayToggled(bool isActive)
        {
            SetToggleActive(HandRayToggle, isActive);
        }

        private void OnGrabToggled(bool isActive)
        {
            SetToggleActive(GrabToggle, isActive);
        }

        private void OnGazeToggled(bool isActive)
        {
            SetToggleActive(GazeToggle, isActive);
        }

        private void OnControllerRayToggled(bool isActive)
        {
            SetToggleActive(ControllerRayToggle, isActive);
        }

        private void SetToggleActive(PressableButton toggle, bool isActive)
        {
            if(toggle.IsToggled != isActive)
            {
                toggle.ForceSetToggled(isActive, true);
            }
        }
    }
}
