using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit.Subsystems;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class OpenXRControllerModelSubsystem
    {
        public static Dictionary<ulong, GameObject> ControllerModelDictionary;

        public async static Task<GameObject> TryGenerateControllerModelFromPlatformSDK(ControllerModel controllerModelProvider)
        {
            GameObject gltfGameObject = null;

#if MROPENXR_PRESENT && (UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_ANDROID)
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

            GltfObject gltfObject = GltfUtility.GetGltfObjectFromGlb(modelStream);
            gltfGameObject = await gltfObject.ConstructAsync();

            if (gltfGameObject != null)
            {
                // After all the awaits, double check that another task didn't finish earlier
                if (ControllerModelDictionary.TryGetValue(modelKey, out GameObject existingGameObject))
                {
                    Object.Destroy(gltfGameObject);
                    return existingGameObject;
                }
                else
                {
                    ControllerModelDictionary.Add(modelKey, gltfGameObject);
                }
            }
#endif //MROPENXR_PRESENT && (UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_ANDROID)

            return gltfGameObject;
        }
    }
}
