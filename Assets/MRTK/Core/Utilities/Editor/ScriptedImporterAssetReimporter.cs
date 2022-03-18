// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Unity has a strange bug when it tries to import a DLL with a ScriptedImporter and an asset that importer is targeting.
    /// The first time, it will not invoke the ScriptedImporter as it's just being imported itself; the second time the ScriptedImporter will be constructed but Unity thinks it fails.
    /// The third time, the import will succeed. This class will invoke the third time import for .gltf, .glb and .room extensions.
    /// </summary>
    public class ScriptedImporterAssetReimporter : AssetPostprocessor
    {
        private static readonly Dictionary<string, int> assetsAttemptedToReimport = new Dictionary<string, int>();

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string asset in importedAssets)
            {
                // Ignore the StreamingAssets folder, as this appears to not be required and generates console warnings.
                if (asset.Contains("Assets/StreamingAssets"))
                {
                    continue;
                }

                string extension = Path.GetExtension(asset);
                if (extension == ".room" || extension == ".glb" || extension == ".gltf")
                {
                    Type assetType = AssetDatabase.GetMainAssetTypeAtPath(asset);
                    if (assetType == typeof(DefaultAsset))
                    {
                        if (!assetsAttemptedToReimport.TryGetValue(asset, out int numAttempts))
                        {
                            numAttempts = 0;
                        }

                        assetsAttemptedToReimport[asset] = ++numAttempts;

                        if (numAttempts <= 3)
                        {
                            AssetDatabase.ImportAsset(asset);
                        }
                        else
                        {
                            Debug.LogWarning($"Asset '{asset}' appears to have failed the re-import 3 times, will not try again.");
                        }
                    }
                }
            }
        }
    }
}
