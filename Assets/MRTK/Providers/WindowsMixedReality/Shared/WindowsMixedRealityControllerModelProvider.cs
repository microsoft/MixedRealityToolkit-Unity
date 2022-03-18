// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Threading.Tasks;
using UnityEngine;

#if WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using System;
using System.Collections.Generic;
using Windows.Storage.Streams;
using Windows.UI.Input.Spatial;
#endif

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// Queries the WinRT APIs for a renderable controller model.
    /// </summary>
    internal class WindowsMixedRealityControllerModelProvider
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
#endif // WINDOWS_UWP

        // Disables "This async method lacks 'await' operators and will run synchronously." for non-UWP
#pragma warning disable CS1998
        /// <summary>
        /// Attempts to load the glTF controller model from the Windows SDK.
        /// </summary>
        /// <returns>The controller model as a GameObject or null if it was unobtainable.</returns>
        public async Task<GameObject> TryGenerateControllerModelFromPlatformSDK()
        {
            GameObject gltfGameObject = null;

#if WINDOWS_UWP
            if (spatialInteractionSource == null)
            {
                return null;
            }

            string key = GenerateKey(spatialInteractionSource);

            // See if we've generated this model before and if we can return it
            if (ControllerModelDictionary.TryGetValue(key, out gltfGameObject))
            {
                gltfGameObject.SetActive(true);
                return gltfGameObject;
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

            if (fileBytes != null)
            {
                Utilities.Gltf.Schema.GltfObject gltfObject = GltfUtility.GetGltfObjectFromGlb(fileBytes);
                gltfGameObject = await gltfObject.ConstructAsync();
                if (gltfGameObject != null)
                {
                    // After all the awaits, double check that another task didn't finish earlier
                    if (ControllerModelDictionary.TryGetValue(key, out GameObject existingGameObject))
                    {
                        UnityEngine.Object.Destroy(gltfGameObject);
                        return existingGameObject;
                    }
                    else
                    {
                        ControllerModelDictionary.Add(key, gltfGameObject);
                    }
                }
            }
#endif // WINDOWS_UWP

            return gltfGameObject;
        }
#pragma warning restore CS1998

#if WINDOWS_UWP
        private string GenerateKey(SpatialInteractionSource spatialInteractionSource)
        {
            return spatialInteractionSource.Controller.VendorId + "/" + spatialInteractionSource.Controller.ProductId + "/" + spatialInteractionSource.Controller.Version + "/" + spatialInteractionSource.Handedness;
        }
#endif // WINDOWS_UWP
    }
}
