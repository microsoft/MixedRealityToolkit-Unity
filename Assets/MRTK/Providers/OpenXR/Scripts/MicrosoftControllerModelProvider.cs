// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;
using UnityEngine;

#if MSFT_OPENXR_0_9_4_OR_NEWER
using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using System.Collections.Generic;
#endif // MSFT_OPENXR_0_9_4_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    /// <summary>
    /// Queries the OpenXR APIs for a renderable controller model.
    /// </summary>
    internal class MicrosoftControllerModelProvider
    {
        public MicrosoftControllerModelProvider(Utilities.Handedness handedness)
        {
#if MSFT_OPENXR_0_9_4_OR_NEWER
            controllerModelProvider = handedness == Utilities.Handedness.Left ? ControllerModel.Left : ControllerModel.Right;
#endif // MSFT_OPENXR_0_9_4_OR_NEWER
        }

#if MSFT_OPENXR_0_9_4_OR_NEWER
        private static readonly Dictionary<ulong, GameObject> ControllerModelDictionary = new Dictionary<ulong, GameObject>(2);
        private readonly ControllerModel controllerModelProvider;
#endif // MSFT_OPENXR_0_9_4_OR_NEWER

        // Disables "This async method lacks 'await' operators and will run synchronously." when the correct OpenXR package isn't installed
#pragma warning disable CS1998
        /// <summary>
        /// Attempts to load the glTF controller model from OpenXR.
        /// </summary>
        /// <returns>The controller model as a GameObject or null if it was unobtainable.</returns>
        public async Task<GameObject> TryGenerateControllerModelFromPlatformSDK()
        {
            GameObject gltfGameObject = null;

#if MSFT_OPENXR_0_9_4_OR_NEWER
            if (!controllerModelProvider.TryGetControllerModelKey(out ulong modelKey))
            {
                Debug.LogError("Failed to obtain controller model key from platform.");
                return null;
            }

            if (ControllerModelDictionary.TryGetValue(modelKey, out GameObject controllerModel))
            {
                controllerModel.SetActive(true);
                return controllerModel;
            }

            byte[] modelStream = await controllerModelProvider.TryGetControllerModel(modelKey);

            if (modelStream == null || modelStream.Length == 0)
            {
                Debug.LogError("Failed to obtain controller model from platform.");
                return null;
            }

            Utilities.Gltf.Schema.GltfObject gltfObject = GltfUtility.GetGltfObjectFromGlb(modelStream);
            gltfGameObject = await gltfObject.ConstructAsync();

            if (gltfGameObject != null)
            {
                ControllerModelDictionary.Add(modelKey, gltfGameObject);
            }
#endif // MSFT_OPENXR_0_9_4_OR_NEWER

            return gltfGameObject;
        }
#pragma warning restore CS1998
    }
}
