// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR_WIN
using System;
using System.Runtime.InteropServices;
#endif

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using GLTF;
using System.Collections;
using UnityEngine.XR.WSA.Input;

#if !UNITY_EDITOR
using Windows.Foundation;
using Windows.Storage.Streams;
#endif
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// This script spawns a specific GameObject when a controller is detected
    /// and animates the controller position, rotation, button presses, and
    /// thumbstick/touchpad interactions, where applicable.
    /// </summary>
    public class MotionControllerVisualizer : MonoBehaviour
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

        // This will be used to keep track of our controllers, indexed by their unique source ID.
        private Dictionary<uint, MotionControllerInfo> controllerDictionary = new Dictionary<uint, MotionControllerInfo>(0);
        private List<uint> loadingControllers = new List<uint>();

#if UNITY_EDITOR_WIN
        [DllImport("MotionControllerModel")]
        private static extern bool TryGetMotionControllerModel([In] uint controllerId, [Out] out uint outputSize, [Out] out IntPtr outputBuffer);
#endif

        private void Awake()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            foreach (var sourceState in InteractionManager.GetCurrentReading())
            {
                if (sourceState.source.kind == InteractionSourceKind.Controller)
                {
                    StartTrackingController(sourceState.source);
                }
            }

            Application.onBeforeRender += Application_onBeforeRender;

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

            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
#endif
        }

        private void Update()
        {
            // NOTE: The controller's state is being updated here in order to provide a good position and rotation
            // for any child GameObjects that might want to raycast or otherwise reason about their location in the world.
            UpdateControllerState();
        }

        private void OnDestroy()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            InteractionManager.InteractionSourceDetected -= InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;
            Application.onBeforeRender -= Application_onBeforeRender;
#endif
        }

        private void Application_onBeforeRender()
        {
            // NOTE: This work is being done here to present the most correct rendered location of the controller each frame.
            // Any app logic depending on the controller state should happen in Update() or using InteractionManager's events.
            UpdateControllerState();
        }

        private void UpdateControllerState()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            foreach (var sourceState in InteractionManager.GetCurrentReading())
            {
                MotionControllerInfo currentController;
                if (sourceState.source.kind == InteractionSourceKind.Controller && controllerDictionary.TryGetValue(sourceState.source.id, out currentController))
                {
                    if (AnimateControllerModel)
                    {
                        currentController.AnimateSelect(sourceState.selectPressedAmount);

                        if (sourceState.source.supportsGrasp)
                        {
                            currentController.AnimateGrasp(sourceState.grasped);
                        }

                        if (sourceState.source.supportsMenu)
                        {
                            currentController.AnimateMenu(sourceState.menuPressed);
                        }

                        if (sourceState.source.supportsThumbstick)
                        {
                            currentController.AnimateThumbstick(sourceState.thumbstickPressed, sourceState.thumbstickPosition);
                        }

                        if (sourceState.source.supportsTouchpad)
                        {
                            currentController.AnimateTouchpad(sourceState.touchpadPressed, sourceState.touchpadTouched, sourceState.touchpadPosition);
                        }
                    }

                    Vector3 newPosition;
                    if (sourceState.sourcePose.TryGetPosition(out newPosition, InteractionSourceNode.Grip))
                    {
                        currentController.ControllerParent.transform.localPosition = newPosition;
                    }

                    Quaternion newRotation;
                    if (sourceState.sourcePose.TryGetRotation(out newRotation, InteractionSourceNode.Grip))
                    {
                        currentController.ControllerParent.transform.localRotation = newRotation;
                    }
                }
            }
