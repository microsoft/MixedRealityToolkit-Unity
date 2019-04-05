// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractableShaderTheme : InteractableThemeBase
    {
        private static InteractableThemePropertyValue emptyValue = new InteractableThemePropertyValue();

        protected MaterialPropertyBlock propertyBlock;
        protected List<ShaderProperties> shaderProperties;
        protected Renderer renderer;

        private InteractableThemePropertyValue startValue = new InteractableThemePropertyValue();

        public InteractableShaderTheme()
        {
            Types = new Type[] { typeof(Renderer) };
            Name = "Shader Float";
            ThemeProperties.Add(
                new InteractableThemeProperty()
                {
                    Name = "Shader",
                    Type = InteractableThemePropertyValueTypes.ShaderFloat,
                    Values = new List<InteractableThemePropertyValue>(),
                    Default = new InteractableThemePropertyValue() { Float = 0}
                });
        }

        public override void Init(GameObject host, InteractableThemePropertySettings settings)
        {
            base.Init(host, settings);

            shaderProperties = new List<ShaderProperties>();
            for (int i = 0; i < ThemeProperties.Count; i++)
            {
                InteractableThemeProperty prop = ThemeProperties[i];
                if (prop.ShaderOptions.Count > 0)
                {
                    shaderProperties.Add(prop.ShaderOptions[prop.PropId]);
                }
            }

            propertyBlock = InteractableThemeShaderUtils.GetMaterialPropertyBlock(host, shaderProperties.ToArray());

            renderer = Host.GetComponent<Renderer>();
        }

        public override void SetValue(InteractableThemeProperty property, int index, float percentage)
        {
            if (Host == null)
                return;

            string propId = property.GetShaderPropId();
            float newValue;
            switch (property.Type)
            {
                case InteractableThemePropertyValueTypes.Color:
                    Color newColor = Color.Lerp(property.StartValue.Color, property.Values[index].Color, percentage);
                    propertyBlock = SetColor(propertyBlock, newColor, propId);
                    break;
                case InteractableThemePropertyValueTypes.ShaderFloat:
                    newValue = LerpFloat(property.StartValue.Float, property.Values[index].Float, percentage);
                    propertyBlock = SetFloat(propertyBlock, newValue, propId);
                    break;
                case InteractableThemePropertyValueTypes.shaderRange:
                    newValue = LerpFloat(property.StartValue.Float, property.Values[index].Float, percentage);
                    propertyBlock = SetFloat(propertyBlock, newValue, propId);
                    break;
                default:
                    break;
            }

            renderer.SetPropertyBlock(propertyBlock);
        }

        public override InteractableThemePropertyValue GetProperty(InteractableThemeProperty property)
        {
            if (Host == null)
                return emptyValue;

            startValue.Reset();

            string propId = property.GetShaderPropId();
            switch (property.Type)
            {
                case InteractableThemePropertyValueTypes.Color:
                    startValue.Color = propertyBlock.GetVector(propId);
                    break;
                case InteractableThemePropertyValueTypes.ShaderFloat:
                    startValue.Float = propertyBlock.GetFloat(propId);
                    break;
                case InteractableThemePropertyValueTypes.shaderRange:
                    startValue.Float = propertyBlock.GetFloat(propId);
                    break;
                default:
                    break;
            }

            return startValue;
        }

        public static float GetFloat(GameObject host, string propId)
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

        public static MaterialPropertyBlock SetFloat(MaterialPropertyBlock block, float value, string propId)
        {
            if (block == null)
                return null;

            block.SetFloat(propId, value);
            return block;
        }

        public static Color GetColor(GameObject host, string propId)
        {
            if (host == null)
                return Color.white;

            MaterialPropertyBlock block = InteractableThemeShaderUtils.GetPropertyBlock(host);
            return block.GetVector(propId);
        }

        public static MaterialPropertyBlock SetColor(MaterialPropertyBlock block, Color color, string propId)
        {
            if (block == null)
                return null;

            block.SetColor(propId, color);
            return block;

        }
    }
}
