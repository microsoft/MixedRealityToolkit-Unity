// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class MaterialPropertyAssetCache : AssetCache
    {
        [SerializeField]
        private MaterialPropertyAsset[] materialPropertyAssets = null;

        private ILookup<string, MaterialPropertyAsset> materialPropertiesByShaderName;
        private ILookup<string, MaterialPropertyAsset> customMaterialPropertiesByShaderName;
        private MaterialPropertyAsset[] customInstanceShaderProperties;

        private MaterialPropertyAsset[] universalMaterialProperties = new MaterialPropertyAsset[]
        {
            new MaterialPropertyAsset
            {
                propertyName = "renderQueue-36f16fdc-72c7-430d-9479-6c9c2a318b6b",
                propertyType = MaterialPropertyType.RenderQueue,
                Shader = null
            },
            new MaterialPropertyAsset
            {
                propertyName = "shaderKeywords-266455c9-953b-476b-85cd-7dd3fde79381",
                propertyType = MaterialPropertyType.ShaderKeywords,
                Shader = null
            }
        };

        private MaterialPropertyAsset[] CustomInstanceShaderProperties
        {
            get
            {
                if (customInstanceShaderProperties == null)
                {
                    customInstanceShaderProperties = AssetCache.LoadAssetCache<CustomShaderPropertyAssetCache>()?.CustomInstanceShaderProperties ?? Array.Empty<MaterialPropertyAsset>();
                }
                return customInstanceShaderProperties;
            }
        }

        public IEnumerable<MaterialPropertyAsset> GetMaterialProperties(string shaderName)
        {
            return universalMaterialProperties.Concat(MaterialPropertiesByShaderName[shaderName]).Concat(CustomMaterialPropertiesByShaderName[shaderName]);
        }

        private ILookup<string, MaterialPropertyAsset> MaterialPropertiesByShaderName
        {
            get
            {
                return materialPropertiesByShaderName ?? (materialPropertiesByShaderName = (materialPropertyAssets ?? Array.Empty<MaterialPropertyAsset>()).ToLookup(m => m.ShaderName));
            }
        }

        private ILookup<string, MaterialPropertyAsset> CustomMaterialPropertiesByShaderName
        {
            get
            {
                return customMaterialPropertiesByShaderName ?? (customMaterialPropertiesByShaderName = CustomInstanceShaderProperties.ToLookup(m => m.ShaderName));
            }
        }

        public override void UpdateAssetCache()
        {
#if UNITY_EDITOR
            Dictionary<MaterialPropertyKey, MaterialPropertyAsset> materialProperties = (materialPropertyAssets ?? Array.Empty<MaterialPropertyAsset>()).ToDictionary(p => new MaterialPropertyKey { PropertyName = p.propertyName, ShaderName = p.ShaderName });
            HashSet<MaterialPropertyKey> unvisitedMaterialProperties = new HashSet<MaterialPropertyKey>(materialProperties.Keys);

            foreach (Renderer renderer in EnumerateAllComponentsInScenesAndPrefabs<Renderer>())
            {
                if (renderer != null && renderer.sharedMaterials != null)
                {
                    foreach (Material material in renderer.sharedMaterials)
                    {
                        UpdateMaterial(materialProperties, unvisitedMaterialProperties, material);
                    }
                }
            }

            foreach (Graphic graphic in EnumerateAllComponentsInScenesAndPrefabs<Graphic>())
            {
                if (graphic.materialForRendering != null)
                {
                    UpdateMaterial(materialProperties, unvisitedMaterialProperties, graphic.materialForRendering);
                }
            }

            foreach (Material material in EnumerateAllAssetsInAssetDatabase<Material>(IsMaterialFileExtension))
            {
                UpdateMaterial(materialProperties, unvisitedMaterialProperties, material);
            }

            CleanUpUnused(materialProperties, unvisitedMaterialProperties);
            materialPropertyAssets = materialProperties.Values.ToArray();

            EditorUtility.SetDirty(this);
#endif
        }

        public override void ClearAssetCache()
        {
#if UNITY_EDITOR
            materialPropertyAssets = null;
            materialPropertiesByShaderName = null;

            EditorUtility.SetDirty(this);
#endif
        }

        private static bool IsMaterialFileExtension(string fileExtension)
        {
            return fileExtension == ".mat";
        }

#if UNITY_EDITOR
        private static void UpdateMaterial(Dictionary<MaterialPropertyKey, MaterialPropertyAsset> materialProperties, HashSet<MaterialPropertyKey> unvisitedMaterialProperties, Material material)
        {
            if (material != null)
            {
                foreach (MaterialProperty materialProperty in MaterialEditor.GetMaterialProperties(new Material[] { material }))
                {
                    var key = new MaterialPropertyKey(material.shader.name, materialProperty.name);
                    unvisitedMaterialProperties.Remove(key);
                    if (!materialProperties.ContainsKey(key))
                    {
                        materialProperties.Add(key, new MaterialPropertyAsset
                        {
                            Shader = material.shader,
                            propertyName = materialProperty.name,
                            propertyType = (MaterialPropertyType)materialProperty.type
                        });
                    }
                }
            }
        }
#endif
    }
}