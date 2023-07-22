// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for samples. While nice to have, this XML documentation is not required for samples.
#pragma warning disable CS1591

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UX;
using Microsoft.MixedReality.Toolkit.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Helper script to implement the demo scene hand menu actions.
    /// </summary>
    [AddComponentMenu("MRTK/Examples/Sample Scene Hand Menu")]
    internal class SampleSceneHandMenu : MonoBehaviour
    {
        [SerializeField]
        private StatefulInteractable rayToggle;

        [SerializeField]
        private StatefulInteractable gazePinchToggle;

        [SerializeField]
        private StatefulInteractable perfOverlayToggle;

        [SerializeField]
        private StatefulInteractable previousSceneButton;

        [SerializeField]
        private StatefulInteractable nextSceneButton;

        [SerializeField]
        private TMP_Text sceneTitleLabel;

        [SerializeField]
        private GameObject profilerObject;

        /// <summary>
        /// A Unity event function that is called when an enabled script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            rayToggle.ForceSetToggled(GetHandRaysActive());
            gazePinchToggle.ForceSetToggled(GetGazePinchActive());

            rayToggle.OnClicked.AddListener(() => SetHandRaysActive(rayToggle.IsToggled));
            gazePinchToggle.OnClicked.AddListener(() => SetGazePinchActive(gazePinchToggle.IsToggled));
            
            previousSceneButton.enabled = IsSceneValid(SceneManager.GetActiveScene().buildIndex - 1);
            nextSceneButton.enabled = IsSceneValid(SceneManager.GetActiveScene().buildIndex + 1);

            SimpleProfiler profiler = Object.FindObjectOfType<SimpleProfiler>(true);
            if (profiler != null)
            {
                profilerObject = profiler.gameObject;
                perfOverlayToggle.OnClicked.AddListener(() => SetPerfOverlayActive(perfOverlayToggle.IsToggled));
            }
            else
            {
                // Removes button if no profiler found in scene.
                perfOverlayToggle.gameObject.SetActive(false);
            }

            previousSceneButton.OnClicked.AddListener(() => GoToPreviousScene());
            nextSceneButton.OnClicked.AddListener(() => GoToNextScene());

            sceneTitleLabel.text = SceneManager.GetActiveScene().name + ".unity";
        }
        
        /// <summary>
        /// Toggle hand rays.
        /// </summary>
        public void SetHandRaysActive(bool value)
        {
            var handRays = PlayspaceUtilities.XROrigin.GetComponentsInChildren<MRTKRayInteractor>(true);

            foreach (var interactor in handRays)
            {
                interactor.gameObject.SetActive(value);
            }
        }

        /// <summary>
        /// Get if all hand rays in the scene are active.
        /// </summary>
        private bool GetHandRaysActive()
        {
            bool active = true;
            var handRays = PlayspaceUtilities.XROrigin.GetComponentsInChildren<MRTKRayInteractor>(true);

            foreach (var interactor in handRays)
            {
                active &= interactor.gameObject.activeSelf;
                if (!active)
                {
                    break;
                }
            }

            return active;
        }

        /// <summary>
        /// Toggle gaze pinch interactors.
        /// </summary>
        public void SetGazePinchActive(bool value)
        {
            var gazePinchInteractors = PlayspaceUtilities.XROrigin.GetComponentsInChildren<GazePinchInteractor>(true);

            foreach (var interactor in gazePinchInteractors)
            {
                interactor.gameObject.SetActive(value);
            }
        }

        /// <summary>
        /// Get if all pinch interactors in the scene are active.
        /// </summary>
        private bool GetGazePinchActive()
        {
            bool active = true;
            var gazePinchInteractors = PlayspaceUtilities.XROrigin.GetComponentsInChildren<GazePinchInteractor>(true);

            foreach (var interactor in gazePinchInteractors)
            {
                active &= interactor.gameObject.activeSelf;
                if (!active)
                {
                    break;
                }
            }

            return active;
        }

        /// <summary>
        /// Toggle perf overlay.
        /// </summary>
        public void SetPerfOverlayActive(bool value)
        {
            if (profilerObject != null)
            {
                profilerObject.SetActive(value);
            }
        }

        /// <summary>
        /// Load the next scene in build order.
        /// </summary>
        public void GoToNextScene()
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            if (IsSceneValid(currentIndex + 1))
            {
                SceneManager.LoadSceneAsync(currentIndex + 1);
            }
        }

        /// <summary>
        /// Load the previous scene in build order.
        /// </summary>
        public void GoToPreviousScene()
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            if (IsSceneValid(currentIndex - 1))
            {
                SceneManager.LoadSceneAsync(currentIndex - 1);
            }
        }

        private bool IsSceneValid(int buildIndex) => buildIndex < SceneManager.sceneCountInBuildSettings && buildIndex >= 0;
    }
}
#pragma warning restore CS1591