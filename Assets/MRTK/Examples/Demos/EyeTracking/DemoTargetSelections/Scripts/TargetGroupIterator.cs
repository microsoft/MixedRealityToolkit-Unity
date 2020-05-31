// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// Iterates through a given set of targets based on a required TargetGroupCreator.
    /// </summary>
    [RequireComponent(typeof(TargetGroupCreatorRadial))]
    [AddComponentMenu("Scripts/MRTK/Examples/TargetGroupIterator")]
    public class TargetGroupIterator : MonoBehaviour, IMixedRealityPointerHandler
    {
        #region Variables
        [Tooltip("The amount of targets to select (the amount of displayed targets may be higher).")]
        [SerializeField]
        private int nrOfTargetsToSelect = 8;

        [Tooltip("Randomize the order of targets to select.")]
        [SerializeField]
        private bool Randomize = false;

        [Tooltip("The highlight color to indicate the current query target.")]
        [SerializeField]
        private Color highlightColor = Color.white;

        /// <summary>
        /// Property that returns the highlight color.
        /// </summary>
        public Color HighlightColor => highlightColor;

        [Tooltip("Maximum number of tries for selecting the current query target before automatically proceeding.")]
        [SerializeField]
        private int maxNumberOfTries = 5;

        [Tooltip("[Optional] Deactivate the query target after it's been selected.")]
        [SerializeField]
        private bool DeactiveDistractors = false;

        [Tooltip("[Optional] Template GameObject that will be displayed at the center of the current query target.")]
        [SerializeField]
        private GameObject template_VisualMarkerForCurrTarget = null;

        [Tooltip("[Optional] Name of the scene to load after finishing selecting the specified number of targets.")]
        [SerializeField]
        private string SceneToLoadOnFinish = string.Empty;

        [Tooltip("[Optional] Audio clip that will be played after selecting all required targets.")]
        [SerializeField]
        private AudioClip AudioApplauseOnFinish = null;

        public delegate void TargetGroupEventHandler();
        public event TargetGroupEventHandler OnAllTargetsSelected;
        public event TargetGroupEventHandler OnTargetSelected;

        private bool notInitializedYet = true;
        private int countSelectionTries;
        private GameObject[] targets;
        private int currTargetIndex = 0;
        private Color originalColor;
        #endregion

        [SerializeField]
        private MixedRealityInputAction selectAction = MixedRealityInputAction.None;

        private void Start()
        {
            // Save original colors for resetting after highlighting them
            SaveOriginalColors();
        }

        private void Update()
        {
            if (notInitializedYet)
            {
                ResetIterator();
                notInitializedYet = false;
            }
        }

        /// <summary>
        /// Reset the number of counted tries for selecting the current query target.
        /// </summary>
        private void ResetAmountOfTries()
        {
            countSelectionTries = 0;
        }

        /// <summary>
        /// Reset the target group iterator. 
        /// </summary>
        public void ResetIterator()
        {
            currTargetIndex = 0;
            ResetAmountOfTries();
            targets = GetComponent<TargetGroupCreatorRadial>().InstantiatedObjects;

            // Randomize the order in which targets are highlighted
            if (Randomize)
            {
                targets = EyeTrackingDemoUtils.RandomizeListOrder(targets);
            }

            // Show first highlighted target
            // Hide all targets first
            foreach (GameObject obj in targets)
            {
                obj.SetActive(!DeactiveDistractors);
            }

            // Then activate current target and highlight
            ShowHighlights();
        }

        /// <summary>
        /// Proceed to the next target. If the end is reached, fire an event that the target group has been fully 
        /// iterated and restart the scene.
        /// </summary>
        private void ProceedToNextTarget()
        {
            Debug.Log("[TargetGroupIterator] >> Next target: " + currTargetIndex + " / " + nrOfTargetsToSelect);

            ResetAmountOfTries();
            Fire_OnTargetSelected();

            if (currTargetIndex < nrOfTargetsToSelect - 1)
            {
                // 1. Let's reset the highlight for the last target.
                HideHighlights();

                // 2. If "DisableDistractors" is true then let's hide the selected target
                if (DeactiveDistractors)
                    CurrentTarget.gameObject.SetActive(false);

                // 3. Let's update to highlight the new target.
                currTargetIndex++;
                ShowHighlights();
            }
            else // At the end
            {
                // Fire an event to inform listeners that all targets have been iterated through.
                Fire_OnAllTargetsSelected();

                // Play some audio feedback (e.g., a cheerful applause). 
                AudioFeedbackPlayer.Instance.PlaySound(AudioApplauseOnFinish);

                // If a scene is given, reload the scene after a short timeout.
                if (SceneToLoadOnFinish != "")
                    StartCoroutine(EyeTrackingDemoUtils.LoadNewScene(SceneToLoadOnFinish, 0.1f));
            }
        }

        /// <summary>
        /// Property which returns the current query target.
        /// </summary>
        public GameObject CurrentTarget
        {
            get { return targets[currTargetIndex]; }
        }

        /// <summary>
        /// Property which returns the previous query target or 'null' in case there is no previous target.
        /// </summary>
        public GameObject PreviousTarget
        {
            get
            {
                if (currTargetIndex > 0)
                    return targets[currTargetIndex - 1];
                else
                    return null;
            }
        }

        /// <summary>
        /// Property which returns whether the current query target is valid. 
        /// </summary>
        public bool CurrentTargetIsValid
        {
            get { return ((targets != null) && (targets.Length > 0) && (currTargetIndex < targets.Length)); }
        }

        /// <summary>
        /// Save the original base color of the target (before changing it for highlighting).
        /// </summary>
        private void SaveOriginalColors()
        {
            if ((CurrentTargetIsValid) && (originalColor == new Color(0, 0, 0, 0)))
            {
                Renderer[] renderers = CurrentTarget.GetComponents<Renderer>();
                if ((renderers.Length > 0) && (renderers[0].materials.Length > 0))
                {
                    originalColor = renderers[0].materials[0].color;
                }
            }
        }

        /// <summary>
        /// Make sure the query target is set up before highlighting it.
        /// </summary>
        private void HighlightTarget()
        {
            if (CurrentTarget == null)
                ProceedToNextTarget();

            CurrentTarget.gameObject.SetActive(true);
            HighlightTarget(highlightColor);
        }

        /// <summary>
        /// Change the base color of the current query target based on the given color.
        /// </summary>
        /// <param name="color">Highlight tint</param>
        private void HighlightTarget(Color color)
        {
            Renderer[] renderers = CurrentTarget.GetComponents<Renderer>();
            if ((renderers.Length > 0) && (renderers[0].materials.Length > 0))
            {
                renderers[0].materials[0].color = color;
            }
        }

        /// <summary>
        /// Show a visual marker at the center of the current query target.
        /// </summary>
        private void ShowVisualMarkerForCurrTarget()
        {
            if (template_VisualMarkerForCurrTarget != null)
            {
                template_VisualMarkerForCurrTarget.SetActive(true);

                if (currTargetIndex < targets.Length)
                    template_VisualMarkerForCurrTarget.transform.position = targets[currTargetIndex].transform.position;
            }
        }

        /// <summary>
        /// Hide the visual marker that indicated the current query target.
        /// </summary>
        private void HideVisualMarkerForCurrTarget()
        {
            if (template_VisualMarkerForCurrTarget != null)
            {
                template_VisualMarkerForCurrTarget.SetActive(false);
            }
        }

        /// <summary>
        /// Resets the current query target to its original base color.
        /// </summary>
        private void UnhighlightTarget()
        {
            HighlightTarget(originalColor);
        }

        /// <summary>
        /// Highlight the current query target.
        /// </summary>
        private void ShowHighlights()
        {
            if (CurrentTargetIsValid)
            {
                SaveOriginalColors();
                HighlightTarget();
                ShowVisualMarkerForCurrTarget();
                ResetAmountOfTries();
            }
        }

        /// <summary>
        /// Handling hiding any indicators for the current query target.
        /// </summary>
        private void HideHighlights()
        {
            UnhighlightTarget();
            HideVisualMarkerForCurrTarget();
        }

        /// <summary>
        /// The event is triggered when all targets in the target group have been selected.
        /// </summary>
        private void Fire_OnAllTargetsSelected()
        {
            // Make sure someone is listening to event
            if (OnAllTargetsSelected != null)
                OnAllTargetsSelected();
        }

        /// <summary>
        /// The event is triggered if a target (not necessarily the query target) has been selected.
        /// </summary>
        private void Fire_OnTargetSelected()
        {
            // Make sure someone is listening to event
            if (OnTargetSelected != null)
                OnTargetSelected();
        }

        #region IMixedRealityPointerHandler
        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            if (eventData.MixedRealityInputAction == selectAction)
            {
                Debug.Log(">> TargetGroupIterator -- > OnPointerClicked");
                if (CurrentTargetIsValid)
                {
                    // Once a target has been selected, check whether this is the correct target and proceed to highlighting the next one. 
                    if (eventData.selectedObject == targets[currTargetIndex])
                    {
                        ProceedToNextTarget();
                        return;
                    }

                    countSelectionTries++;
                    Debug.Log("TargetGridIterator >> Try #" + countSelectionTries + "");

                    if (countSelectionTries >= maxNumberOfTries)
                    {
                        Debug.Log("TargetGridIterator >> Show next target - Too many tries [Amount triggered].");
                        ProceedToNextTarget();
                    }
                }
            }
        }
        #endregion
    }
}