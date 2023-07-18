// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Example class to demonstrate how to turn various interactors on and off.
    /// Hook up buttons to the public functions to turn interactors on and off.
    /// </summary>
    [AddComponentMenu("MRTK/Examples/InteractorBehaviorControls")]
    public class InteractorBehaviorControls : MonoBehaviour
    {
        [SerializeField]
        private InteractionModeManager interactionModeManager;

        [SerializeField]
        private XRInteractionManager interactionManager;

        [SerializeField]
        private XRBaseInteractor[] handRaysInteractors;

        [SerializeField]
        private XRBaseInteractor[] controllerRayInteractors;

        [SerializeField]
        private XRBaseInteractor[] grabInteractors;

        [SerializeField]
        private XRBaseInteractor[] pokeInteractors;

        [SerializeField]
        private XRBaseInteractor gazeInteractor;

        /// <summary>
        /// event triggered when hand rays are toggled on or off
        /// </summary>
        public event Action<bool> onHandRayToggled;

        /// <summary>
        /// event triggered when controller rays are toggled on or off
        /// </summary>
        public event Action<bool> onControllerRayToggled;

        /// <summary>
        /// event triggered when grab interactors are toggled on/off
        /// </summary>
        public event Action<bool> onGrabToggled;

        /// <summary>
        /// event triggered when poke interactors are toggled on/off
        /// </summary>
        public event Action<bool> onPokeToggled;

        /// <summary>
        /// event triggered when gaze interactors are toggled on/off
        /// </summary>
        public event Action<bool> onGazeToggled;

        /// <summary>
        /// Sets pointer behavior to mimic HoloLens 2
        /// Poke interactor will be On
        /// Grab interactor will be On
        /// HandRay interactor will be On
        /// MotionControllerRay interactor will be Off
        /// Gaze interactor will be On
        /// </summary>
        public void SetHololens()
        {
            SetHandPokeActive(true);
            SetHandGrabActive(true);
            SetHandRayActive(true);
            SetControllerRayActive(false);
            SetGazeActive(true);
        }

        /// <summary>
        /// Sets pointer states to mimic traditional vr behavior.
        /// Poke interactor will be Off
        /// Grab interactor will be Off
        /// HandRay interactor will be Off
        /// MotionControllerRay interactor will be On
        /// Gaze interactor will be Off
        /// </summary>
        public void SetVR()
        {
            SetHandPokeActive(false);
            SetHandGrabActive(false);
            SetHandRayActive(false);
            SetControllerRayActive(true);
            SetGazeActive(false);
        }

        public void SetGazeActive(bool isActive)
        {
            ToggleInteractor(gazeInteractor, isActive);
            onGazeToggled?.Invoke(isActive);
        }

        public void SetHandPokeActive(bool isActive)
        {
            ToggleInteractors(pokeInteractors, isActive);
            onPokeToggled?.Invoke(isActive);
        }

        public void SetHandGrabActive(bool isActive)
        {
            ToggleInteractors(grabInteractors, isActive);
            onGrabToggled?.Invoke(isActive);
        }

        public void SetControllerRayActive(bool isActive)
        {
            ToggleInteractors(controllerRayInteractors, isActive);
            onControllerRayToggled?.Invoke(isActive);
        }

        public void SetHandRayActive(bool isActive)
        {
            ToggleInteractors(handRaysInteractors, isActive);
            onHandRayToggled?.Invoke(isActive);
        }

        private void ToggleInteractors(XRBaseInteractor[] interactors, bool isActive)
        {
            if (isActive)
            {
                ActivateInteractors(interactors);
            }
            else
            {
                DeactivateInteractors(interactors);
            }
        }

        private void ToggleInteractor(XRBaseInteractor interactor, bool isActive)
        {
            if (isActive)
            {
                ActivateInteractor(interactor);
            }
            else
            {
                DeactivateInteractor(interactor);
            }
        }

        private void ActivateInteractors(XRBaseInteractor[] interactors)
        {
            for (int i = 0; i < interactors.Length; i++)
            {
                ActivateInteractor(interactors[i]);
            }
        }

        private void ActivateInteractor(XRBaseInteractor interactor)
        {
            interactor.gameObject.SetActive(true);
            interactionModeManager.RegisterInteractor(interactor);
        }

        private void DeactivateInteractors(XRBaseInteractor[] interactors)
        {
            for (int i = 0; i < interactors.Length; i++)
            {
                DeactivateInteractor(interactors[i]);
            }
        }

        private void DeactivateInteractor(XRBaseInteractor interactor)
        {
            interactionModeManager.UnregisterInteractor(interactor);
            interactor.gameObject.SetActive(false);
        }
    }
}
