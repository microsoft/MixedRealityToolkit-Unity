// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using GLTF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_EDITOR && UNITY_WSA
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Input.Spatial;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// This script spawns a specific GameObject when a controller is detected
    /// and animates the controller position, rotation, button presses, and
    /// thumbstick/touchpad interactions, where applicable.
    /// </summary>
    [RequireComponent(typeof(SetGlobalListener))]
    public class ControllerVisualizer : MonoBehaviour, ISourcePositionHandler, ISourceRotationHandler
    {
        [Tooltip("Use a model with the tip in the positive Z direction and the front face in the positive Y direction.")]
        [SerializeField]
        protected GameObject leftControllerOverride;
        [Tooltip("Use a model with the tip in the positive Z direction and the front face in the positive Y direction.")]
        [SerializeField]
        protected GameObject rightControllerOverride;
        [Tooltip("Use this to override the indicator used to show the user's touch location on the touchpad. Default is a sphere.")]
        [SerializeField]
        protected GameObject touchpadTouchedOverride;

        [Tooltip("This shader will be used on the loaded GLTF controller model. This does not affect the above overrides.")]
        public Shader GLTFShader;

#if !UNITY_EDITOR && UNITY_WSA
        // This is used to get the renderable controller model, since Unity does not expose this API.
        private SpatialInteractionManager spatialInteractionManager;
#endif

        // This will be used to keep track of our controllers, indexed by their unique source ID.
        private Dictionary<uint, ControllerInfo> controllerDictionary;

        private void Start()
        {
            controllerDictionary = new Dictionary<uint, ControllerInfo>();

#if !UNITY_EDITOR && UNITY_WSA
            // Since the SpatialInteractionManager exists in the current CoreWindow, this call needs to run on the UI thread.
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                spatialInteractionManager = SpatialInteractionManager.GetForCurrentView();
                if (spatialInteractionManager != null)
                {
                    spatialInteractionManager.SourceDetected += ControllerVisualizer_SourceDetected;
                    spatialInteractionManager.SourceLost += ControllerVisualizer_SourceLost;

                    spatialInteractionManager.SourceUpdated += ControllerVisualizer_SourceUpdated;
                }
            }, true);
#else
            // Since we're using non-Unity APIs, this will only run in a UWP app.
            Debug.Log("ControllerVisualizer only works in UWP.");
#endif
        }

