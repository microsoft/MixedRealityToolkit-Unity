// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Blend
{
    public class BlendShaderUtils
    {
        public static ShaderProperties[] GetShaderProperties(Renderer renderer, ShaderPropertyType[] filter)
        {
            List<ShaderProperties> properties = new List<ShaderProperties>();
            if (renderer != null)
            {
                Material material = ColorAbstraction.GetValidMaterial(renderer);

                if (material != null)
                {
                    int count = ShaderUtil.GetPropertyCount(material.shader);

                    for (int i = 0; i < count; i++)
                    {
                        string name = ShaderUtil.GetPropertyName(material.shader, i);
                        ShaderPropertyType type = ShaderUtilConvert(ShaderUtil.GetPropertyType(material.shader, i));
                        bool isHidden = ShaderUtil.IsShaderPropertyHidden(material.shader, i);
                        Vector2 range = new Vector2(ShaderUtil.GetRangeLimits(material.shader, i, 1), ShaderUtil.GetRangeLimits(material.shader, i, 2));

                        if (!isHidden && HasShaderPropertyType(filter, type))
                        {
                            properties.Add(new ShaderProperties() { Name = name, Type = type, Range = range });
                        }
                    }
                }
            }
            return properties.ToArray();
        }

        public static ShaderPropertyType ShaderUtilConvert(ShaderUtil.ShaderPropertyType type)
        {
            ShaderPropertyType shaderType;
            switch (type)
            {
                case ShaderUtil.ShaderPropertyType.Color:
                    shaderType = ShaderPropertyType.Color;
                    break;
                case ShaderUtil.ShaderPropertyType.Vector:
                    shaderType = ShaderPropertyType.Vector;
                    break;
                case ShaderUtil.ShaderPropertyType.Float:
                    shaderType = ShaderPropertyType.Float;
                    break;
                case ShaderUtil.ShaderPropertyType.Range:
                    shaderType = ShaderPropertyType.Range;
                    break;
                case ShaderUtil.ShaderPropertyType.TexEnv:
                    shaderType = ShaderPropertyType.TexEnv;
                    break;
                default:
                    shaderType = ShaderPropertyType.None;
                    break;
            }
            return shaderType;
        }

        public static bool HasShaderPropertyType(ShaderPropertyType[] filter, ShaderPropertyType type)
        {
            for (int i = 0; i < filter.Length; i++)
            {
                if (filter[i] == type)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
