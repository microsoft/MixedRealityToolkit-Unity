// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#if !UNITY_EDITOR
using GLTF;
using System.Collections;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Input.Spatial;
#endif
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// This script spawns a specific GameObject when a controller is detected
    /// and animates the controller position, rotation, button presses, and
    /// thumbstick/touchpad interactions, where applicable.
    /// </summary>
    public class ControllerVisualizer : MonoBehaviour
    {
        [Tooltip("This setting will be used to determine if the model, override or otherwise, should attempt to be animated based on the user's input.")]
        public bool AnimateControllerModel = true;

        [Tooltip("Use a model with the tip in the positive Z direction and the front face in the positive Y direction. This will override the platform left controller model.")]
        [SerializeField]
        protected GameObject LeftControllerOverride;
        [Tooltip("Use a model with the tip in the positive Z direction and the front face in the positive Y direction. This will override the platform right controller model.")]
        [SerializeField]
        protected GameObject RightControllerOverride;
        [Tooltip("Use this to override the indicator used to show the user's touch location on the touchpad. Default is a sphere.")]
        [SerializeField]
        protected GameObject TouchpadTouchedOverride;

        [Tooltip("This material will be used on the loaded glTF controller model. This does not affect the above overrides.")]
        [SerializeField]
        protected UnityEngine.Material GLTFMaterial;

#if !UNITY_EDITOR && UNITY_WSA
        // This is used to get the renderable controller model, since Unity does not expose this API.
        private SpatialInteractionManager spatialInteractionManager;
#endif

        // This will be used to keep track of our controllers, indexed by their unique source ID.
        private Dictionary<uint, ControllerInfo> controllerDictionary;

        private void Start()
        {
            controllerDictionary = new Dictionary<uint, ControllerInfo>();

#if UNITY_WSA
#if !UNITY_EDITOR
            if (GLTFMaterial == null)
            {
                if (LeftControllerOverride == null && RightControllerOverride == null)
                {
                    Debug.Log("If using glTF, please specify a material on " + name + ". Otherwise, please specify controller overrides.");
                }
                else if (LeftControllerOverride == null || RightControllerOverride == null)
                {
                    Debug.Log("Only one override is specified, and no material is specified for the glTF model. Please set the material or the " + ((LeftControllerOverride == null) ? "left" : "right") + " controller override on " + name + ".");
                }
            }

            // Since the SpatialInteractionManager exists in the current CoreWindow, this call needs to run on the UI thread.
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                spatialInteractionManager = SpatialInteractionManager.GetForCurrentView();
                if (spatialInteractionManager != null)
                {
                    spatialInteractionManager.SourceDetected += SpatialInteractionManager_SourceDetected;
                    spatialInteractionManager.SourceLost += SpatialInteractionManager_SourceLost;
                }
            }, true);
#else
            // Since we're using non-Unity APIs, glTF will only load in a UWP app.
            if (LeftControllerOverride == null && RightControllerOverride == null)
            {
                Debug.Log("Running in the editor won't render the glTF models, and no controller overrides are set. Please specify them on " + name + ".");
            }
            else if (LeftControllerOverride == null || RightControllerOverride == null)
            {
                Debug.Log("Running in the editor won't render the glTF models, and only one controller override is specified. Please set the " + ((LeftControllerOverride == null) ? "left" : "right") + " override on " + name + ".");
            }

            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
#endif
            InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
#endif
        }

#if !UNITY_EDITOR && UNITY_WSA
        /// <summary>
        /// When a controller is detected, the model is spawned and the controller object
        /// is added to the tracking dictionary.
        /// </summary>
        /// <param name="sender">The SpatialInteractionManager which sent this event.</param>
        /// <param name="args">The source event data to be used to set up our controller model.</param>
        private void SpatialInteractionManager_SourceDetected(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            SpatialInteractionSource source = args.State.Source;
            // We only want to attempt loading a model if this source is actually a controller.
            if (source.Kind == SpatialInteractionSourceKind.Controller && controllerDictionary != null && !controllerDictionary.ContainsKey(source.Id))
            {
                SpatialInteractionController controller = source.Controller;
                if (controller != null)
                {
                    // Since this is a Unity call and will create a GameObject, this must run on Unity's app thread.
                    UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                    {
                        // LoadControllerModel is a coroutine in order to handle/wait for async calls.
                        StartCoroutine(LoadControllerModel(controller, source));
                    }, false);
                }
            }
        }

        private void SpatialInteractionManager_SourceLost(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            SpatialInteractionSource source = args.State.Source;
            if (source.Kind == SpatialInteractionSourceKind.Controller)
            {
                ControllerInfo controller;
                if (controllerDictionary != null && controllerDictionary.TryGetValue(source.Id, out controller))
                {
                    controllerDictionary.Remove(source.Id);

                    UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                    {
                        Destroy(controller);
                    }, false);
                }
            }
        }

        private IEnumerator LoadControllerModel(SpatialInteractionController controller, SpatialInteractionSource source)
        {
            GameObject controllerModelGameObject;
            if (source.Handedness == SpatialInteractionSourceHandedness.Left && LeftControllerOverride != null)
            {
                controllerModelGameObject = Instantiate(LeftControllerOverride);
            }
            else if (source.Handedness == SpatialInteractionSourceHandedness.Right && RightControllerOverride != null)
            {
                controllerModelGameObject = Instantiate(RightControllerOverride);
            }
            else
            {
                if (GLTFMaterial == null)
                {
                    Debug.Log("If using glTF, please specify a material on " + name + ".");
                    yield break;
                }

                // This API returns the appropriate glTF file according to the motion controller you're currently using, if supported.
                IAsyncOperation<IRandomAccessStreamWithContentType> modelTask = controller.TryGetRenderableModelAsync();

                if (modelTask == null)
                {
                    Debug.Log("Model task is null.");
                    yield break;
                }

                while (modelTask.Status == AsyncStatus.Started)
                {
                    yield return null;
                }

                IRandomAccessStreamWithContentType modelStream = modelTask.GetResults();

                if (modelStream == null)
                {
                    Debug.Log("Model stream is null.");
                    yield break;
                }

                if (modelStream.Size == 0)
                {
                    Debug.Log("Model stream is empty.");
                    yield break;
                }

                byte[] fileBytes = new byte[modelStream.Size];

                using (DataReader reader = new DataReader(modelStream))
                {
                    DataReaderLoadOperation loadModelOp = reader.LoadAsync((uint)modelStream.Size);

                    while (loadModelOp.Status == AsyncStatus.Started)
                    {
                        yield return null;
                    }

                    reader.ReadBytes(fileBytes);
                }

                controllerModelGameObject = new GameObject();
                GLTFComponentStreamingAssets gltfScript = controllerModelGameObject.AddComponent<GLTFComponentStreamingAssets>();
                gltfScript.ColorMaterial = GLTFMaterial;
                gltfScript.NoColorMaterial = GLTFMaterial;
                gltfScript.GLTFData = fileBytes;

                yield return gltfScript.LoadModel();
            }

            FinishControllerSetup(controllerModelGameObject, source.Handedness.ToString(), source.Id);
        }
