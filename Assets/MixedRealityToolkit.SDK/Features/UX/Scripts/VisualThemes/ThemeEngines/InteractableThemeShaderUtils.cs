// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    // Basic value types within a shader
    public enum ShaderPropertyType { Color, Float, Range, TexEnv, Vector, None }

    /// <summary>
    /// Obsolete container. Only exists to support backward compatibility to copy values from old scriptableobjects
    /// </summary>
    [System.Serializable]
    public struct ShaderProperties
    {
        public string Name;
        public ShaderPropertyType Type;
        public Vector2 Range;
    }

    /// <summary>
    /// Collection of shader and material utilities
    /// </summary>
    public static class InteractableThemeShaderUtils
    {
        /// <summary>
        /// Get a MaterialPropertyBlock and copy the designated properties
        /// </summary>
        public static MaterialPropertyBlock InitMaterialPropertyBlock(GameObject gameObject, List<ThemeStateProperty> props)
        {
            MaterialPropertyBlock materialBlock = GetPropertyBlock(gameObject);
            Renderer renderer = gameObject.GetComponent<Renderer>();

            if (renderer != null)
            {
                Material material = GetValidMaterial(renderer);
                if (material != null)
                {
                    foreach (ThemeStateProperty prop in props)
                    {
                        switch (prop.Type)
                        {
                            case ThemePropertyTypes.Color:
                                Color color = material.GetVector(prop.ShaderPropertyName);
                                materialBlock.SetColor(prop.ShaderPropertyName, color);
                                break;
                            case ThemePropertyTypes.Texture:
                                Texture tex = material.GetTexture(prop.ShaderPropertyName);
                                if (tex != null)
                                {
                                    materialBlock.SetTexture(prop.ShaderPropertyName, tex);
                                }
                                break;
                            case ThemePropertyTypes.ShaderFloat:
                            case ThemePropertyTypes.ShaderRange:
                                float value = material.GetFloat(prop.ShaderPropertyName);
                                materialBlock.SetFloat(prop.ShaderPropertyName, value);
                                break;
                            default:
                                break;
                        }
                    }
                }

                gameObject.GetComponent<Renderer>().SetPropertyBlock(materialBlock);
            }

            return materialBlock;
        }

        /// <summary>
        /// Get the MaterialPropertyBlock from a renderer on a gameObject
        /// </summary>
        public static MaterialPropertyBlock GetPropertyBlock(GameObject gameObject)
        {
            MaterialPropertyBlock materialBlock = new MaterialPropertyBlock();
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.GetPropertyBlock(materialBlock);
            }
            return materialBlock;
        }

        /// <summary>
        /// Grab the shared material to avoid creating new material instances and breaking batching.
        /// Because MaterialPropertyBlocks are used for setting material properties the shared material is
        /// used to set the initial state of the MaterialPropertyBlock(s) before mutating state.
        /// </summary>
        public static Material GetValidMaterial(Renderer renderer)
        {
            Material material = null;

            if (renderer != null)
            {
                material = renderer.sharedMaterial;
            }
            return material;
        }
    }
}
