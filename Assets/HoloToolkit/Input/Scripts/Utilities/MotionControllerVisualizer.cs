// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR_WIN
using System.Runtime.InteropServices;
#endif

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using System.Collections;
using System.IO;
using UnityEngine.XR.WSA.Input;
using UnityGLTF;

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
    public class MotionControllerVisualizer : Singleton<MotionControllerVisualizer>
    {
        [Tooltip("This setting will be used to determine if the model, override or otherwise, should attempt to be animated based on the user's input.")]
        public bool AnimateControllerModel = true;

        [Tooltip("This setting will be used to determine if the model should always be the left alternate. If false, the platform controller models will be preferred, only if they can't be loaded will the alternate be used. Otherwise, it will always use the alternate model.")]
        public bool AlwaysUseAlternateLeftModel = false;
        [Tooltip("This setting will be used to determine if the model should always be the right alternate. If false, the platform controller models will be preferred, only if they can't be loaded will the alternate be used. Otherwise, it will always use the alternate model.")]
        public bool AlwaysUseAlternateRightModel = false;

        [Tooltip("Use a model with the tip in the positive Z direction and the front face in the positive Y direction. To override the platform left controller model set AlwaysUseAlternateModel to true; otherwise this will be the default if the model can't be found.")]
        [SerializeField]
        protected GameObject AlternateLeftController;
        [Tooltip("Use a model with the tip in the positive Z direction and the front face in the positive Y direction. To override the platform right controller model set AlwaysUseAlternateModel to true; otherwise this will be the default if the model can't be found.")]
        [SerializeField]
        protected GameObject AlternateRightController;
        [Tooltip("Use this to override the indicator used to show the user's touch location on the touchpad. Default is a sphere.")]
        [SerializeField]
        protected GameObject TouchpadTouchedOverride;

        [Tooltip("This material will be used on the loaded glTF controller model. This does not affect the above overrides.")]
        [SerializeField]
        protected UnityEngine.Material GLTFMaterial;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        // This will be used to keep track of our controllers, indexed by their unique source ID.
        private Dictionary<string, MotionControllerInfo> controllerDictionary = new Dictionary<string, MotionControllerInfo>(0);
        private List<string> loadingControllers = new List<string>();

        private MotionControllerInfo leftControllerModel;
        private MotionControllerInfo rightControllerModel;

        public event Action<MotionControllerInfo> OnControllerModelLoaded;
        public event Action<MotionControllerInfo> OnControllerModelUnloaded;

        private bool leftModelIsAlternate = false;
        private bool rightModelIsAlternate = false;
#endif

#if UNITY_EDITOR_WIN
        [DllImport("EditorMotionController")]
        private static extern bool TryGetMotionControllerModel([In] uint controllerId, [Out] out uint outputSize, [Out] out IntPtr outputBuffer);
#endif

        protected override void Awake()
        {
            base.Awake();

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            Application.onBeforeRender += Application_onBeforeRender;

            if (GLTFMaterial == null)
            {
                if (AlternateLeftController == null && AlternateRightController == null)
                {
                    Debug.Log("If using glTF, please specify a material on " + name + ". Otherwise, please specify controller alternates.");
                }
                else if (AlternateLeftController == null || AlternateRightController == null)
                {
                    Debug.Log("Only one alternate is specified, and no material is specified for the glTF model. Please set the material or the " + ((AlternateLeftController == null) ? "left" : "right") + " controller alternate on " + name + ".");
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

        protected override void OnDestroy()
        {
            base.OnDestroy();

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            InteractionManager.InteractionSourceDetected -= InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;
            Application.onBeforeRender -= Application_onBeforeRender;

            foreach (MotionControllerInfo controllerInfo in controllerDictionary.Values)
            {
                Destroy(controllerInfo.ControllerParent);
            }
#endif
        }

        private void Application_onBeforeRender()
        {
            // NOTE: This work is being done here to present the most correct rendered location of the controller each frame.
            // Any app logic depending on the controller state should happen in Update() or using InteractionManager's events.
            // We don't want to potentially start loading a new controller model in this call, since onBeforeRender shouldn't
            // do much work for performance reasons.
            UpdateControllerState(false);
        }

        private void UpdateControllerState(bool createNewControllerIfNeeded = true)
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            foreach (var sourceState in InteractionManager.GetCurrentReading())
            {
                if (sourceState.source.kind != InteractionSourceKind.Controller)
                {
                    continue;
                }

                string key = GenerateKey(sourceState.source);
                if (createNewControllerIfNeeded && !controllerDictionary.ContainsKey(key) && !loadingControllers.Contains(key))
                {
                    StartTrackingController(sourceState.source);
                }

                MotionControllerInfo currentController;
                if (controllerDictionary.TryGetValue(key, out currentController))
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
                    if (sourceState.sourcePose.TryGetPosition(out newPosition, InteractionSourceNode.Grip) && ValidPosition(newPosition))
                    {
                        currentController.ControllerParent.transform.localPosition = newPosition;
                    }

                    Quaternion newRotation;
                    if (sourceState.sourcePose.TryGetRotation(out newRotation, InteractionSourceNode.Grip) && ValidRotation(newRotation))
                    {
                        currentController.ControllerParent.transform.localRotation = newRotation;
                    }
                }
            }
#endif
        }

        private bool ValidRotation(Quaternion newRotation)
        {
            return !float.IsNaN(newRotation.x) && !float.IsNaN(newRotation.y) && !float.IsNaN(newRotation.z) && !float.IsNaN(newRotation.w) &&
                !float.IsInfinity(newRotation.x) && !float.IsInfinity(newRotation.y) && !float.IsInfinity(newRotation.z) && !float.IsInfinity(newRotation.w);
        }

        private bool ValidPosition(Vector3 newPosition)
        {
            return !float.IsNaN(newPosition.x) && !float.IsNaN(newPosition.y) && !float.IsNaN(newPosition.z) &&
                !float.IsInfinity(newPosition.x) && !float.IsInfinity(newPosition.y) && !float.IsInfinity(newPosition.z);
        }

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs obj)
        {
            StartTrackingController(obj.state.source);
        }

        private void StartTrackingController(InteractionSource source)
        {
            string key = GenerateKey(source);

            MotionControllerInfo controllerInfo;
            if (source.kind == InteractionSourceKind.Controller)
            {
                if (!controllerDictionary.ContainsKey(key) && !loadingControllers.Contains(key))
                {
                    StartCoroutine(LoadControllerModel(source));
                }
                else if (controllerDictionary.TryGetValue(key, out controllerInfo))
                {
                    if (controllerInfo.Handedness == InteractionSourceHandedness.Left)
                    {
                        leftControllerModel = controllerInfo;
                    }
                    else if (controllerInfo.Handedness == InteractionSourceHandedness.Right)
                    {
                        rightControllerModel = controllerInfo;
                    }

                    controllerInfo.ControllerParent.SetActive(true);

                    if (OnControllerModelLoaded != null)
                    {
                        OnControllerModelLoaded(controllerInfo);
                    }
                }
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
                MotionControllerInfo controllerInfo;
                string key = GenerateKey(source);
                if (controllerDictionary != null && controllerDictionary.TryGetValue(key, out controllerInfo))
                {
                    if (OnControllerModelUnloaded != null)
                    {
                        OnControllerModelUnloaded(controllerInfo);
                    }

                    if (controllerInfo.Handedness == InteractionSourceHandedness.Left)
                    {
                        leftControllerModel = null;

                        if (!AlwaysUseAlternateLeftModel && leftModelIsAlternate)
                        {
                            controllerDictionary.Remove(key);
                            Destroy(controllerInfo.ControllerParent);
                        }
                    }
                    else if (controllerInfo.Handedness == InteractionSourceHandedness.Right)
                    {
                        rightControllerModel = null;

                        if (!AlwaysUseAlternateRightModel && rightModelIsAlternate)
                        {
                            controllerDictionary.Remove(key);
                            Destroy(controllerInfo.ControllerParent);
                        }
                    }

                    controllerInfo.ControllerParent.SetActive(false);
                }
            }
        }

        private IEnumerator LoadControllerModel(InteractionSource source)
        {
            loadingControllers.Add(GenerateKey(source));

            if (AlwaysUseAlternateLeftModel && source.handedness == InteractionSourceHandedness.Left)
            {
                if (AlternateLeftController == null)
                {
                    Debug.LogWarning("Always use the alternate left model is set on " + name + ", but the alternate left controller model was not specified.");
                    yield return LoadSourceControllerModel(source);
                }
                else
                {
                    LoadAlternateControllerModel(source);
                }
            }
            else if (AlwaysUseAlternateRightModel && source.handedness == InteractionSourceHandedness.Right)
            {
                if (AlternateRightController == null)
                {
                    Debug.LogWarning("Always use the alternate right model is set on " + name + ", but the alternate right controller model was not specified.");
                    yield return LoadSourceControllerModel(source);
                }
                else
                {
                    LoadAlternateControllerModel(source);
                }
            }
            else
            {
                yield return LoadSourceControllerModel(source);
            }
        }

        private IEnumerator LoadSourceControllerModel(InteractionSource source)
        {
            byte[] fileBytes;
            GameObject controllerModelGameObject;

            if (GLTFMaterial == null)
            {
                Debug.Log("If using glTF, please specify a material on " + name + ".");
                yield break;
            }

#if !UNITY_EDITOR
            // This API returns the appropriate glTF file according to the motion controller you're currently using, if supported.
            IAsyncOperation<IRandomAccessStreamWithContentType> modelTask = source.TryGetRenderableModelAsync();

            if (modelTask == null)
            {
                Debug.Log("Model task is null; loading alternate.");
                LoadAlternateControllerModel(source);
                yield break;
            }

            while (modelTask.Status == AsyncStatus.Started)
            {
                yield return null;
            }

            IRandomAccessStreamWithContentType modelStream = modelTask.GetResults();

            if (modelStream == null)
            {
                Debug.Log("Model stream is null; loading alternate.");
                LoadAlternateControllerModel(source);
                yield break;
            }

            if (modelStream.Size == 0)
            {
                Debug.Log("Model stream is empty; loading alternate.");
                LoadAlternateControllerModel(source);
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
                Debug.Log("Unable to load controller models; loading alternate.");
                LoadAlternateControllerModel(source);
                yield break;
            }
#endif

            controllerModelGameObject = new GameObject { name = "glTFController" };
            controllerModelGameObject.transform.Rotate(0, 180, 0);

            var sceneImporter = new GLTFSceneImporter(
                "",
                new MemoryStream(fileBytes, 0, fileBytes.Length, false, true),
                controllerModelGameObject.transform
                );

            sceneImporter.SetShaderForMaterialType(GLTFSceneImporter.MaterialType.PbrMetallicRoughness, GLTFMaterial.shader);
            sceneImporter.SetShaderForMaterialType(GLTFSceneImporter.MaterialType.KHR_materials_pbrSpecularGlossiness, GLTFMaterial.shader);
            sceneImporter.SetShaderForMaterialType(GLTFSceneImporter.MaterialType.CommonConstant, GLTFMaterial.shader);

            yield return sceneImporter.Load();

            if (source.handedness == InteractionSourceHandedness.Left)
            {
                leftModelIsAlternate = false;
            }
            else if (source.handedness == InteractionSourceHandedness.Right)
            {
                rightModelIsAlternate = false;
            }

            FinishControllerSetup(controllerModelGameObject, source.handedness, GenerateKey(source));
        }

        private void LoadAlternateControllerModel(InteractionSource source)
        {
            GameObject controllerModelGameObject;
            if (source.handedness == InteractionSourceHandedness.Left && AlternateLeftController != null)
            {
                controllerModelGameObject = Instantiate(AlternateLeftController);
                leftModelIsAlternate = true;
            }
            else if (source.handedness == InteractionSourceHandedness.Right && AlternateRightController != null)
            {
                controllerModelGameObject = Instantiate(AlternateRightController);
                rightModelIsAlternate = true;
            }
            else
            {
                loadingControllers.Remove(GenerateKey(source));
                return;
            }

            FinishControllerSetup(controllerModelGameObject, source.handedness, GenerateKey(source));
        }

        private string GenerateKey(InteractionSource source)
        {
            return source.vendorId + "/" + source.productId + "/" + source.productVersion + "/" + source.handedness;
        }

        private void FinishControllerSetup(GameObject controllerModelGameObject, InteractionSourceHandedness handedness, string dictionaryKey)
        {
            var parentGameObject = new GameObject
            {
                name = dictionaryKey + "Controller"
            };

            parentGameObject.transform.parent = transform;
            controllerModelGameObject.transform.parent = parentGameObject.transform;

            var newControllerInfo = new MotionControllerInfo(parentGameObject, handedness);

            newControllerInfo.LoadInfo(controllerModelGameObject.GetComponentsInChildren<Transform>());

            if (handedness == InteractionSourceHandedness.Left)
            {
                leftControllerModel = newControllerInfo;
            }
            else if (handedness == InteractionSourceHandedness.Right)
            {
                rightControllerModel = newControllerInfo;
            }

            if (OnControllerModelLoaded != null)
            {
                OnControllerModelLoaded(newControllerInfo);
            }

            loadingControllers.Remove(dictionaryKey);
            controllerDictionary.Add(dictionaryKey, newControllerInfo);
        }

        public bool TryGetControllerModel(InteractionSourceHandedness handedness, out MotionControllerInfo controller)
        {
            if (handedness == InteractionSourceHandedness.Left && leftControllerModel != null)
            {
                controller = leftControllerModel;
                return true;
            }
            else if (handedness == InteractionSourceHandedness.Right && rightControllerModel != null)
            {
                controller = rightControllerModel;
                return true;
            }
            else
            {
                controller = null;
                return false;
            }
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
