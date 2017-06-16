// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// This script spawns a specific GameObject when a controller is detected
    /// and animates the controller position and rotation.
    /// </summary>
    [RequireComponent(typeof(SetGlobalListener))]
    public class ControllerVisualizer : MonoBehaviour, ISourceStateHandler, ISourceRotationHandler, ISourcePositionHandler
    {
        [Tooltip("Use a model with the tip in the positive Z direction and the front face in the positive Y direction.")]
        [SerializeField]
        private GameObject controllerModel;

        // This will be used to keep track of our controllers, indexed by its unique source ID.
        private Dictionary<uint, GameObject> controllerDictionary;

        private void Start()
        {
            if (controllerModel == null)
            {
                Debug.Log("Please make sure to add a controller model in the Inspector for ControllerVisualizer.cs on " + name + ".");
            }

            controllerDictionary = new Dictionary<uint, GameObject>();
        }

        /// <summary>
        /// When a controller is detected, the model is spawned and the controller object
        /// is added to the tracking dictionary.
        /// </summary>
        /// <param name="eventData">The source event data to be used to set up our controller model.</param>
        public void OnSourceDetected(SourceStateEventData eventData)
        {
            InteractionSourceKind sourceKind;
            if (controllerModel != null && eventData.InputSource.TryGetSourceKind(eventData.SourceId, out sourceKind) && sourceKind == InteractionSourceKind.Controller)
            {
                if (!controllerDictionary.ContainsKey(eventData.SourceId))
                {
                    GameObject controller = Instantiate(controllerModel);
                    GameObject parent = new GameObject();
                    controller.transform.parent = parent.transform;

                    controllerDictionary.Add(eventData.SourceId, parent);
                }
            }
        }

        /// <summary>
        /// When a controller is lost, the model is destroyed and the controller object
        /// is removed from the tracking dictionary.
        /// </summary>
        /// <param name="eventData">The source event data to be used to detect which controller to destroy.</param>
        public void OnSourceLost(SourceStateEventData eventData)
        {
            GameObject controller;
            if (controllerDictionary.TryGetValue(eventData.SourceId, out controller))
            {
                Destroy(controller);
                controllerDictionary.Remove(eventData.SourceId);
            }
        }

        public void OnRotationChanged(SourceRotationEventData eventData)
        {
            GameObject controller;
            if (controllerDictionary.TryGetValue(eventData.SourceId, out controller))
            {
                controller.transform.localRotation = eventData.Rotation;
            }
        }

        public void OnPositionChanged(SourcePositionEventData eventData)
        {
            GameObject controller;
            if (controllerDictionary.TryGetValue(eventData.SourceId, out controller))
            {
                controller.transform.localPosition = eventData.Position;
            }
        }
    }
}