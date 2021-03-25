// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

#if WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Windows.Storage.Streams;
using Windows.UI.Input.Spatial;
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
#if WINDOWS_UWP
            spatialInteractionSource = WindowsExtensions.GetSpatialInteractionSource(handedness, InputSourceType.Controller);
#endif // WINDOWS_UWP
        }

#if WINDOWS_UWP
        private readonly SpatialInteractionSource spatialInteractionSource;

        private static readonly Dictionary<string, GameObject> ControllerModelDictionary = new Dictionary<string, GameObject>(2);

        /// <summary>
        /// Attempts to load the glTF controller model from the Windows SDK.
        /// </summary>
        /// <returns>The controller model as a GameObject or null if it was unobtainable.</returns>
        public async Task<GameObject> TryGenerateControllerModelFromPlatformSDK()
        {
            if (spatialInteractionSource == null)
            {
                return null;
            }

            // See if we've generated this model before and if we can return it
            if (ControllerModelDictionary.TryGetValue(GenerateKey(spatialInteractionSource), out GameObject controllerModel))
            {
                controllerModel.SetActive(true);
                return controllerModel;
            }

            Debug.Log("Trying to load controller model from platform SDK");
            byte[] fileBytes = null;

            var controllerModelStream = await spatialInteractionSource.Controller.TryGetRenderableModelAsync();
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
                Utilities.Gltf.Schema.GltfObject gltfObject = GltfUtility.GetGltfObjectFromGlb(fileBytes);
                gltfGameObject = await gltfObject.ConstructAsync();
                if (gltfGameObject != null)
                {
                    ControllerModelDictionary.Add(GenerateKey(spatialInteractionSource), gltfGameObject);
                }
            }

            return gltfGameObject;
        }

        private string GenerateKey(SpatialInteractionSource spatialInteractionSource)
        {
            return spatialInteractionSource.Controller.VendorId + "/" + spatialInteractionSource.Controller.ProductId + "/" + spatialInteractionSource.Controller.Version + "/" + spatialInteractionSource.Handedness;
        }
#endif // WINDOWS_UWP
    }
}
