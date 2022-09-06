// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

#if MROPENXR_PRESENT && (UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_ANDROID) && GLTFAST_PRESENT && KTX_PRESENT
using Microsoft.MixedReality.OpenXR;
using GLTFast;
#endif // MROPENXR_PRESENT && (UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_ANDROID) && GLTFAST_PRESENT && KTX_PRESENT

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A helper class which loads and caches controller models fetched from the controller's platform sdk
    /// </summary>
    public static class ControllerModelLoader
    {
        /// <summary>
        /// A dictionary which caches the controller model gameobject associated with a specified model key
        /// </summary>
        private static Dictionary<ulong, GameObject> controllerModelDictionary = new Dictionary<ulong, GameObject>();

        /// <summary>
        /// Tries to load the controller model game object from the provided OpenXR ControllerModel.
        /// Requires the MR OpenXR plugin to work.
        /// </summary>
        /// <param name="controllerModelProvider">The OpenXR ControllerModel to loade from</param>
        /// <returns>A gameobject representing the generated controller model in the scene</returns>
        public async static Task<GameObject> TryGenerateControllerModelFromPlatformSDK(Handedness handedness)
        {
            GameObject gltfGameObject = null;

#if MROPENXR_PRESENT && (UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_ANDROID) && GLTFAST_PRESENT && KTX_PRESENT
            ControllerModel controllerModelProvider = handedness == Handedness.Left ? ControllerModel.Left :
                                                        handedness == Handedness.Right ?  ControllerModel.Right :
                                                        null;

            if (controllerModelProvider == null)
            {
                Debug.LogWarning("Controller model provider does not exist for this handedness");
                return null;
            }

            if (!controllerModelProvider.TryGetControllerModelKey(out ulong modelKey))
            {
                Debug.LogWarning("Failed to obtain controller model key from platform.");
                return null;
            }

            if (controllerModelDictionary.TryGetValue(modelKey, out gltfGameObject))
            {
                gltfGameObject.SetActive(true);
                return gltfGameObject;
            }

            byte[] modelStream = await controllerModelProvider.TryGetControllerModel(modelKey);

            if (modelStream == null || modelStream.Length == 0)
            {
                Debug.LogError("Failed to obtain controller model from platform.");
                return null;
            }

            GltfImport gltf = new GltfImport();
            bool success = await gltf.LoadGltfBinary(modelStream);
            gltfGameObject = new GameObject(modelKey.ToString());
            if (success && gltf.InstantiateMainScene(gltfGameObject.transform))
            {
                // After all the awaits, double check that another task didn't finish earlier
                if (controllerModelDictionary.TryGetValue(modelKey, out GameObject existingGameObject))
                {
                    Object.Destroy(gltfGameObject);
                    return existingGameObject;
                }

#if MROPENXR_ANIM_PRESENT
                // Try to add an animator to the controller model
                ControllerModelArticulator controllerModelArticulator = gltfGameObject.EnsureComponent<ControllerModelArticulator>();
                if (!controllerModelArticulator.TryStartArticulating(controllerModelProvider, modelKey))
                {
                    Debug.LogError("Unable to load model animation.");
                }
#endif

                controllerModelDictionary.Add(modelKey, gltfGameObject);
            }
            else
            {
                Debug.LogError("Failed to obtain controller model from platform.");
                Object.Destroy(gltfGameObject);
            }
#else
            await Task.CompletedTask;
#endif // MROPENXR_PRESENT && (UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_ANDROID) && GLTFAST_PRESENT && KTX_PRESENT

            return gltfGameObject;
        }
    }
}
