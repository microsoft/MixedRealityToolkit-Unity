// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif // UNITY_2020_2_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization.Editor
{
#if !MRTK_GLTF_IMPORTER_OFF
    [ScriptedImporter(1, "glb")]
#endif // !MRTK_GLTF_IMPORTER_OFF
    public class GlbAssetImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext context)
        {
            GltfEditorImporter.OnImportGltfAsset(context);
        }
    }
}
