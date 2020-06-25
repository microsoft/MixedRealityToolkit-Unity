// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor.Experimental.AssetImporters;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization.Editor
{
    [ScriptedImporter(1, "glb")]
    public class GlbAssetImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext context)
        {
            GltfEditorImporter.OnImportGltfAsset(context);
        }
    }
}