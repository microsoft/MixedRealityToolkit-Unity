// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor.Experimental.AssetImporters;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization.Editor
{
    [ScriptedImporter(1, "gltf")]
    public class GltfAssetImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext context)
        {
            GltfEditorImporter.OnImportGltfAsset(context);
        }
    }
}