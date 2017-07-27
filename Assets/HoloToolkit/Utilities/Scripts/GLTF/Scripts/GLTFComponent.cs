using System.Collections;
using UnityEngine;

namespace GLTF
{
    class GLTFComponent : MonoBehaviour
    {
        private byte[] GLTFData;
        public bool Multithreaded = true;

        public int MaximumLod = 300;

        public Shader GLTFStandard;
        public Shader GLTFConstant;

        public IEnumerator LoadModel()
        {
            var loader = new GLTFLoader(
                GLTFData,
                gameObject.transform
            );
            loader.SetShaderForMaterialType(GLTFLoader.MaterialType.PbrMetallicRoughness, GLTFStandard);
            loader.SetShaderForMaterialType(GLTFLoader.MaterialType.CommonConstant, GLTFConstant);
            loader.Multithreaded = Multithreaded;
            loader.MaximumLod = MaximumLod;
            yield return loader.Load();
        }

        public void SetData(byte[] newData)
        {
            GLTFData = newData;
        }
    }
}