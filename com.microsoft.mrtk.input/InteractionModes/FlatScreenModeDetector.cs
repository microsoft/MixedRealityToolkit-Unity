// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    internal class FlatScreenModeDetector : MonoBehaviour, IInteractionModeDetector
    {
        [SerializeField]
        [Tooltip("The interaction mode to be set when the interactor is hovering over an interactable.")]
        private InteractionMode flatScreenInteractionMode;

        [SerializeField]
        [Tooltip("List of XR Base Controllers that this interaction mode detector has jurisdiction over. Interaction modes will be set on all specified controllers.")]
        private List<GameObject> controllers;

        public InteractionMode ModeOnDetection => flatScreenInteractionMode;

        protected ControllerLookup controllerLookup = null;

        public void Awake()
        {
            ControllerLookup[] lookups = GameObject.FindObjectsOfType(typeof(ControllerLookup)) as ControllerLookup[];
            if (lookups.Length == 0)
            {
                Debug.LogError("Could not locate an instance of the ControllerLookup class in the hierarchy. It is recommended to add ControllerLookup to your camera rig.");
            }
            else if (lookups.Length > 1)
            {
                Debug.LogWarning("Found more than one instance of the ControllerLookup class in the hierarchy. Defaulting to the first instance.");
                controllerLookup = lookups[0];
            }
            else
            {
                controllerLookup = lookups[0];
            }
        }

        /// <inheritdoc />
        public List<GameObject> GetControllers() => controllers;

        public bool IsModeDetected()
        {
            // Flat screen mode is only active if the Left and Right Hand Controllers aren't being tracked
            return !controllerLookup.LeftHandController.currentControllerState.inputTrackingState.HasPositionAndRotation() && !controllerLookup.RightHandController.currentControllerState.inputTrackingState.HasPositionAndRotation();
        }
    }
}
