using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace GLTF
{
    class GLTFComponent : MonoBehaviour
    {
        public string Url;
        public bool Multithreaded = true;

        public int MaximumLod = 300;

        public Shader GLTFStandard;
        public Shader GLTFConstant;

        IEnumerator Start()
        {
            UnityWebRequest www = UnityWebRequest.Get(Url);
            yield return www.SendWebRequest();
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