#endif

#if UNITY_WSA
        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs obj)
        {
            if (obj.state.source.kind == InteractionSourceKind.Controller && controllerDictionary != null && !controllerDictionary.ContainsKey(obj.state.source.id))
            {
                GameObject controllerModelGameObject;
                if (obj.state.source.handedness == InteractionSourceHandedness.Left && LeftControllerOverride != null)
                {
                    controllerModelGameObject = Instantiate(LeftControllerOverride);
                }
                else if (obj.state.source.handedness == InteractionSourceHandedness.Right && RightControllerOverride != null)
                {
                    controllerModelGameObject = Instantiate(RightControllerOverride);
                }
                else // InteractionSourceHandedness.Unknown || both overrides are null
                {
                    return;
                }

                FinishControllerSetup(controllerModelGameObject, obj.state.source.handedness.ToString(), obj.state.source.id);
            }
        }

        /// <summary>
        /// When a controller is lost, the model is destroyed and the controller object
        /// is removed from the tracking dictionary.
        /// </summary>
        /// <param name="obj">The source event args to be used to determine the controller model to be removed.</param>
        private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs obj)
        {
            InteractionSource source = obj.state.source;
            if (source.kind == InteractionSourceKind.Controller)
            {
                ControllerInfo controller;
                if (controllerDictionary != null && controllerDictionary.TryGetValue(source.id, out controller))
                {
                    controllerDictionary.Remove(source.id);

                    Destroy(controller);
                }
            }
        }

        private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs obj)
        {
            ControllerInfo currentController;
            if (AnimateControllerModel && controllerDictionary != null && controllerDictionary.TryGetValue(obj.state.source.id, out currentController))
            {
                currentController.AnimateSelect(obj.state.selectPressedAmount);

                if (obj.state.source.supportsGrasp)
                {
                    currentController.AnimateGrasp(obj.state.grasped);
                }

                if (obj.state.source.supportsMenu)
                {
                    currentController.AnimateMenu(obj.state.menuPressed);
                }

                if (obj.state.source.supportsThumbstick)
                {
                    currentController.AnimateThumbstick(obj.state.thumbstickPressed, obj.state.thumbstickPosition);
                }

                if (obj.state.source.supportsTouchpad)
                {
                    currentController.AnimateTouchpad(obj.state.touchpadPressed, obj.state.touchpadTouched, obj.state.touchpadPosition);
                }

                Vector3 newPosition;
                if (obj.state.sourcePose.TryGetPosition(out newPosition, InteractionSourceNode.Grip))
                {
                    currentController.gameObject.transform.localPosition = newPosition;
                }

                Quaternion newRotation;
                if (obj.state.sourcePose.TryGetRotation(out newRotation, InteractionSourceNode.Grip))
                {
                    currentController.gameObject.transform.localRotation = newRotation;
                }
            }
        }

        private void FinishControllerSetup(GameObject controllerModelGameObject, string handedness, uint id)
        {
            var parentGameObject = new GameObject
            {
                name = handedness + "Controller"
            };

            parentGameObject.transform.parent = transform;
            controllerModelGameObject.transform.parent = parentGameObject.transform;

            var newControllerInfo = parentGameObject.AddComponent<ControllerInfo>();
            if (AnimateControllerModel)
            {
                newControllerInfo.LoadInfo(controllerModelGameObject.GetComponentsInChildren<Transform>(), this);
            }
            controllerDictionary.Add(id, newControllerInfo);
        }
#endif

        public GameObject SpawnTouchpadVisualizer(Transform parentTransform)
        {
            GameObject touchVisualizer;
            if (TouchpadTouchedOverride != null)
            {
                touchVisualizer = Instantiate(TouchpadTouchedOverride);
            }
            else
            {
                touchVisualizer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                touchVisualizer.transform.localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);
                touchVisualizer.GetComponent<Renderer>().material = GLTFMaterial;
            }
            Destroy(touchVisualizer.GetComponent<Collider>());
            touchVisualizer.transform.parent = parentTransform;
            touchVisualizer.transform.localPosition = Vector3.zero;
            touchVisualizer.transform.localRotation = Quaternion.identity;
            touchVisualizer.SetActive(false);
            return touchVisualizer;
        }
    }
}
