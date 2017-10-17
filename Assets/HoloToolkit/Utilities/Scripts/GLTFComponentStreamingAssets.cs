using System.Collections;
using System.IO;
using UnityEngine;

namespace GLTF
{
    class GLTFComponentStreamingAssets : MonoBehaviour
    {
        [Tooltip("This file should be in the StreamingAssets folder. Please include the file extension.")]
        public string GLTFName = string.Empty;
        public bool Multithreaded = true;

        public int MaximumLod = 300;

        public UnityEngine.Material ColorMaterial = null;
        public UnityEngine.Material NoColorMaterial = null;

        [HideInInspector]
        public byte[] GLTFData;

        private void Start()
        {
            if (!string.IsNullOrEmpty(GLTFName))
            {
                if (File.Exists(Path.Combine(Application.streamingAssetsPath, GLTFName)))
                {
                    GLTFData = File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, GLTFName));
                    StartCoroutine(LoadModel());
                }
                else
                {
                    Debug.Log("The glTF file specified on " + name + " does not exist in the StreamingAssets folder.");
                }
            }
        }

        public IEnumerator LoadModel()
        {
            var loader = new GLTFLoader(
                GLTFData,
                gameObject.transform
            );
            loader.ColorMaterial = ColorMaterial;
            loader.NoColorMaterial = NoColorMaterial;
            loader.Multithreaded = Multithreaded;
            loader.MaximumLod = MaximumLod;
            yield return loader.Load();
        }
    }
}