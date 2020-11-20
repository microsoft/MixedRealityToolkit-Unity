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
#endif

//#if DOTNETWINRT_PRESENT
//using Microsoft.Windows.UI.Input.Spatial;
//#endif

#if WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.Windows.Input;
using Windows.Storage.Streams;
using Windows.UI.Input.Spatial;
using System.Threading.Tasks;
#endif


namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// Queries the hand mesh data that an articulated hand on HoloLens 2 can provide.
    /// </summary>
    public class WindowsMixedRealityControllerModelProvider
    {

        //#if DOTNETWINRT_PRESENT
#if WINDOWS_UWP
        public WindowsMixedRealityControllerModelProvider(BaseController controller, SpatialInteractionSource spatialInteractionSource)
        {
            this.controller = controller;
            this.spatialInteractionSource = spatialInteractionSource;
        }

        public bool UnableToGenerateSDKModel;

        private readonly BaseController controller;
        private IMixedRealityInputSource InputSource => controller.InputSource;
        private Handedness Handedness => controller.ControllerHandedness;
        private SpatialInteractionSource spatialInteractionSource;

        private static readonly Dictionary<string, GameObject> controllerModelDictionary = new Dictionary<string, GameObject>(0);

        public async Task<GameObject> TryGenerateControllerModelFromPlatformSDK()
        {
            // First see if we've generated this model before and if we can return it
            GameObject controllerModel;

            if (controllerModelDictionary.TryGetValue(GenerateKey(spatialInteractionSource), out controllerModel))
            {
                UnableToGenerateSDKModel = false;
                controllerModel.SetActive(true);
                return controllerModel;
            }

            DebugUtilities.Log("Trying to load controller model from platform SDK");
            byte[] fileBytes = null;

            var controllerModelStream = await spatialInteractionSource.Controller.TryGetRenderableModelAsync();
            if (controllerModelStream == null ||
                controllerModelStream.Size == 0)
            {
                DebugUtilities.LogError("Failed to obtain controller model from driver");
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
                    DebugUtilities.LogError("Loaded model");
                    var visualizationProfile = CoreServices.InputSystem?.InputSystemProfile.ControllerVisualizationProfile;
                    if (visualizationProfile != null)
                    {
                        var visualizationType = visualizationProfile.GetControllerVisualizationTypeOverride(GetType(), Handedness);
                        if (visualizationType != null)
                        {
                            // Set the platform controller model to not be destroyed when the source is lost. It'll be disabled instead,
                            // and re-enabled when the same controller is re-detected.
                            if (gltfGameObject.AddComponent(visualizationType.Type) is IMixedRealityControllerPoseSynchronizer visualizer)
                            {
                                visualizer.DestroyOnSourceLost = false;
                            }

                            controllerModelDictionary.Add(GenerateKey(spatialInteractionSource), gltfGameObject);
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

        private string GenerateKey(SpatialInteractionSource spatialInteractionSourceState)
        {
            return spatialInteractionSourceState.Id + "/" + spatialInteractionSourceState.Kind + "/" + spatialInteractionSourceState.Handedness;
        }
#endif // WINDOWS_UWP
    }
}
