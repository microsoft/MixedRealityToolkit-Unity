// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization.Editor
{
    public static class GltfEditorImporter
    {
        public static async void OnImportGltfAsset(AssetImportContext context)
        {
            var importedObject = await GltfUtility.ImportGltfObjectFromPathAsync(context.assetPath);

            if (importedObject == null ||
                importedObject.GameObjectReference == null)
            {
                Debug.LogError("Failed to import glTF object");
                return;
            }

            var gltfAsset = (GltfAsset)ScriptableObject.CreateInstance(typeof(GltfAsset));

            gltfAsset.GltfObject = importedObject;
            gltfAsset.name = $"{gltfAsset.GltfObject.Name}{Path.GetExtension(context.assetPath)}";
            gltfAsset.Model = importedObject.GameObjectReference;
            context.AddObjectToAsset("main", gltfAsset.Model);
            context.SetMainObject(importedObject.GameObjectReference);
            context.AddObjectToAsset("glTF data", gltfAsset);

            bool reImport = false;

            for (var i = 0; i < gltfAsset.GltfObject.textures?.Length; i++)
            {
                GltfTexture gltfTexture = gltfAsset.GltfObject.textures[i];

                if (gltfTexture == null) { continue; }

                var path = AssetDatabase.GetAssetPath(gltfTexture.Texture);

                if (string.IsNullOrWhiteSpace(path))
                {
                    var textureName = gltfTexture.name;

                    if (string.IsNullOrWhiteSpace(textureName))
                    {
                        textureName = $"Texture_{i}";
                        gltfTexture.Texture.name = textureName;
                    }

                    context.AddObjectToAsset(textureName, gltfTexture.Texture);
                }
                else
                {
                    if (!gltfTexture.Texture.isReadable)
                    {
                        var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                        if (textureImporter != null)
                        {
                            textureImporter.isReadable = true;
                            textureImporter.SetPlatformTextureSettings(new TextureImporterPlatformSettings { format = TextureImporterFormat.RGBA32 });
                            textureImporter.SaveAndReimport();
                            reImport = true;
                        }
                    }
                }
            }

            if (reImport)
            {
                var importer = AssetImporter.GetAtPath(context.assetPath);
                importer.SaveAndReimport();
                return;
            }

            for (var i = 0; i < gltfAsset.GltfObject.meshes?.Length; i++)
            {
                GltfMesh gltfMesh = gltfAsset.GltfObject.meshes[i];

                string meshName = string.IsNullOrWhiteSpace(gltfMesh.name) ? $"Mesh_{i}" : gltfMesh.name;

                gltfMesh.Mesh.name = meshName;
                context.AddObjectToAsset($"{meshName}", gltfMesh.Mesh);
            }

            if (gltfAsset.GltfObject.materials != null)
            {
                foreach (GltfMaterial gltfMaterial in gltfAsset.GltfObject.materials)
                {
                    if (context.assetPath.EndsWith(".glb"))
                    {
                        context.AddObjectToAsset(gltfMaterial.name, gltfMaterial.Material);
                    }
                    else
                    {
                        var relativePath = Path.GetFullPath(Path.GetDirectoryName(context.assetPath)).Replace(Path.GetFullPath(Application.dataPath), "Assets");
                        relativePath = Path.Combine(relativePath, $"{gltfMaterial.name}.mat");
                        AssetDatabase.CreateAsset(gltfMaterial.Material, relativePath);
                        gltfMaterial.Material = AssetDatabase.LoadAssetAtPath<Material>(relativePath);
                    }
                }
            }
        }
    }
}