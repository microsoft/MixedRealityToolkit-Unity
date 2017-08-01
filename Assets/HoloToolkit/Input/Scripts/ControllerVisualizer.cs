// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR || UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

#if !UNITY_EDITOR && UNITY_WSA
using GLTF;
using System.Collections;
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
    public class ControllerVisualizer : MonoBehaviour
    {
        [Tooltip("Use a model with the tip in the positive Z direction and the front face in the positive Y direction. This will override the platform left controller model.")]
        [SerializeField]
        protected GameObject leftControllerOverride;
        [Tooltip("Use a model with the tip in the positive Z direction and the front face in the positive Y direction. This will override the platform right controller model.")]
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
                    spatialInteractionManager.SourceDetected += SpatialInteractionManager_SourceDetected;
                }
            }, true);

#elif UNITY_EDITOR
            // Since we're using non-Unity APIs, this will only run in a UWP app.
            Debug.Log("Running in the editor will only render the override models.");
            InteractionManager.OnSourceDetected += InteractionManager_OnSourceDetected;
#endif

#if UNITY_EDITOR || UNITY_WSA
            InteractionManager.OnSourceLost += InteractionManager_OnSourceLost;
            InteractionManager.OnSourceUpdated += InteractionManager_OnSourceUpdated;
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

        private IEnumerator LoadControllerModel(SpatialInteractionController controller, SpatialInteractionSource source)
        {
            bool isOverride;
            if (controllerDictionary != null && !controllerDictionary.ContainsKey(source.Id))
            {
                GameObject controllerModelGO;
                if (source.Handedness == SpatialInteractionSourceHandedness.Left && leftControllerOverride != null)
                {
                    controllerModelGO = Instantiate(leftControllerOverride);
                    isOverride = true;
                }
                else if (source.Handedness == SpatialInteractionSourceHandedness.Right && rightControllerOverride != null)
                {
                    controllerModelGO = Instantiate(rightControllerOverride);
                    isOverride = true;
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
                    isOverride = false;
                }

                FinishControllerSetup(controllerModelGO, isOverride, source.Handedness.ToString(), source.Id);
            }
        }
