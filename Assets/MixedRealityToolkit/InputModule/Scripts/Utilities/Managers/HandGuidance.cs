// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using UnityEngine;

#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#else
using UnityEngine.VR.WSA.Input;
#endif
#endif

namespace MixedRealityToolkit.InputModule.Utilities.Managers
{
    /// <summary>
    /// Show a hand guidance indicator when the user's hand is close to leaving the camera's view.
    /// </summary>
    public class HandGuidance : Singleton<HandGuidance>
    {
        [Tooltip("The Cursor object the HandGuidanceIndicator will be positioned around.")]
        public GameObject Cursor;

        [Tooltip("GameObject to display when your hand is about to lose tracking.")]
        public GameObject HandGuidanceIndicator;

        // Hand source loss risk to start showing a hand indicator.
        // As the source loss risk approaches 1, the hand is closer to being out of view.
        [Range(0.0f, 1.0f)]
        [Tooltip("When to start showing the Hand Guidance Indicator. 1 is out of view, 0 is centered in view.")]
        public float HandGuidanceThreshold = 0.5f;

#if UNITY_WSA
        private GameObject handGuidanceIndicatorGameObject;

        private Quaternion defaultHandGuidanceRotation;

        private uint? currentlyTrackedHand;

        protected override void Awake()
        {
            base.Awake();
            if (HandGuidanceIndicator == null)
            {
                Debug.LogError("Please include a GameObject for the Hand Guidance Indicator.");
            }

            if (Cursor == null)
            {
                Debug.LogError("Please include a GameObject for the Cursor to display the indicator around.");
            }

            if (HandGuidanceIndicator != null)
            {
                // Cache the initial rotation of the HandGuidanceIndicator so future rotations 
                // can be done with respect to this rotation.
                defaultHandGuidanceRotation = HandGuidanceIndicator.transform.rotation;
            }

            // Create an object in the scene for the guidance indicator and default it to not be visible.
            handGuidanceIndicatorGameObject = Instantiate(HandGuidanceIndicator);
            handGuidanceIndicatorGameObject.SetActive(false);

            // Register for hand and finger events to know where your hand
            // is being tracked and what state it is in.
#if UNITY_2017_2_OR_NEWER
            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
            InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
            InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;
#else
            InteractionManager.SourceLost += InteractionManager_SourceLost;
            InteractionManager.SourceUpdated += InteractionManager_SourceUpdated;
            InteractionManager.SourceReleased += InteractionManager_SourceReleased;
#endif
        }

        private void ShowHandGuidanceIndicator(InteractionSourceState hand)
        {
            if (!currentlyTrackedHand.HasValue)
            {
                return;
            }

            // Get the position and rotation of the hand guidance indicator and display the indicator object.
            if (handGuidanceIndicatorGameObject != null)
            {
                Vector3 position;
                Quaternion rotation;
                GetIndicatorPositionAndRotation(hand, out position, out rotation);

                handGuidanceIndicatorGameObject.transform.position = position;
                handGuidanceIndicatorGameObject.transform.rotation = rotation * defaultHandGuidanceRotation;
                handGuidanceIndicatorGameObject.SetActive(true);
            }
        }

        private void HideHandGuidanceIndicator(InteractionSourceState hand)
        {
            if (!currentlyTrackedHand.HasValue)
            {
                return;
            }

            if (handGuidanceIndicatorGameObject != null)
            {
                handGuidanceIndicatorGameObject.SetActive(false);
            }
        }

