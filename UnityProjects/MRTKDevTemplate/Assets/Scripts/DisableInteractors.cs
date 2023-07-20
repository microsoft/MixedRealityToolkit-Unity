// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UX;
using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// This example demonstrates how to turn interactors on and off by 
    /// specifying custom behaviors.
    /// </summary>
    [RequireComponent(typeof(InteractorBehaviorControls))]
    [AddComponentMenu("MRTK/Examples/Disable Interactors")]
    public class DisableInteractors : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The button to toggle Gaze input on and off.")]
        private PressableButton gazeToggle;

        [SerializeField]
        [Tooltip("The button to toggle Grab input on and off.")]
        private PressableButton grabToggle;

        [SerializeField]
        [Tooltip("The button to toggle Poke input on and off.")]
        private PressableButton pokeToggle;

        [SerializeField]
        [Tooltip("The button to toggle Hand Ray input on and off.")]
        private PressableButton handRayToggle;

        [SerializeField]
        [Tooltip("The button to toggle Controller Ray input on and off.")]
        private PressableButton controllerRayToggle;

        [SerializeField]
        [Tooltip("The button to activate VR device specific pointer types.")]
        private PressableButton VRToggle;

        [SerializeField]
        [Tooltip("The button to activate Hololens device specific pointer types.")]
        private PressableButton hololensToggle;

        private InteractorBehaviorControls interactorBehaviorControls;

        /// <summary>
        /// A Unity event function that is called when an enabled script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            interactorBehaviorControls = GetComponent<InteractorBehaviorControls>();

            SetupSpeechCommand();
        }

        /// <summary>
        /// A Unity event function that is called on the frame when a script is enabled just before any of the update methods are called the first time.
        /// </summary> 
        private void Start()
        {
            ResetExample();
        }

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary> 
        private void OnEnable()
        {
            interactorBehaviorControls.onControllerRayToggled += OnControllerRayToggled;
            interactorBehaviorControls.onGazeToggled += OnGazeToggled;
            interactorBehaviorControls.onGrabToggled += OnGrabToggled;
            interactorBehaviorControls.onHandRayToggled += OnHandRayToggled;
            interactorBehaviorControls.onPokeToggled += OnPokeToggled;
        }

        /// <summary>
        /// A Unity event function that is called when the script component has been disabled.
        /// </summary>
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

        /// <summary>
        /// Reset the interactors of this scene, based of the availability of articulated hand input.
        /// </summary>
        /// <remarks>
        /// If the application has configured a <see cref="IHandsAggregatorSubsystem"/>, this will put the interactors into HoloLens mode,
        /// otherwise the interactors are put into VR mode.
        /// </remarks>
        public void ResetExample()
        {
            if (XRSubsystemHelpers.HandsAggregator != null)
            {
                SetHololensModeActive();
            }
            else
            {
                List<InputDevice> motionControllers = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand, motionControllers);

                if (motionControllers.Count > 0)
                {
                    SetVRModeActive();
                }
            }
        }

        /// <summary>
        /// Method triggered by the VR button in the scene
        /// </summary>
        public void SetVRModeActive()
        {
            interactorBehaviorControls.SetVR();

            SetToggleActive(VRToggle, true);
            SetToggleActive(hololensToggle, false);
        }

        /// <summary>
        /// Method triggered by the Hololens 2 button in the scene
        /// </summary>
        public void SetHololensModeActive()
        {
            interactorBehaviorControls.SetHololens();

            SetToggleActive(VRToggle, false);
            SetToggleActive(hololensToggle, true);
        }

        private void OnPokeToggled(bool isActive)
        {
            SetToggleActive(pokeToggle, isActive);
        }

        private void OnHandRayToggled(bool isActive)
        {
            SetToggleActive(handRayToggle, isActive);
        }

        private void OnGrabToggled(bool isActive)
        {
            SetToggleActive(grabToggle, isActive);
        }

        private void OnGazeToggled(bool isActive)
        {
            SetToggleActive(gazeToggle, isActive);
        }

        private void OnControllerRayToggled(bool isActive)
        {
            SetToggleActive(controllerRayToggle, isActive);
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