#endif

        private void InteractionManager_OnSourceDetected(SourceDetectedEventArgs obj)
        {
            if (obj.state.source.kind == InteractionSourceKind.Controller)
            {
                if (!controllerDictionary.ContainsKey(obj.state.source.id))
                {
                    GameObject controllerModelGO;
                    if (obj.state.handType == InteractionSourceHandType.Left && leftControllerOverride != null)
                    {
                        controllerModelGO = Instantiate(leftControllerOverride);
                    }
                    else if (obj.state.handType == InteractionSourceHandType.Right && rightControllerOverride != null)
                    {
                        controllerModelGO = Instantiate(rightControllerOverride);
                    }
                    else // InteractionSourceHandedness.Unknown || both overrides are null
                    {
                        return;
                    }

                    FinishControllerSetup(controllerModelGO, true, obj.state.handType.ToString(), obj.state.source.id);
                }
            }
        }

        /// <summary>
        /// When a controller is lost, the model is destroyed and the controller object
        /// is removed from the tracking dictionary.
        /// </summary>
        /// <param name="sender">The SpatialInteractionManager which sent this event.</param>
        /// <param name="args">The source event data to be used determine the controller model to be removed.</param>
        private void InteractionManager_OnSourceLost(SourceLostEventArgs obj)
        {
            InteractionSource source = obj.state.source;
            if (source.kind == InteractionSourceKind.Controller)
            {
                ControllerInfo controller;
                if (controllerDictionary != null && controllerDictionary.TryGetValue(source.id, out controller))
                {
                    Destroy(controller.gameObject);

                    // After destruction, the reference can be removed from the dictionary.
                    controllerDictionary.Remove(source.id);
                }
            }
        }

        private void InteractionManager_OnSourceUpdated(SourceUpdatedEventArgs obj)
        {
            ControllerInfo currentController;
            if (controllerDictionary != null && controllerDictionary.TryGetValue(obj.state.source.id, out currentController))
            {
                if (currentController.grasp != null && obj.state.supportsGrasp)
                {
                    if (obj.state.grasped != currentController.wasGrasped)
                    {
                        if (obj.state.grasped)
                        {
                            currentController.grasp.transform.localPosition = currentController.graspPressed.localPosition;
                            currentController.grasp.transform.localRotation = currentController.graspPressed.localRotation;
                        }
                        else
                        {
                            currentController.grasp.transform.localPosition = currentController.graspUnpressed.localPosition;
                            currentController.grasp.transform.localRotation = currentController.graspUnpressed.localRotation;
                        }
                        currentController.wasGrasped = obj.state.grasped;
                    }
                }

                if (currentController.menu != null && obj.state.supportsMenu)
                {
                    if (obj.state.menuPressed != currentController.wasMenuPressed)
                    {
                        if (obj.state.menuPressed)
                        {
                            currentController.menu.transform.localPosition = currentController.menuPressed.localPosition;
                            currentController.menu.transform.localRotation = currentController.menuPressed.localRotation;
                        }
                        else
                        {
                            currentController.menu.transform.localPosition = currentController.menuUnpressed.localPosition;
                            currentController.menu.transform.localRotation = currentController.menuUnpressed.localRotation;
                        }
                        currentController.wasMenuPressed = obj.state.menuPressed;
                    }
                }

                if (currentController.select != null)
                {
                    if (obj.state.selectPressedAmount != currentController.lastSelectPressedAmount)
                    {
                        currentController.select.transform.localPosition = Vector3.Lerp(currentController.selectUnpressed.localPosition, currentController.selectPressed.localPosition, (float)(obj.state.selectPressedAmount));
                        currentController.select.transform.localRotation = Quaternion.Lerp(currentController.selectUnpressed.localRotation, currentController.selectPressed.localRotation, (float)(obj.state.selectPressedAmount));
                        currentController.lastSelectPressedAmount = obj.state.selectPressedAmount;
                    }
                }

                if (currentController.thumbstickPress != null && obj.state.supportsThumbstick)
                {
                    if (obj.state.thumbstickPressed != currentController.wasThumbstickPressed)
                    {
                        if (obj.state.thumbstickPressed)
                        {
                            currentController.thumbstickPress.transform.localPosition = currentController.thumbstickPressed.localPosition;
                            currentController.thumbstickPress.transform.localRotation = currentController.thumbstickPressed.localRotation;
                        }
                        else
                        {
                            currentController.thumbstickPress.transform.localPosition = currentController.thumbstickUnpressed.localPosition;
                            currentController.thumbstickPress.transform.localRotation = currentController.thumbstickUnpressed.localRotation;
                        }
                        currentController.wasThumbstickPressed = obj.state.thumbstickPressed;
                    }
                }

                if(currentController.thumbstickX != null && currentController.thumbstickY != null && obj.state.supportsThumbstick)
                {
                    if (obj.state.thumbstickPosition != currentController.lastThumbstickPosition)
                    {
                        Vector2 thumbstickNormalized = (obj.state.thumbstickPosition + Vector2.one) / 2.0f;

                        currentController.thumbstickX.transform.localPosition = Vector3.Lerp(currentController.thumbstickXMin.localPosition, currentController.thumbstickXMax.localPosition, thumbstickNormalized.x);
                        currentController.thumbstickX.transform.localRotation = Quaternion.Lerp(currentController.thumbstickXMin.localRotation, currentController.thumbstickXMax.localRotation, thumbstickNormalized.x);

                        currentController.thumbstickY.transform.localPosition = Vector3.Lerp(currentController.thumbstickYMax.localPosition, currentController.thumbstickYMin.localPosition, thumbstickNormalized.y);
                        currentController.thumbstickY.transform.localRotation = Quaternion.Lerp(currentController.thumbstickYMax.localRotation, currentController.thumbstickYMin.localRotation, thumbstickNormalized.y);

                        currentController.lastThumbstickPosition = obj.state.thumbstickPosition;
                    }
                }

                if (currentController.touchpadPress != null && obj.state.supportsTouchpad)
                {
                    if (obj.state.touchpadPressed != currentController.wasTouchpadPressed)
                    {
                        if (obj.state.touchpadPressed)
                        {
                            currentController.touchpadPress.transform.localPosition = currentController.touchpadPressed.localPosition;
                            currentController.touchpadPress.transform.localRotation = currentController.touchpadPressed.localRotation;
                        }
                        else
                        {
                            currentController.touchpadPress.transform.localPosition = currentController.touchpadUnpressed.localPosition;
                            currentController.touchpadPress.transform.localRotation = currentController.touchpadUnpressed.localRotation;
                        }
                        currentController.wasTouchpadPressed = obj.state.touchpadPressed;
                    }
                }

                if (currentController.touchpadTouchX != null && currentController.touchpadTouchY != null && obj.state.supportsTouchpad)
                {
                    if (obj.state.touchpadTouched != currentController.wasTouchpadTouched)
                    {
                        if (obj.state.touchpadTouched)
                        {
                            currentController.touchpadTouchVisualizer.SetActive(true);
                        }
                        else
                        {
                            currentController.touchpadTouchVisualizer.SetActive(false);
                        }
                        currentController.wasTouchpadTouched = obj.state.touchpadTouched;
                    }

                    if (obj.state.touchpadPosition != currentController.lastTouchpadPosition)
                    {
                        Vector2 touchpadNormalized = (obj.state.touchpadPosition + Vector2.one) / 2.0f;

                        currentController.touchpadTouchX.transform.localPosition = Vector3.Lerp(currentController.touchpadTouchXMin.localPosition, currentController.touchpadTouchXMax.localPosition, touchpadNormalized.x);
                        currentController.touchpadTouchX.transform.localRotation = Quaternion.Lerp(currentController.touchpadTouchXMin.localRotation, currentController.touchpadTouchXMax.localRotation, touchpadNormalized.x);

                        currentController.touchpadTouchY.transform.localPosition = Vector3.Lerp(currentController.touchpadTouchYMax.localPosition, currentController.touchpadTouchYMin.localPosition, touchpadNormalized.y);
                        currentController.touchpadTouchY.transform.localRotation = Quaternion.Lerp(currentController.touchpadTouchYMax.localRotation, currentController.touchpadTouchYMin.localRotation, touchpadNormalized.y);

                        currentController.lastTouchpadPosition = obj.state.touchpadPosition;
                    }
                }

                Vector3 newPosition;
                if (obj.state.properties.location.grip.TryGetPosition(out newPosition) && newPosition != currentController.lastPosition)
                {
                    currentController.gameObject.transform.localPosition = newPosition;
                    currentController.lastPosition = newPosition;
                }

                Quaternion newRotation;
                if (obj.state.properties.location.grip.TryGetRotation(out newRotation) && newRotation != currentController.lastRotation)
                {
                    currentController.gameObject.transform.localRotation = newRotation;
                    currentController.lastRotation = newRotation;
                }
            }
        }

        private void FinishControllerSetup(GameObject controllerModelGO, bool isOverride, string handedness, uint id)
        {
            GameObject parentGameObject = new GameObject()
            {
                name = handedness + "Controller"
            };

            parentGameObject.transform.parent = transform;
            controllerModelGO.transform.parent = parentGameObject.transform;

            ControllerInfo newControllerInfo = parentGameObject.AddComponent<ControllerInfo>();
            if (!isOverride)
            {
                newControllerInfo.LoadInfo(controllerModelGO.GetComponentsInChildren<Transform>(), this);
            }
            controllerDictionary.Add(id, newControllerInfo);
        }

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
    }
}