#endif
        }

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs obj)
        {
            StartTrackingController(obj.state.source);
        }

        private void StartTrackingController(InteractionSource source)
        {
            if (source.kind == InteractionSourceKind.Controller && !controllerDictionary.ContainsKey(source.id) && !loadingControllers.Contains(source.id))
            {
                StartCoroutine(LoadControllerModel(source));
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
                MotionControllerInfo controller;
                if (controllerDictionary != null && controllerDictionary.TryGetValue(source.id, out controller))
                {
                    controllerDictionary.Remove(source.id);

                    Destroy(controller.ControllerParent);
                }
            }
        }

        private IEnumerator LoadControllerModel(InteractionSource source)
        {
            loadingControllers.Add(source.id);

            GameObject controllerModelGameObject;

            if (source.handedness == InteractionSourceHandedness.Left && LeftControllerOverride != null)
            {
                controllerModelGameObject = Instantiate(LeftControllerOverride);
            }
            else if (source.handedness == InteractionSourceHandedness.Right && RightControllerOverride != null)
            {
                controllerModelGameObject = Instantiate(RightControllerOverride);
            }
            else
            {
                byte[] fileBytes;

                if (GLTFMaterial == null)
                {
                    Debug.Log("If using glTF, please specify a material on " + name + ".");
                    loadingControllers.Remove(source.id);
                    yield break;
                }

#if !UNITY_EDITOR
                // This API returns the appropriate glTF file according to the motion controller you're currently using, if supported.
                IAsyncOperation<IRandomAccessStreamWithContentType> modelTask = source.TryGetRenderableModelAsync();

                if (modelTask == null)
                {
                    Debug.Log("Model task is null.");
                    loadingControllers.Remove(source.id);
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
                    loadingControllers.Remove(source.id);
                    yield break;
                }

                if (modelStream.Size == 0)
                {
                    Debug.Log("Model stream is empty.");
                    loadingControllers.Remove(source.id);
                    yield break;
                }

                fileBytes = new byte[modelStream.Size];

                using (DataReader reader = new DataReader(modelStream))
                {
                    DataReaderLoadOperation loadModelOp = reader.LoadAsync((uint)modelStream.Size);

                    while (loadModelOp.Status == AsyncStatus.Started)
                    {
                        yield return null;
                    }

                    reader.ReadBytes(fileBytes);
                }
#else
                IntPtr controllerModel = new IntPtr();
                uint outputSize = 0;

                if (TryGetMotionControllerModel(source.id, out outputSize, out controllerModel))
                {
                    fileBytes = new byte[Convert.ToInt32(outputSize)];

                    Marshal.Copy(controllerModel, fileBytes, 0, Convert.ToInt32(outputSize));
                }
                else
                {
                    Debug.Log("Unable to load controller models.");
                    loadingControllers.Remove(source.id);
                    yield break;
                }
#endif

                controllerModelGameObject = new GameObject();
                controllerModelGameObject.name = "glTFController";
                GLTFComponentStreamingAssets gltfScript = controllerModelGameObject.AddComponent<GLTFComponentStreamingAssets>();
                gltfScript.ColorMaterial = GLTFMaterial;
                gltfScript.NoColorMaterial = GLTFMaterial;
                gltfScript.GLTFData = fileBytes;

                yield return gltfScript.LoadModel();
            }

            FinishControllerSetup(controllerModelGameObject, source.handedness.ToString(), source.id);
        }
#endif

        private void FinishControllerSetup(GameObject controllerModelGameObject, string handedness, uint id)
        {
            var parentGameObject = new GameObject
            {
                name = handedness + "Controller"
            };

            parentGameObject.transform.parent = transform;
            controllerModelGameObject.transform.parent = parentGameObject.transform;

            var newControllerInfo = new MotionControllerInfo() { ControllerParent = parentGameObject };
            if (AnimateControllerModel)
            {
                newControllerInfo.LoadInfo(controllerModelGameObject.GetComponentsInChildren<Transform>(), this);
            }

            loadingControllers.Remove(id);
            controllerDictionary.Add(id, newControllerInfo);
        }

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
                touchVisualizer.GetComponent<Renderer>().sharedMaterial = GLTFMaterial;
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
