// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using System;

#if UNITY_WSA
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using Windows.UI.Input.Spatial;
#endif

#if WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.Windows.Input;
using Windows.Storage.Streams;
using Windows.UI.Input.Spatial;
using System.Threading.Tasks;
#endif


namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// Queries the WinRT APIs for a renderable controller model.
    /// </summary>
    public class WindowsMixedRealityControllerModelProvider
    {
        public WindowsMixedRealityControllerModelProvider(Handedness handedness)
        {
            this.handedness = handedness;

#if WINDOWS_UWP
            spatialInteractionSource = WindowsExtensions.GetSpatialInteractionSource(handedness, InputSourceType.Controller);
#endif // WINDOWS_UWP
        }

        public bool UnableToGenerateSDKModel;
        private readonly Handedness handedness;

        private readonly BaseController controller;
        private IMixedRealityInputSource InputSource => controller.InputSource;
        private SpatialInteractionSourceState spatialInteractionSourceState;
#if WINDOWS_UWP

        private static readonly Dictionary<string, GameObject> controllerModelDictionary = new Dictionary<string, GameObject>(0);

        public async Task<GameObject> TryGenerateControllerModelFromPlatformSDK()
        {
            // First see if we've generated this model before and if we can return it
            GameObject controllerModel;

            if (controllerModelDictionary.TryGetValue(GenerateKey(spatialInteractionSourceState), out controllerModel))
            {
                UnableToGenerateSDKModel = false;
                controllerModel.SetActive(true);
                return controllerModel;
            }

            Debug.Log("Trying to load controller model from platform SDK");
            byte[] fileBytes = null;

            var controllerModelStream = await spatialInteractionSourceState.Source.Controller.TryGetRenderableModelAsync();
            if (controllerModelStream == null ||
                controllerModelStream.Size == 0)
            {
                Debug.LogError("Failed to obtain controller model from driver");
            }
            else
            {
                fileBytes = new byte[controllerModelStream.Size];
                using (DataReader reader = new DataReader(controllerModelStream))
                {
                    await reader.LoadAsync((uint)controllerModelStream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }

            GameObject gltfGameObject = null;
            if (fileBytes != null)
            {
                var gltfObject = GltfUtility.GetGltfObjectFromGlb(fileBytes);
                gltfGameObject = await gltfObject.ConstructAsync();
                if (gltfGameObject != null)
                {
                    var visualizationProfile = CoreServices.InputSystem?.InputSystemProfile.ControllerVisualizationProfile;
                    if (visualizationProfile != null)
                    {
                        var visualizationType = visualizationProfile.GetControllerVisualizationTypeOverride(GetType(), handedness);
                        if (visualizationType != null)
                        {
                            // Set the platform controller model to not be destroyed when the source is lost. It'll be disabled instead,
                            // and re-enabled when the same controller is re-detected.
                            if (gltfGameObject.AddComponent(visualizationType.Type) is IMixedRealityControllerPoseSynchronizer visualizer)
                            {
                                visualizer.DestroyOnSourceLost = false;
                            }

                            controllerModelDictionary.Add(GenerateKey(spatialInteractionSourceState), gltfGameObject);
                            return gltfGameObject;
                        }
                        else
                        {
                            Debug.LogError("Controller visualization type not defined for controller visualization profile");
                            UnityEngine.Object.Destroy(gltfGameObject);
                            gltfGameObject = null;
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to obtain a controller visualization profile");
                    }
                }
            }

            // record whether or not we were able to succesfully generate the controller model
            UnableToGenerateSDKModel = gltfGameObject == null;
            return null;
        }

        private string GenerateKey(SpatialInteractionSourceState spatialInteractionSourceState)
        {
            return spatialInteractionSourceState.Source.Id + "/" + spatialInteractionSourceState.Source.Kind + "/" + spatialInteractionSourceState.Source.Handedness;
        }
#endif // WINDOWS_UWP
    }
}
