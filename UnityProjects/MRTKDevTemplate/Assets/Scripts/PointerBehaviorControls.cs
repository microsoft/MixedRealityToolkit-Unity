using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Utility class to control <see cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/> of pointers.
    /// Hook up buttons to the public functions to turn rays on and off.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/PointerBehaviorControls")]
    public class PointerBehaviorControls : MonoBehaviour
    {
        [SerializeField]
        private InteractionModeManager interactionModeManager;

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

        public void SetGazeActive(bool isActive)
        {
            ToggleInteractor(gazeInteractor, isActive);
        }

        public void SetHandPokeActive(bool isActive)
        {
            ToggleInteractors(pokeInteractors, isActive);
        }

        public void SetHandGrabActive(bool isActive)
        {
            ToggleInteractors(grabInteractors, isActive);
        }

        public void SetControllerRayActive(bool isActive)
        {
            ToggleInteractors(controllerRayInteractors, isActive);
        }

        public void SetHandRayActive(bool isActive)
        {
            ToggleInteractors(handRaysInteractors, isActive);
        }

        private void ToggleInteractors(XRBaseInteractor[] interactors, bool isActive)
        {
            if (isActive)
                ActivateInteractors(interactors);
            else
                DeactivateInteractors(interactors);
        }

        private void ToggleInteractor(XRBaseInteractor interactor, bool isActive)
        {
            if (isActive)
                ActivateInteractor(interactor);
            else
                DeactivateInteractor(interactor);
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
            interactor.enabled = false;
        }
    }
}
