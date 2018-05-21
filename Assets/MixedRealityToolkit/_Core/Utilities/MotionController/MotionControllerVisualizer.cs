// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using UnityEngine;

#if UNITY_EDITOR_WIN
using System.Runtime.InteropServices;
#endif

#if UNITY_WSA
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.WSA.Input;
//using UnityGLTF;

#if !UNITY_EDITOR
using Windows.Foundation;
using Windows.Storage.Streams;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
#endif
#endif

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities
{
    /// <summary>
    /// This script spawns a specific GameObject when a controller is detected
    /// and animates the controller position, rotation, button presses, and
    /// thumbstick/touchpad interactions, where applicable.
    /// </summary>
    public class MotionControllerVisualizer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("This setting will be used to determine if the model, override or otherwise, should attempt to be animated based on the user's input.")]
        private bool animateControllerModel = true;

        [SerializeField]
        [Tooltip("This setting will be used to determine if the model should always be the left alternate. If false, the platform controller models will be preferred, only if they can't be loaded will the alternate be used. Otherwise, it will always use the alternate model.")]
        private bool alwaysUseAlternateLeftModel = false;

        [SerializeField]
        [Tooltip("This setting will be used to determine if the model should always be the right alternate. If false, the platform controller models will be preferred, only if they can't be loaded will the alternate be used. Otherwise, it will always use the alternate model.")]
        private bool alwaysUseAlternateRightModel = false;

        [SerializeField]
        [Tooltip("Use a model with the tip in the positive Z direction and the front face in the positive Y direction. To override the platform left controller model set AlwaysUseAlternateModel to true; otherwise this will be the default if the model can't be found.")]
        protected GameObject AlternateLeftController;

        [SerializeField]
        [Tooltip("Use a model with the tip in the positive Z direction and the front face in the positive Y direction. To override the platform right controller model set AlwaysUseAlternateModel to true; otherwise this will be the default if the model can't be found.")]
        protected GameObject AlternateRightController;

        [SerializeField]
        [Tooltip("Use this to override the indicator used to show the user's touch location on the touchpad. Default is a sphere.")]
        protected GameObject TouchpadTouchedOverride;

        private static GameObject touchpadTouchedOverride;

        [SerializeField]
        [Tooltip("This material will be used on the loaded glTF controller model. This does not affect the above overrides.")]
        protected UnityEngine.Material GltfMaterial;

#if UNITY_WSA
        // This will be used to keep track of our controllers, indexed by their unique source ID.
        private Dictionary<string, MotionControllerInfo> controllerDictionary = new Dictionary<string, MotionControllerInfo>(0);
        private List<string> loadingControllers = new List<string>();

        private MotionControllerInfo leftControllerModel;
        private MotionControllerInfo rightControllerModel;

#endif
        public static event Action<MotionControllerInfo> OnControllerModelLoaded;
        public static event Action<MotionControllerInfo> OnControllerModelUnloaded;

#if UNITY_EDITOR_WIN
        [DllImport("EditorMotionController")]
        private static extern bool TryGetMotionControllerModel([In] uint controllerId, [Out] out uint outputSize, [Out] out IntPtr outputBuffer);
