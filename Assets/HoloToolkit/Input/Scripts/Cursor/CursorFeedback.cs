// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// CursorFeedback class takes GameObjects to give cursor feedback
    /// to users based on different states.
    /// </summary>
    public class CursorFeedback : MonoBehaviour, ISourceStateHandler
    {
        [Tooltip("Drag a prefab object to display when a hand is detected.")]
        public GameObject HandDetectedAsset;
        private GameObject handDetectedGameObject;

        [Tooltip("Drag a prefab object to parent the feedback assets.")]
        public GameObject FeedbackParent;

        private uint trackedHandsCount;

        private void Awake()
        {
            if (HandDetectedAsset != null)
            {
                handDetectedGameObject = InstantiatePrefab(HandDetectedAsset);
            }
            else
            {
                Debug.LogError("Missing a required game object asset.  Check HandDetectedAsset is not null in editor.");
            }
        }

        private void Start()
        {
            InputManager.Instance.AddGlobalListener(gameObject);
        }

        private void OnDestroy()
        {
            InputManager.Instance.RemoveGlobalListener(gameObject);
        }

        private GameObject InstantiatePrefab(GameObject inputPrefab)
        {
            GameObject instantiatedPrefab = null;

            if (inputPrefab != null && FeedbackParent != null)
            {
                instantiatedPrefab = GameObject.Instantiate(inputPrefab);
                // Assign parent to be the FeedbackParent
                // so that feedback assets move and rotate with this parent.
                instantiatedPrefab.transform.parent = FeedbackParent.transform;
                instantiatedPrefab.transform.localRotation = Quaternion.identity;

                // Set starting state of the prefab's GameObject to be inactive.
                instantiatedPrefab.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Missing a required game object asset.  Check FeedbackParent is not null in editor.");
            }

            return instantiatedPrefab;
        }

        void Update()
        {
            UpdateHandDetectedState();
        }

        private void UpdateHandDetectedState()
        {
            if (handDetectedGameObject == null)
            {
                return;
            }

            handDetectedGameObject.SetActive(trackedHandsCount > 0);
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            // If the source has positional info and there is currently no visible source
            if ((eventData.InputSource.SupportedInputInfo & SupportedInputInfo.Position) != 0)
            {
                trackedHandsCount++;
            }
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            if ((eventData.InputSource.SupportedInputInfo & SupportedInputInfo.Position) != 0)
            {
                trackedHandsCount--;
            }
        }
    }
}