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
    /// </summary>
    /// <remarks>
    /// Hook up buttons to the public functions to turn interactors on and off.
    /// </remarks>
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
        private XRBaseInteractor[] gazePinchInteractors;

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
        /// event triggered when gaze pinch interactors are toggled on/off
        /// </summary>
        public event Action<bool> onGazePinchToggled;

        /// <summary>
        /// Enable all interactors
        /// </summary>
        public void EnableAll()
        {
            SetControllerRayActive(true);
            SetHandGrabActive(true);
            SetHandPokeActive(true);
            SetHandRayActive(true);
            SetGazeActive(true);
            SetGazePinchActive(true);
        }

        /// <summary>
        /// Enable everything, and disable all gaze interactions.
        /// </summary>
        public void OnlyHands()
        {
            EnableAll();
            SetGazeActive(false);
            SetGazePinchActive(false);
        }

        /// <summary>
        /// Enable everything, and disable all hand interactions.
        /// </summary>
        public void OnlyGaze()
        {
            EnableAll();
            SetControllerRayActive(false);
            SetHandGrabActive(false);
            SetHandPokeActive(false);
            SetHandRayActive(false);
        }

        /// <summary>
        /// Enable everything, and disable controller ray interactors.
        /// </summary>
        public void DisablControllerRays()
        {
            EnableAll();
            SetControllerRayActive(false);
        }

        /// <summary>
        /// Enable everything, and disable hand grab interactors.
        /// </summary>
        public void DisableHandGrabs()
        {
            EnableAll();
            SetHandGrabActive(false);
        }

        /// <summary>
        /// Enable everything, and disable hand poke interactors.
        /// </summary>
        public void DisableHandPokes()
        {
            EnableAll();
            SetHandPokeActive(false);
        }

        /// <summary>
        /// Enable everything, and disable hand poke interactors.
        /// </summary>
        public void DisableHandRays()
        {
            EnableAll();
            SetHandRayActive(false);
        }

        /// <summary>
        /// Enable everything, and disable gaze interactors.
        /// </summary>
        public void DisableGaze()
        {
            EnableAll();
            SetGazeActive(false);
        }

        /// <summary>
        /// Enable everything, and disable gaze pinch interactors.
        /// </summary>
        public void DisableGazePinch()
        {
            EnableAll();
            SetGazePinchActive(false);
        }

        /// <summary>
        /// Enable or disable the specified gaze interactors.
        /// </summary>
        public void SetGazeActive(bool isActive)
        {
            if (ToggleInteractor(gazeInteractor, isActive))
            {
                onGazeToggled?.Invoke(isActive);
            }
        }

        /// <summary>
        /// Enable or disable the specified gaze pinch interactors.
        /// </summary>
        public void SetGazePinchActive(bool isActive)
        {
            if (ToggleInteractors(gazePinchInteractors, isActive))
            {
                onGazeToggled?.Invoke(isActive);
            }
        }

        /// <summary>
        /// Enable or disable the specified poke interactors.
        /// </summary>
        public void SetHandPokeActive(bool isActive)
        {
            if (ToggleInteractors(pokeInteractors, isActive))
            {
                onPokeToggled?.Invoke(isActive);
            }
        }

        /// <summary>
        /// Enable or disable the specified hand grab interactors.
        /// </summary>
        public void SetHandGrabActive(bool isActive)
        {
            if (ToggleInteractors(grabInteractors, isActive))
            {
                onGrabToggled?.Invoke(isActive);
            }
        }

        /// <summary>
        /// Enable or disable the specified controller ray interactors.
        /// </summary>
        public void SetControllerRayActive(bool isActive)
        {
            if (ToggleInteractors(controllerRayInteractors, isActive))
            {
                onControllerRayToggled?.Invoke(isActive);
            }
        }

        /// <summary>
        /// Enable or disable the specified hand ray interactors.
        /// </summary>
        public void SetHandRayActive(bool isActive)
        {
            if (ToggleInteractors(handRaysInteractors, isActive))
            {
                onHandRayToggled?.Invoke(isActive);
            }
        }

        /// <summary>
        /// Toggle interactors, and return true if something changed.
        /// </summary>
        private bool ToggleInteractors(XRBaseInteractor[] interactors, bool isActive)
        {
            if (isActive)
            {
                return ActivateInteractors(interactors);
            }
            else
            {
                return DeactivateInteractors(interactors);
            }
        }

        /// <summary>
        /// Toggle interactor, and return true if something changed.
        /// </summary>
        private bool ToggleInteractor(XRBaseInteractor interactor, bool isActive)
        {
            if (isActive)
            {
                return ActivateInteractor(interactor);
            }
            else
            {
                return DeactivateInteractor(interactor);
            }
        }

        /// <summary>
        /// Activate interactors, and return true if something changed.
        /// </summary>
        private bool ActivateInteractors(XRBaseInteractor[] interactors)
        {
            bool change = false;
            for (int i = 0; i < interactors.Length; i++)
            {
                change |= ActivateInteractor(interactors[i]);
            }
            return change;
        }

        /// <summary>
        /// Activate interactor, and return true if something changed.
        /// </summary>
        private bool ActivateInteractor(XRBaseInteractor interactor)
        {
            if (interactor.gameObject.activeSelf)
            {
                return false;
            }

            interactor.gameObject.SetActive(true);
            interactionModeManager.RegisterInteractor(interactor);
            return true;
        }

        /// <summary>
        /// Deactivate interactors, and return true if something changed.
        /// </summary>
        private bool DeactivateInteractors(XRBaseInteractor[] interactors)
        {
            bool change = false;
            for (int i = 0; i < interactors.Length; i++)
            {
                change |= DeactivateInteractor(interactors[i]);
            }
            return change;
        }

        /// <summary>
        /// Deactivate interactor, and return true if something changed.
        /// </summary>
        private bool DeactivateInteractor(XRBaseInteractor interactor)
        {
            if (!interactor.gameObject.activeSelf)
            {
                return false;
            }

            interactionModeManager.UnregisterInteractor(interactor);
            interactor.gameObject.SetActive(false);
            return true;
        }
    }
}