#endif

        private void Awake()
        {
            touchpadTouchedOverride = TouchpadTouchedOverride;

#if UNITY_WSA
            foreach (var sourceState in InteractionManager.GetCurrentReading())
            {
                if (sourceState.source.kind == InteractionSourceKind.Controller)
                {
                    StartTrackingController(sourceState.source);
                }
            }

            Application.onBeforeRender += Application_onBeforeRender;

            if (GltfMaterial == null)
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

        private void OnDestroy()
        {
#if UNITY_WSA
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
#if UNITY_WSA
            if (InteractionManager.numSourceStates == 0) { return; }

            foreach (var sourceState in InteractionManager.GetCurrentReading())
            {
                MotionControllerInfo currentController;
                if (sourceState.source.kind == InteractionSourceKind.Controller && controllerDictionary.TryGetValue(GenerateKey(sourceState.source), out currentController))
                {
                    if (animateControllerModel)
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

#if UNITY_WSA
        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs obj)
        {
            StartTrackingController(obj.state.source);
        }

        private void StartTrackingController(InteractionSource source)
        {
            string key = GenerateKey(source);

            if (source.kind == InteractionSourceKind.Controller)
            {
                MotionControllerInfo controllerInfo;

                if (!controllerDictionary.ContainsKey(key) && !loadingControllers.Contains(key))
                {
                    StartCoroutine(LoadControllerModel(source));
                }
                else if (controllerDictionary.TryGetValue(key, out controllerInfo))
                {
                    switch (controllerInfo.Handedness)
                    {
                        case (Handedness)InteractionSourceHandedness.Left:
                            leftControllerModel = controllerInfo;
                            break;
                        case (Handedness)InteractionSourceHandedness.Right:
                            rightControllerModel = controllerInfo;
                            break;
                    }

                    controllerInfo.ControllerParent.SetActive(true);

                    OnControllerModelLoaded?.Invoke(controllerInfo);
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
                if (controllerDictionary != null && controllerDictionary.TryGetValue(GenerateKey(source), out controllerInfo))
                {
                    OnControllerModelUnloaded?.Invoke(controllerInfo);

                    switch (controllerInfo.Handedness)
                    {
                        case (Handedness)InteractionSourceHandedness.Left:
                            leftControllerModel = null;
                            break;
                        case (Handedness)InteractionSourceHandedness.Right:
                            rightControllerModel = null;
                            break;
                    }

                    controllerInfo.ControllerParent.SetActive(false);
                }
            }
        }

        private IEnumerator LoadControllerModel(InteractionSource source)
        {
            loadingControllers.Add(GenerateKey(source));

            if (alwaysUseAlternateLeftModel && source.handedness == InteractionSourceHandedness.Left)
            {
                if (AlternateLeftController == null)
                {
                    Debug.LogWarning($"Always use the alternate left model is set on {name}, but the alternate left controller model was not specified.");
                    yield return LoadSourceControllerModel(source);
                }
                else
                {
                    LoadAlternateControllerModel(source);
                }
            }
            else if (alwaysUseAlternateRightModel && source.handedness == InteractionSourceHandedness.Right)
            {
                if (AlternateRightController == null)
                {
                    Debug.LogWarning($"Always use the alternate right model is set on {name}, but the alternate right controller model was not specified.");
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

            if (GltfMaterial == null)
            {
                Debug.Log($"If using glTF, please specify a material on {name}.");
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
            uint outputSize;
            IntPtr controllerModel;

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
            //var gltfScript = controllerModelGameObject.AddComponent<GLTFComponent>();
            //gltfScript.GLTFConstant = gltfScript.GLTFStandard = gltfScript.GLTFStandardSpecular = gltfMaterial.shader;
            //gltfScript.UseStream = true;
            //gltfScript.GLTFStream = new MemoryStream(fileBytes);

            //yield return gltfScript.WaitForModelLoad();

            FinishControllerSetup(controllerModelGameObject, source.handedness, GenerateKey(source));
        }

        private void LoadAlternateControllerModel(InteractionSource source)
        {
            GameObject controllerModelGameObject;
            if (source.handedness == InteractionSourceHandedness.Left && AlternateLeftController != null)
            {
                controllerModelGameObject = Instantiate(AlternateLeftController);
            }
            else if (source.handedness == InteractionSourceHandedness.Right && AlternateRightController != null)
            {
                controllerModelGameObject = Instantiate(AlternateRightController);
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
            return $"{source.vendorId}/{source.productId}/{source.productVersion}/{source.handedness}";
        }

        private void FinishControllerSetup(GameObject controllerModelGameObject, InteractionSourceHandedness handedness, string dictionaryKey)
        {
            var parentGameObject = new GameObject
            {
                name = $"{handedness} Controller"
            };

            parentGameObject.transform.parent = transform;
            controllerModelGameObject.transform.parent = parentGameObject.transform;

            var newControllerInfo = new MotionControllerInfo(parentGameObject, (Handedness)handedness);

            newControllerInfo.LoadInfo(controllerModelGameObject.GetComponentsInChildren<Transform>(), this);

            switch (handedness)
            {
                case InteractionSourceHandedness.Left:
                    leftControllerModel = newControllerInfo;
                    break;
                case InteractionSourceHandedness.Right:
                    rightControllerModel = newControllerInfo;
                    break;
            }

            OnControllerModelLoaded?.Invoke(newControllerInfo);

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

            if (handedness == InteractionSourceHandedness.Right && rightControllerModel != null)
            {
                controller = rightControllerModel;
                return true;
            }

            controller = null;
            return false;
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
                touchVisualizer.GetComponent<Renderer>().sharedMaterial = GltfMaterial;
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
