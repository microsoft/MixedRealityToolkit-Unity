// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using global::Unity.XR.CoreUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Sample for allowing the game object that this script is attached to follow the user's eye gaze
    /// at a given distance of <see cref="defaultDistanceInMeters"/>. 
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/FollowEyeGaze")]
    public class FollowEyeGaze : MonoBehaviour
    {
        [Tooltip("Display the game object along the eye gaze ray at a default distance (in meters).")]
        [SerializeField]
        private float defaultDistanceInMeters = 2f;

        [Tooltip("The default color of the GameObject.")]
        [SerializeField]
        private Color idleStateColor;

        [Tooltip("The highlight color of the GameObject when hovered over another StatefulInteractable.")]
        [SerializeField]
        private Color hightlightStateColor;

        private Material material;

        [SerializeField]
        private ActionBasedController gazeController;

        [SerializeField]
        private InputActionProperty _gazeTranslationAction;

        private IGazeInteractor gazeInteractor;
        private List<IXRInteractable> targets;

        private void Awake()
        {
            material = GetComponent<Renderer>().material;

            gazeInteractor = gazeController.GetComponentInChildren<IGazeInteractor>();

            targets = new List<IXRInteractable>();
        }

        private void OnEnable()
        {
            if (_gazeTranslationAction == null || _gazeTranslationAction.action == null)
            {
                return;
            }

            _gazeTranslationAction.action.performed += FollowEyeGazeAction;
            _gazeTranslationAction.EnableDirectAction();
        }
        
        private void OnDisable()
        {
            if (_gazeTranslationAction == null || _gazeTranslationAction.action == null)
            {
                return;
            }

            _gazeTranslationAction.DisableDirectAction();
            _gazeTranslationAction.action.performed -= FollowEyeGazeAction;
        }

        private void Update()
        {
            targets.Clear();

            gazeInteractor.GetValidTargets(targets);
            material.color = targets.Count > 0 ? hightlightStateColor : idleStateColor;

            // Note: A better workflow would be to create and attach a prefab to the MRTK Gaze Controller object.
            // Doing this will parent the cursor to the gaze controller transform and be updated automatically.
            var pose = gazeController.transform.GetWorldPose();
            transform.position = pose.position + gazeController.transform.forward * defaultDistanceInMeters;
        }

        private void FollowEyeGazeAction(InputAction.CallbackContext obj)
        {
            // Example of obtaining gaze input action properties
            Vector3 translation = _gazeTranslationAction.action.ReadValue<Vector3>();
        }
    }
}
