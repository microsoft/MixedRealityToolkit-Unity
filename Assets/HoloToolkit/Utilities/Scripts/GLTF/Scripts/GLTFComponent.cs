using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace GLTF
{
    class GLTFComponent : MonoBehaviour
    {
        public string Url = string.Empty;
        public bool Multithreaded = true;

        public int MaximumLod = 300;

        public Shader GLTFStandard = null;
        public Shader GLTFConstant = null;

        IEnumerator Start()
        {
            UnityWebRequest www = UnityWebRequest.Get(Url);

#if UNITY_2017_2_OR_NEWER
            yield return www.SendWebRequest();
#else
            yield return www.Send();
#endif
            byte[] gltfData = www.downloadHandler.data;

            var loader = new GLTFLoader(
                gltfData,
                gameObject.transform
            );
            loader.SetShaderForMaterialType(GLTFLoader.MaterialType.PbrMetallicRoughness, GLTFStandard);
            loader.SetShaderForMaterialType(GLTFLoader.MaterialType.CommonConstant, GLTFConstant);
            loader.Multithreaded = Multithreaded;
            loader.MaximumLod = MaximumLod;
            yield return loader.Load();
        }
    }
}