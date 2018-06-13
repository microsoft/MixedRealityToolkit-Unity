// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloToolkit.Unity.Examples.Utilities
{
    /// <summary>
    /// This class is used in the SolverExamples scene, used to swap between active solvers
    /// and placeholder solvers displayed in the scene.
    /// </summary>
    public class SwapVolume : MonoBehaviour, IInputClickHandler
    {
        [SerializeField]
        private GameObject hideThisObject = null;

        [SerializeField]
        private GameObject spawnThisPrefab = null;

        [SerializeField]
        private bool updateSolverTargetToClickSource = true;

        private SolverHandler solverHandler;
        private GameObject spawnedObject;

        private void Awake()
        {
            // This example script depends on both GameObjects being properly set.
            if (hideThisObject == null || spawnThisPrefab == null)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            spawnedObject = Instantiate(spawnThisPrefab, hideThisObject.transform.position, hideThisObject.transform.rotation);
            spawnedObject.SetActive(false);
            solverHandler = spawnedObject.GetComponent<SolverHandler>();
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (spawnedObject.activeSelf)
            {
                spawnedObject.SetActive(false);
                hideThisObject.SetActive(true);
            }
            else
            {
                spawnedObject.SetActive(true);

                if (updateSolverTargetToClickSource && solverHandler != null)
                {
                    InteractionInputSource interactionInputSource = eventData.InputSource as InteractionInputSource;

                    if (interactionInputSource != null)
                    {
                        InteractionSourceInfo sourceKind;
                        if (interactionInputSource.TryGetSourceKind(eventData.SourceId, out sourceKind))
                        {
                            switch (sourceKind)
                            {
                                case InteractionSourceInfo.Controller:
                                    Handedness handedness;
                                    if (interactionInputSource.TryGetHandedness(eventData.SourceId, out handedness))
                                    {
                                        if (handedness == Handedness.Right)
                                        {
                                            solverHandler.TrackedObjectToReference = SolverHandler.TrackedObjectToReferenceEnum.MotionControllerRight;
                                        }
                                        else
                                        {
                                            solverHandler.TrackedObjectToReference = SolverHandler.TrackedObjectToReferenceEnum.MotionControllerLeft;
                                        }
                                    }
                                    break;
                                default:
                                    Debug.LogError("The click event came from a device that isn't tracked. Nothing to attach to! Use a controller to select an example.");
                                    break;
                            }
                        }
                    }
                }

                hideThisObject.SetActive(false);
            }

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        private void OnDestroy()
        {
            Destroy(spawnedObject);
            Destroy(hideThisObject);
        }
    }
}
