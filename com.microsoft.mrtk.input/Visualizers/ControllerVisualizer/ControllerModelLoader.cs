// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.InputSystem.XR;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

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
        /// A dictionary which caches the controller model gameobject associated with a specified input device
        /// Stores a boolean indicating whether the controller model was successfully loaded to prevent repeated failed loads.
        /// </summary>
        private static Dictionary<InputDevice, (bool, GameObject)> controllerModelDictionary = new Dictionary<InputDevice, (bool, GameObject)>();

        /// <summary>
        /// Tries to load the controller model game object of the specified input device with the corresponding handedness
        /// Requires the MR OpenXR plugin to work.
        /// </summary>
        /// <param name="inputDevice">The input device we are trying to get the controller model of</param>
        /// <param name="handedness">The handedness of the input device requesting the controller model</param>
        /// <returns>A gameobject representing the generated controller model in the scene</returns>
        public async static Task<GameObject> TryGenerateControllerModelFromPlatformSDK(InputDevice inputDevice, Handedness handedness)
        {
            // Sanity check to ensure that the xrInputDevice's usages matches the provided handedness
            InternedString targetUsage = handedness == Handedness.Left ? CommonUsages.LeftHand : CommonUsages.RightHand;
            Debug.Assert(inputDevice.usages.Contains(targetUsage));

            bool existsInCahce = controllerModelDictionary.TryGetValue(inputDevice, out var results);
            var (isLoadable, cachedGameObject) = existsInCahce ? results : (false, null);

            // Try to fetch the controller model from our cache and exit early if we are able to get a valid model
            if (existsInCahce)
            {
                if (!isLoadable)
                {
                    // If the object wasn't loadable, return null and exit early
                    return null;
                }
                else if (cachedGameObject != null)
                {
                    // If the object was loadable and the cached object is not null, set it active and return it.
                    cachedGameObject.SetActive(true);
                    return cachedGameObject;
                }
            }

            // Otherwise, proceed with trying to load the model from the platform
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
                // Make sure we record in our cache that a model is not loadable
                controllerModelDictionary[inputDevice] = (false, null);

                Debug.LogWarning("Failed to obtain controller model key from platform.");
                return null;
            }

            byte[] modelStream = await controllerModelProvider.TryGetControllerModel(modelKey);

            if (modelStream == null || modelStream.Length == 0)
            {
                // Make sure we record in our cache that a model is not loadable
                controllerModelDictionary[inputDevice] = (false, null);

                Debug.LogError("Failed to obtain controller model from platform.");
                return null;
            }

            GltfImport gltf = new GltfImport();
            bool success = await gltf.LoadGltfBinary(modelStream);
            gltfGameObject = new GameObject(modelKey.ToString());
            if (success && gltf.InstantiateMainScene(gltfGameObject.transform))
            {
                // After all the awaits, double check that another task didn't finish earlier and write into this cache
                existsInCahce = controllerModelDictionary.TryGetValue(inputDevice, out var previousResults);
                (isLoadable, cachedGameObject) = existsInCahce ? previousResults : (false, null);

                if (existsInCahce && isLoadable && cachedGameObject != null && gltfGameObject != null)
                {
                    Object.Destroy(gltfGameObject);
                    return cachedGameObject;
                }

#if MROPENXR_ANIM_PRESENT
                // Try to add an animator to the controller model
                ControllerModelArticulator controllerModelArticulator = gltfGameObject.EnsureComponent<ControllerModelArticulator>();
                if (!controllerModelArticulator.TryStartArticulating(controllerModelProvider, modelKey))
                {
                    Debug.LogError("Unable to load model animation.");
                }
#endif

                controllerModelDictionary[inputDevice] = (true, gltfGameObject);
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
