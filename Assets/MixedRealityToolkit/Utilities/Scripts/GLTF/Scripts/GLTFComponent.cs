using System.Collections;
using System.IO;
using UnityEngine;

namespace UnityGLTF
{
    /// <summary>
    /// Component to load a GLTF scene with
    /// </summary>
    class GLTFComponent : MonoBehaviour
    {
        public string Url = "";
        public bool Multithreaded = true;
        public bool UseStream = false;

        public int MaximumLod = 300;

        public Shader GLTFStandard = null;
        public Shader GLTFStandardSpecular = null;
        public Shader GLTFConstant = null;

        public bool addColliders = false;

        public Stream GLTFStream = null;

        public bool IsLoaded { get; set; }

        IEnumerator Start()
        {
            GLTFSceneImporter loader = null;

            if (UseStream)
            {
                string fullPath = "";

                if (GLTFStream == null)
                {
                    fullPath = Path.Combine(Application.streamingAssetsPath, Url);
                    GLTFStream = File.OpenRead(fullPath);
                }

                loader = new GLTFSceneImporter(
                    fullPath,
                    GLTFStream,
                    gameObject.transform,
                    addColliders
                    );
            }
            else
            {
                loader = new GLTFSceneImporter(
                    Url,
                    gameObject.transform,
                    addColliders
                    );
            }

            loader.SetShaderForMaterialType(GLTFSceneImporter.MaterialType.PbrMetallicRoughness, GLTFStandard);
            loader.SetShaderForMaterialType(GLTFSceneImporter.MaterialType.KHR_materials_pbrSpecularGlossiness, GLTFStandardSpecular);
            loader.SetShaderForMaterialType(GLTFSceneImporter.MaterialType.CommonConstant, GLTFConstant);
            loader.MaximumLod = MaximumLod;
            yield return loader.Load(-1, Multithreaded);

            if (GLTFStream != null)
            {
#if WINDOWS_UWP
                GLTFStream.Dispose();
#else
                GLTFStream.Close();
#endif

                GLTFStream = null;
            }
            
            IsLoaded = true;
        }

        public IEnumerator WaitForModelLoad()
        {
            while (!IsLoaded)
            {
                yield return null;
            }
        }
    }
}
