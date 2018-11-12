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
                uri = $"{Application.dataPath}\\MixedRealityToolkit-Examples\\Demos\\Gltf\\Models\\Lantern\\glTF-Binary\\Lantern.glb";
                uri = uri.Replace("/", "\\");
                Debug.Assert(File.Exists(uri));
            }
        }

        private async void Start()
        {
            await new WaitForSeconds(5f);

            var gltfObject = await GltfSerializationUtility.GetGltfObjectFromPathAsync(uri);

            if (gltfObject != null)
            {
                Debug.Log("Import successful");
            }
        }
    }
}