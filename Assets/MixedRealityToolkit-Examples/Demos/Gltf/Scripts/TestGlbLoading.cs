using System.IO;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Gltf.Serialization;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.Gltf
{
    public class TestGlbLoading : MonoBehaviour
    {
        [SerializeField]
        private string uri = string.Empty;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(uri))
            {
                uri = "\\MixedRealityToolkit-Examples\\Demos\\Gltf\\Models\\Lantern\\glTF-Binary\\Lantern.glb";
                var path = $"{Application.dataPath}{uri}";
                path = path.Replace("/", "\\");
                Debug.Assert(File.Exists(path));
            }
        }

        private async void Start()
        {
            await new WaitForSeconds(5f);

            var gltfObject = await GltfSerializationUtility.GetGltfObjectFromPathAsync($"{Application.dataPath}{uri}");

            if (gltfObject != null)
            {
                Debug.Log("Import successful");
            }
        }
    }
}