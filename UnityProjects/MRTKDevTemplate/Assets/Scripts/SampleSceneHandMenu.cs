// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
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
        private StatefulInteractable handMeshToggle;

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

        private List<RiggedHandMeshVisualizer> handMeshes = new List<RiggedHandMeshVisualizer>();

        private void Start()
        {
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

            handMeshes = new List<RiggedHandMeshVisualizer>(Object.FindObjectsOfType<RiggedHandMeshVisualizer>());
            handMeshToggle.ForceSetToggled((handMeshes.Any((handMesh) => handMesh.IsShowingHands)));
            handMeshToggle.OnClicked.AddListener(() => SetHandMeshActive(handMeshToggle.IsToggled));
            

            previousSceneButton.OnClicked.AddListener(() => GoToPreviousScene());
            nextSceneButton.OnClicked.AddListener(() => GoToNextScene());

            sceneTitleLabel.text = SceneManager.GetActiveScene().name + ".unity";
        }
        
        /// <summary>
        /// Toggle hand rays.
        /// </summary>
        public void SetHandRaysActive(bool value)
        {
            var handRays = PlayspaceUtilities.ReferenceTransform.GetComponentsInChildren<MRTKRayInteractor>(true);

            foreach (var interactor in handRays)
            {
                interactor.gameObject.SetActive(value);
            }
        }

        /// <summary>
        /// Toggle gaze pinch interactors.
        /// </summary>
        public void SetGazePinchActive(bool value)
        {
            var gazePinchInteractors = PlayspaceUtilities.ReferenceTransform.GetComponentsInChildren<GazePinchInteractor>(true);

            foreach (var interactor in gazePinchInteractors)
            {
                interactor.gameObject.SetActive(value);
            }
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

        void SetHandMeshActive(bool value)
        {
            if (handMeshes.Count == 0)
            {
                handMeshes = new List<RiggedHandMeshVisualizer>(Object.FindObjectsOfType<RiggedHandMeshVisualizer>());
            }

            if (handMeshes.Count == 0) {
                handMeshToggle.enabled = false;
                return;
            }

            foreach (var handMesh in handMeshes)
            {
                if (value)
                {
                    handMesh.ShowHandsOnTransparentDisplays = true;
                    handMesh.enabled = true;
                }
                else
                {
                    handMesh.enabled = false;
                }
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
