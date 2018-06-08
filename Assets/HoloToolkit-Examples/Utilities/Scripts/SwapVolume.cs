// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class SwapVolume : MonoBehaviour, IInputClickHandler
    {
        [SerializeField]
        private GameObject hideThis;
        public GameObject HideThis
        {
            get
            {
                return hideThis;
            }

            set
            {
                hideThis = value;
            }
        }

        [SerializeField]
        private GameObject spawnThis;
        public GameObject SpawnThis
        {
            get
            {
                return spawnThis;
            }

            set
            {
                spawnThis = value;
            }
        }

        [SerializeField]
        private bool updateSolverTargetToClickSource = true;

        private SolverHandler solverHandler;
        private Vector3 defaultPosition;
        private Quaternion defaultRotation;
        private bool isOn = false;
        private GameObject spawnedObject;

        private void Start()
        {
            defaultPosition = HideThis.transform.position;
            defaultRotation = HideThis.transform.rotation;
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (isOn)
            {
                if (spawnedObject != null)
                {
                    Destroy(spawnedObject);
                }
                if (HideThis != null)
                {
                    HideThis.SetActive(true);
                }
            }
            else
            {
                spawnedObject = Instantiate(SpawnThis, defaultPosition, defaultRotation);

                if (updateSolverTargetToClickSource)
                {
                    solverHandler = spawnedObject.GetComponent<SolverHandler>();

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

                if (HideThis != null)
                {
                    HideThis.SetActive(false);
                }
            }

            isOn = !isOn;
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }
    }
}