        private void GetIndicatorPositionAndRotation(InteractionSourceState hand, out Vector3 position, out Quaternion rotation)
        {
            // Update the distance from IndicatorParent based on the user's hand's distance from the center of the view.
            // Bound this distance by this maxDistanceFromCenter field, in meters.
            const float maxDistanceFromCenter = 0.3f;
            float distanceFromCenter = (float)(hand.properties.sourceLossRisk * maxDistanceFromCenter);

            // Subtract direction from origin so that the indicator is between the hand and the origin.
            position = Cursor.transform.position - hand.properties.sourceLossMitigationDirection * distanceFromCenter;
            rotation = Quaternion.LookRotation(CameraCache.Main.transform.forward, hand.properties.sourceLossMitigationDirection);
        }

#if UNITY_2017_2_OR_NEWER
        private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs obj)
        {
            if (obj.state.source.kind == InteractionSourceKind.Hand)
            {
                InteractionSourceState hand = obj.state;

                // Only display hand indicators when we are in a holding state, since hands going out of view will affect any active gestures.
                if (!hand.anyPressed)
                {
                    return;
                }

                // Only track a new hand if are not currently tracking a hand.
                if (!currentlyTrackedHand.HasValue)
                {
                    currentlyTrackedHand = hand.source.id;
                }
                else if (currentlyTrackedHand.Value != hand.source.id)
                {
                    // This hand is not the currently tracked hand, do not drawn a guidance indicator for this hand.
                    return;
                }

                // Start showing an indicator to move your hand toward the center of the view.
                if (hand.properties.sourceLossRisk > HandGuidanceThreshold)
                {
                    ShowHandGuidanceIndicator(hand);
                }
                else
                {
                    HideHandGuidanceIndicator(hand);
                }
            }
        }

        private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs obj)
        {
            if (obj.state.source.kind == InteractionSourceKind.Hand)
            {
                // Stop displaying the guidance indicator when the user releases their finger from the pressed state.
                RemoveTrackedHand(obj.state);
            }
        }

        private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs obj)
        {
            if (obj.state.source.kind == InteractionSourceKind.Hand)
            {
                // Stop displaying the guidance indicator when the user's hand leaves the view.
                RemoveTrackedHand(obj.state);
            }
        }

        private void RemoveTrackedHand(InteractionSourceState hand)
        {
            // Only remove a hand if we are currently tracking a hand, and the hand to remove matches this tracked hand.
            if (currentlyTrackedHand.HasValue && currentlyTrackedHand.Value == hand.source.id)
            {
                // Remove a hand by hiding the guidance indicator and nulling out the currentlyTrackedHand field.
                handGuidanceIndicatorGameObject.SetActive(false);
                currentlyTrackedHand = null;
            }
        }

        protected override void OnDestroy()
        {
            InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;
            InteractionManager.InteractionSourceUpdated -= InteractionManager_InteractionSourceUpdated;
            InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;

            base.OnDestroy();
        }
#else
        private void InteractionManager_SourceUpdated(InteractionSourceState hand)
        {
            // Only display hand indicators when we are in a holding state, since hands going out of view will affect any active gestures.
            if (!hand.pressed)
            {
                return;
            }

            // Only track a new hand if are not currently tracking a hand.
            if (!currentlyTrackedHand.HasValue)
            {
                currentlyTrackedHand = hand.source.id;
            }
            else if (currentlyTrackedHand.Value != hand.source.id)
            {
                // This hand is not the currently tracked hand, do not drawn a guidance indicator for this hand.
                return;
            }

            // Start showing an indicator to move your hand toward the center of the view.
            if (hand.properties.sourceLossRisk > HandGuidanceThreshold)
            {
                ShowHandGuidanceIndicator(hand);
            }
            else
            {
                HideHandGuidanceIndicator(hand);
            }
        }

        private void InteractionManager_SourceReleased(InteractionSourceState hand)
        {
            // Stop displaying the guidance indicator when the user releases their finger from the pressed state.
            RemoveTrackedHand(hand);
        }

        private void InteractionManager_SourceLost(InteractionSourceState hand)
        {
            // Stop displaying the guidance indicator when the user's hand leaves the view.
            RemoveTrackedHand(hand);
        }

        private void RemoveTrackedHand(InteractionSourceState hand)
        {
            // Only remove a hand if we are currently tracking a hand, and the hand to remove matches this tracked hand.
            if (currentlyTrackedHand.HasValue && currentlyTrackedHand.Value == hand.source.id)
            {
                // Remove a hand by hiding the guidance indicator and nulling out the currentlyTrackedHand field.
                handGuidanceIndicatorGameObject.SetActive(false);
                currentlyTrackedHand = null;
            }
        }

        protected override void OnDestroy()
        {
            InteractionManager.SourceLost -= InteractionManager_SourceLost;
            InteractionManager.SourceUpdated -= InteractionManager_SourceUpdated;
            InteractionManager.SourceReleased -= InteractionManager_SourceReleased;

            base.OnDestroy();
        }
#endif
#endif
    }
}