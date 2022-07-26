using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.OpenXR;
using System.Threading.Tasks;

#if GLTFAST_PRESENT
using GLTFast;
#endif

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class ControllerModelLoader
    {
        public static Dictionary<ulong, GameObject> ControllerModelDictionary;

        public async static Task<GameObject> TryGenerateControllerModelFromPlatformSDK(ControllerModel controllerModelProvider)
        {
            GameObject gltfGameObject = null;

#if MROPENXR_PRESENT && (UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_ANDROID) && GLTFAST_PRESENT
            if (!controllerModelProvider.TryGetControllerModelKey(out ulong modelKey))
            {
                Debug.LogError("Failed to obtain controller model key from platform.");
                return null;
            }

            if (ControllerModelDictionary.TryGetValue(modelKey, out gltfGameObject))
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
                if (ControllerModelDictionary.TryGetValue(modelKey, out GameObject existingGameObject))
                {
                    Object.Destroy(gltfGameObject);
                    return existingGameObject;
                }
                ControllerModelDictionary.Add(modelKey, gltfGameObject);
            }
            else
            {
                Debug.LogError("Failed to obtain controller model from platform.");
                Object.Destroy(gltfGameObject);
            }
#endif //MROPENXR_PRESENT && (UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_ANDROID)

            return gltfGameObject;
        }
    }
}
