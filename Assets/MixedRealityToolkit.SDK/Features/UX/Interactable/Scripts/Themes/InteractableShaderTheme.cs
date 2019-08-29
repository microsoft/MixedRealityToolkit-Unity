// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractableShaderTheme : InteractableThemeBase
    {
        private static ThemePropertyValue emptyValue = new ThemePropertyValue();

        protected MaterialPropertyBlock propertyBlock;
        protected List<ShaderProperties> shaderProperties;
        protected Renderer renderer;

        private ThemePropertyValue startValue = new ThemePropertyValue();

        private const string DefaultShaderProperty = "_Color";
        private const string DefaultShaderName = "Mixed Reality Toolkit/Standard";

        public InteractableShaderTheme()
        {
            Types = new Type[] { typeof(Renderer) };
            Name = "Shader Float";
            StateProperties = GetDefaultStateProperties();
        }

        /// <inheritdoc />
        public override List<ThemeStateProperty> GetDefaultStateProperties()
        {
            return new List<ThemeStateProperty>()
            {
                new ThemeStateProperty()
                {
                    Name = "Shader Value",
                    Type = ThemePropertyTypes.ShaderFloat,
                    Values = new List<ThemePropertyValue>(),
                    Default = new ThemePropertyValue() { Float = 0}
                },
            };
        }

        /// <inheritdoc />
        public override List<ThemeProperty> GetDefaultThemeProperties()
        {
            return new List<ThemeProperty>()
            {
                new ThemeProperty()
                {
                    Name = "Shader Property",
                    Type = ThemePropertyTypes.ShaderProperty,
                    Value = new ThemePropertyValue()
                    {
                        Shader = Shader.Find(DefaultShaderName),
                        String = DefaultShaderProperty
                    },
                },
            };
        }

        /// <inheritdoc />
        public override void Init(GameObject host, ThemeDefinition settings)
        {
            base.Init(host, settings);

            shaderProperties = new List<ShaderProperties>();
            for (int i = 0; i < StateProperties.Count; i++)
            {
                ThemeStateProperty prop = StateProperties[i];
                if (prop.ShaderOptions.Count > 0)
                {
                    shaderProperties.Add(prop.ShaderOptions[prop.PropId]);
                }
            }

            propertyBlock = InteractableThemeShaderUtils.GetMaterialPropertyBlock(host, shaderProperties.ToArray());

            renderer = Host.GetComponent<Renderer>();
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            if (Host == null)
                return;

            renderer.GetPropertyBlock(propertyBlock);

            int propId = property.GetShaderPropertyId();
            float newValue;
            switch (property.Type)
            {
                case ThemePropertyTypes.Color:
                    Color newColor = Color.Lerp(property.StartValue.Color, property.Values[index].Color, percentage);
                    propertyBlock = SetColor(propertyBlock, newColor, propId);
                    break;
                case ThemePropertyTypes.ShaderFloat:
                    newValue = LerpFloat(property.StartValue.Float, property.Values[index].Float, percentage);
                    propertyBlock = SetFloat(propertyBlock, newValue, propId);
                    break;
                case ThemePropertyTypes.ShaderRange:
                    newValue = LerpFloat(property.StartValue.Float, property.Values[index].Float, percentage);
                    propertyBlock = SetFloat(propertyBlock, newValue, propId);
                    break;
                default:
                    break;
            }

            renderer.SetPropertyBlock(propertyBlock);
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            if (Host == null)
                return emptyValue;

            renderer.GetPropertyBlock(propertyBlock);

            startValue.Reset();
            
            int propId = property.GetShaderPropertyId();
            switch (property.Type)
            {
                case ThemePropertyTypes.Color:
                    startValue.Color = propertyBlock.GetVector(propId);
                    break;
                case ThemePropertyTypes.ShaderFloat:
                    startValue.Float = propertyBlock.GetFloat(propId);
                    break;
                case ThemePropertyTypes.ShaderRange:
                    startValue.Float = propertyBlock.GetFloat(propId);
                    break;
                default:
                    break;
            }

            return startValue;
        }

        public static float GetFloat(GameObject host, int propId)
        {
            if (host == null)
                return 0;

            MaterialPropertyBlock block = InteractableThemeShaderUtils.GetPropertyBlock(host);
            return block.GetFloat(propId);
        }

        public static void SetPropertyBlock(GameObject host, MaterialPropertyBlock block)
        {
            Renderer renderer = host.GetComponent<Renderer>();
            renderer.SetPropertyBlock(block);
        }

        public static MaterialPropertyBlock SetFloat(MaterialPropertyBlock block, float value, int propId)
        {
            if (block == null)
                return null;

            block.SetFloat(propId, value);
            return block;
        }

        public static Color GetColor(GameObject host, int propId)
        {
            if (host == null)
                return Color.white;

            MaterialPropertyBlock block = InteractableThemeShaderUtils.GetPropertyBlock(host);
            return block.GetVector(propId);
        }

        public static MaterialPropertyBlock SetColor(MaterialPropertyBlock block, Color color, int propId)
        {
            if (block == null)
                return null;

            block.SetColor(propId, color);
            return block;

        }
    }
}