#if !UNITY_EDITOR && UNITY_WSA
        /// <summary>
        /// When a controller is detected, the model is spawned and the controller object
        /// is added to the tracking dictionary.
        /// </summary>
        /// <param name="sender">The SpatialInteractionManager which sent this event.</param>
        /// <param name="args">The source event data to be used to set up our controller model.</param>
        private void ControllerVisualizer_SourceDetected(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            SpatialInteractionSource source = args.State.Source;
            // We only want to attempt loading a model if this source is actually a controller.
            if (source.Kind == SpatialInteractionSourceKind.Controller)
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

        /// <summary>
        /// When a controller is lost, the model is destroyed and the controller object
        /// is removed from the tracking dictionary.
        /// </summary>
        /// <param name="sender">The SpatialInteractionManager which sent this event.</param>
        /// <param name="args">The source event data to be used determine the controller model to be removed.</param>
        private void ControllerVisualizer_SourceLost(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            SpatialInteractionSource source = args.State.Source;
            if (source.Kind == SpatialInteractionSourceKind.Controller)
            {
                ControllerInfo controller;
                if (controllerDictionary != null && controllerDictionary.TryGetValue(source.Id, out controller))
                {
                    // Referencing a GameObject must happen on Unity's app thread.
                    UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                    {
                        Destroy(controller.gameObject);
                    }, false);

                    // After destruction, the reference can be removed from the dictionary.
                    controllerDictionary.Remove(source.Id);
                }
            }
        }

        private void ControllerVisualizer_SourceUpdated(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            // Referencing a GameObject must happen on Unity's app thread.
            UnityEngine.WSA.Application.InvokeOnAppThread(() =>
            {
                ControllerInfo currentController;
                if (controllerDictionary != null && controllerDictionary.TryGetValue(args.State.Source.Id, out currentController))
                {
                    if (currentController.grasp != null && args.State.Source.IsGraspSupported)
                    {
                        if (args.State.IsGrasped != currentController.wasGrasped)
                        {
                            if (args.State.IsGrasped)
                            {
                                currentController.grasp.transform.localPosition = currentController.graspPressed.localPosition;
                                currentController.grasp.transform.localRotation = currentController.graspPressed.localRotation;
                            }
                            else
                            {
                                currentController.grasp.transform.localPosition = currentController.graspUnpressed.localPosition;
                                currentController.grasp.transform.localRotation = currentController.graspUnpressed.localRotation;
                            }
                            currentController.wasGrasped = args.State.IsGrasped;
                        }
                    }

                    if (currentController.menu != null && args.State.Source.IsMenuSupported)
                    {
                        if (args.State.IsMenuPressed != currentController.wasMenuPressed)
                        {
                            if (args.State.IsMenuPressed)
                            {
                                currentController.menu.transform.localPosition = currentController.menuPressed.localPosition;
                                currentController.menu.transform.localRotation = currentController.menuPressed.localRotation;
                            }
                            else
                            {
                                currentController.menu.transform.localPosition = currentController.menuUnpressed.localPosition;
                                currentController.menu.transform.localRotation = currentController.menuUnpressed.localRotation;
                            }
                            currentController.wasMenuPressed = args.State.IsMenuPressed;
                        }
                    }

                    if (currentController.select != null)
                    {
                        if (args.State.SelectPressedValue != currentController.lastSelectPressedValue)
                        {
                            currentController.select.transform.localPosition = Vector3.Lerp(currentController.selectUnpressed.localPosition, currentController.selectPressed.localPosition, (float)(args.State.SelectPressedValue));
                            currentController.select.transform.localRotation = Quaternion.Lerp(currentController.selectUnpressed.localRotation, currentController.selectPressed.localRotation, (float)(args.State.SelectPressedValue));
                            currentController.lastSelectPressedValue = args.State.SelectPressedValue;
                        }
                    }

                    if (currentController.thumbstickPress != null && currentController.thumbstickX != null && currentController.thumbstickY != null && args.State.Source.Controller.HasThumbstick)
                    {
                        if (args.State.ControllerProperties.IsThumbstickPressed != currentController.wasThumbstickPressed)
                        {
                            if (args.State.ControllerProperties.IsThumbstickPressed)
                            {
                                currentController.thumbstickPress.transform.localPosition = currentController.thumbstickPressed.localPosition;
                                currentController.thumbstickPress.transform.localRotation = currentController.thumbstickPressed.localRotation;
                            }
                            else
                            {
                                currentController.thumbstickPress.transform.localPosition = currentController.thumbstickUnpressed.localPosition;
                                currentController.thumbstickPress.transform.localRotation = currentController.thumbstickUnpressed.localRotation;
                            }
                            currentController.wasThumbstickPressed = args.State.ControllerProperties.IsThumbstickPressed;
                        }

                        if (args.State.ControllerProperties.ThumbstickX != currentController.lastThumbstickX)
                        {
                            float thumbstickXNormalized = (float)((args.State.ControllerProperties.ThumbstickX + 1) / 2.0f);
                            currentController.thumbstickX.transform.localPosition = Vector3.Lerp(currentController.thumbstickXMin.localPosition, currentController.thumbstickXMax.localPosition, thumbstickXNormalized);
                            currentController.thumbstickX.transform.localRotation = Quaternion.Lerp(currentController.thumbstickXMin.localRotation, currentController.thumbstickXMax.localRotation, thumbstickXNormalized);
                            currentController.lastThumbstickX = args.State.ControllerProperties.ThumbstickX;
                        }

                        if (args.State.ControllerProperties.ThumbstickY != currentController.lastThumbstickY)
                        {
                            float thumbstickYNormalized = (float)((args.State.ControllerProperties.ThumbstickY + 1) / 2.0f);
                            currentController.thumbstickY.transform.localPosition = Vector3.Lerp(currentController.thumbstickYMax.localPosition, currentController.thumbstickYMin.localPosition, thumbstickYNormalized);
                            currentController.thumbstickY.transform.localRotation = Quaternion.Lerp(currentController.thumbstickYMax.localRotation, currentController.thumbstickYMin.localRotation, thumbstickYNormalized);
                            currentController.lastThumbstickY = args.State.ControllerProperties.ThumbstickY;
                        }
                    }

                    if (currentController.touchpadPress != null && args.State.Source.Controller.HasTouchpad)
                    {
                        if (args.State.ControllerProperties.IsTouchpadPressed != currentController.wasTouchpadPressed)
                        {
                            if (args.State.ControllerProperties.IsTouchpadPressed)
                            {
                                currentController.touchpadPress.transform.localPosition = currentController.touchpadPressed.localPosition;
                                currentController.touchpadPress.transform.localRotation = currentController.touchpadPressed.localRotation;
                            }
                            else
                            {
                                currentController.touchpadPress.transform.localPosition = currentController.touchpadUnpressed.localPosition;
                                currentController.touchpadPress.transform.localRotation = currentController.touchpadUnpressed.localRotation;
                            }
                            currentController.wasTouchpadPressed = args.State.ControllerProperties.IsTouchpadPressed;
                        }
                    }

                    if (currentController.touchpadTouchX != null && currentController.touchpadTouchY != null && args.State.Source.Controller.HasTouchpad)
                    {
                        if (args.State.ControllerProperties.IsTouchpadTouched != currentController.wasTouchpadTouched)
                        {
                            if (args.State.ControllerProperties.IsTouchpadTouched)
                            {
                                currentController.touchpadTouchVisualizer.SetActive(true);
                            }
                            else
                            {
                                currentController.touchpadTouchVisualizer.SetActive(false);
                            }
                            currentController.wasTouchpadTouched = args.State.ControllerProperties.IsTouchpadTouched;
                        }

                        if (args.State.ControllerProperties.TouchpadX != currentController.lastTouchpadX)
                        {
                            float touchpadXNormalized = (float)((args.State.ControllerProperties.TouchpadX + 1) / 2.0f);
                            currentController.touchpadTouchX.transform.localPosition = Vector3.Lerp(currentController.touchpadTouchXMin.localPosition, currentController.touchpadTouchXMax.localPosition, touchpadXNormalized);
                            currentController.touchpadTouchX.transform.localRotation = Quaternion.Lerp(currentController.touchpadTouchXMin.localRotation, currentController.touchpadTouchXMax.localRotation, touchpadXNormalized);
                            currentController.lastTouchpadX = args.State.ControllerProperties.TouchpadX;
                        }

                        if (args.State.ControllerProperties.TouchpadY != currentController.lastTouchpadY)
                        {
                            float touchpadYNormalized = (float)((args.State.ControllerProperties.TouchpadY + 1) / 2.0f);
                            currentController.touchpadTouchY.transform.localPosition = Vector3.Lerp(currentController.touchpadTouchYMax.localPosition, currentController.touchpadTouchYMin.localPosition, touchpadYNormalized);
                            currentController.touchpadTouchY.transform.localRotation = Quaternion.Lerp(currentController.touchpadTouchYMax.localRotation, currentController.touchpadTouchYMin.localRotation, touchpadYNormalized);
                            currentController.lastTouchpadY = args.State.ControllerProperties.TouchpadY;
                        }
                    }
                }
            }, false);
        }

        private IEnumerator LoadControllerModel(SpatialInteractionController controller, SpatialInteractionSource source)
        {
            if (controllerDictionary != null && !controllerDictionary.ContainsKey(source.Id))
            {
                GameObject controllerModelGO;
                if (source.Handedness == SpatialInteractionSourceHandedness.Left && leftControllerOverride != null)
                {
                    controllerModelGO = Instantiate(leftControllerOverride);
                }
                else if (source.Handedness == SpatialInteractionSourceHandedness.Right && rightControllerOverride != null)
                {
                    controllerModelGO = Instantiate(rightControllerOverride);
                }
                else
                {
                    if (GLTFShader == null)
                    {
                        Debug.Log("If using glTF, please specify a shader on " + name + ".");
                        yield break;
                    }

                    // This API returns the appropriate GLTF file according to the motion controller you're currently using, if supported.
                    IAsyncOperation<IRandomAccessStreamWithContentType> modelTask = controller.TryGetRenderableModelAsync();

                    if (modelTask == null)
                    {
                        yield break;
                    }

                    while (modelTask.Status == AsyncStatus.Started)
                    {
                        yield return null;
                    }

                    IRandomAccessStreamWithContentType modelStream = modelTask.GetResults();

                    if (modelStream == null)
                    {
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

                    controllerModelGO = new GameObject();
                    GLTFComponent gltfScript = controllerModelGO.AddComponent<GLTFComponent>();
                    gltfScript.GLTFStandard = GLTFShader;
                    gltfScript.SetData(fileBytes);
                    yield return gltfScript.LoadModel();
                }

                GameObject parent = new GameObject()
                {
                    name = source.Handedness + "Controller"
                };

                parent.transform.SetParent(transform);
                controllerModelGO.transform.SetParent(parent.transform);

                ControllerInfo newControllerInfo = parent.AddComponent<ControllerInfo>();
                newControllerInfo.LoadInfo(controllerModelGO.GetComponentsInChildren<Transform>(), this);

                controllerDictionary.Add(source.Id, newControllerInfo);
            }
        }
#endif

        public GameObject SpawnTouchpadVisualizer(Transform parentTransform)
        {
            GameObject touchVisualizer;
            if (touchpadTouchedOverride != null)
            {
                touchVisualizer = Instantiate(touchpadTouchedOverride);
            }
            else
            {
                touchVisualizer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                touchVisualizer.transform.localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);
                touchVisualizer.GetComponent<Renderer>().material.shader = GLTFShader;
            }
            Destroy(touchVisualizer.GetComponent<Collider>());
            touchVisualizer.transform.parent = parentTransform;
            touchVisualizer.transform.localPosition = Vector3.zero;
            touchVisualizer.transform.localRotation = Quaternion.identity;
            touchVisualizer.SetActive(false);
            return touchVisualizer;
        }

        public void OnRotationChanged(SourceRotationEventData eventData)
        {
            ControllerInfo controller;
            if (controllerDictionary != null && controllerDictionary.TryGetValue(eventData.SourceId, out controller))
            {
                controller.gameObject.transform.localRotation = eventData.Rotation;
            }
        }

        public void OnPositionChanged(SourcePositionEventData eventData)
        {
            ControllerInfo controller;
            if (controllerDictionary != null && controllerDictionary.TryGetValue(eventData.SourceId, out controller))
            {
                controller.gameObject.transform.localPosition = eventData.Position;
            }
        }
    }
}