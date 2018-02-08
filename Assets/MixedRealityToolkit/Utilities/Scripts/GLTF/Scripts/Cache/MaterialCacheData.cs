namespace UnityGLTF.Cache
{
    public class MaterialCacheData
    {
        public UnityEngine.Material UnityMaterial { get; set; }
        public UnityEngine.Material UnityMaterialWithVertexColor { get; set; }
        public GLTF.Schema.Material GLTFMaterial { get; set; }

        public UnityEngine.Material GetContents(bool useVertexColors)
        {
            return useVertexColors ? UnityMaterialWithVertexColor : UnityMaterial;
        }
    }
}
