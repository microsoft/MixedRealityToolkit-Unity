// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Theme Engine to control the value of a particular Shader Property based on state changes
    /// Targets the first Renderer component on the initialized GameObject and use MaterialPropertyBlocks
    /// </summary>
    public class InteractableShaderTheme : InteractableThemeBase
    {
        /// <inheritdoc />
        public override bool AreShadersSupported => true;

        private static ThemePropertyValue emptyValue = new ThemePropertyValue();

        protected MaterialPropertyBlock propertyBlock;
        protected List<ThemeStateProperty> shaderProperties;
        protected Renderer renderer;
        private Graphic graphic;

        private ThemePropertyValue startValue = new ThemePropertyValue();

        protected const string DefaultShaderProperty = "_Color";

        public InteractableShaderTheme()
        {
            Types = new Type[] { typeof(Renderer), typeof(Graphic) };
            Name = "Shader Float";
        }

        /// <inheritdoc />
        public override ThemeDefinition GetDefaultThemeDefinition()
        {
            return new ThemeDefinition()
            {
                ThemeType = GetType(),
                StateProperties = new List<ThemeStateProperty>()
                {
                    new ThemeStateProperty()
                    {
                        Name = "Shader Value",
                        Type = ThemePropertyTypes.ShaderFloat,
                        Values = new List<ThemePropertyValue>(),
                        Default = new ThemePropertyValue() { Float = 0},
                        TargetShader = StandardShaderUtility.MrtkStandardShader,
                        ShaderPropertyName = DefaultShaderProperty,
                    },
                },
                CustomProperties =  new List<ThemeProperty>(),
            };
        }

        /// <inheritdoc />
        public override void Init(GameObject host, ThemeDefinition definition)
        {
            base.Init(host, definition);

            shaderProperties = new List<ThemeStateProperty>();
            foreach (var prop in StateProperties)
            {
                if (ThemeStateProperty.IsShaderPropertyType(prop.Type))
                {
                    shaderProperties.Add(prop);
                }
            }
            renderer = Host.GetComponent<Renderer>();
            graphic = Host.GetComponent<Graphic>();
            if (renderer != null)
            {
                propertyBlock = InteractableThemeShaderUtils.InitMaterialPropertyBlock(host, shaderProperties);
            }
            else if (graphic != null)
            {
                UIMaterialInstantiator.TryCreateMaterialCopy(graphic);
            }
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            int propId = property.GetShaderPropertyId();
            var propValue = property.Values[index];
            Color newColor = property.StartValue.Color;
            float newFloatValue = 0;

            switch (property.Type)
            {
                case ThemePropertyTypes.Color:
                    newColor = Color.Lerp(property.StartValue.Color, propValue.Color, percentage);
                    break;
                case ThemePropertyTypes.ShaderFloat:
                case ThemePropertyTypes.ShaderRange:
                    newFloatValue = LerpFloat(property.StartValue.Float, propValue.Float, percentage);
                    break;
                default:
                    break;
            }

            if (renderer != null)
            {
                renderer.GetPropertyBlock(propertyBlock);

                switch (property.Type)
                {
                    case ThemePropertyTypes.Color:
                        propertyBlock.SetColor(propId, newColor);
                        break;
                    case ThemePropertyTypes.Texture:
                        propertyBlock.SetTexture(propId, propValue.Texture);
                        break;
                    case ThemePropertyTypes.ShaderFloat:
                    case ThemePropertyTypes.ShaderRange:
                        propertyBlock.SetFloat(propId, newFloatValue);
                        break;
                    default:
                        break;
                }

                renderer.SetPropertyBlock(propertyBlock);
            }
            else if (graphic != null)
            {
                switch (property.Type)
                {
                    case ThemePropertyTypes.Color:
                        graphic.material.SetColor(propId, newColor);
                        break;
                    case ThemePropertyTypes.Texture:
                        graphic.material.SetTexture(propId, propValue.Texture);
                        break;
                    case ThemePropertyTypes.ShaderFloat:
                    case ThemePropertyTypes.ShaderRange:
                        graphic.material.SetFloat(propId, newFloatValue);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            startValue.Reset();
            int propId = property.GetShaderPropertyId();

            if (renderer != null)
            {
                renderer.GetPropertyBlock(propertyBlock);
                switch (property.Type)
                {
                    case ThemePropertyTypes.Color:
                        startValue.Color = propertyBlock.GetVector(propId);
                        break;
                    case ThemePropertyTypes.Texture:
                        startValue.Texture = propertyBlock.GetTexture(propId);
                        break;
                    case ThemePropertyTypes.ShaderFloat:
                    case ThemePropertyTypes.ShaderRange:
                        startValue.Float = propertyBlock.GetFloat(propId);
                        break;
                    default:
                        break;
                }
            }
            else if (graphic != null)
            {
                switch (property.Type)
                {
                    case ThemePropertyTypes.Color:
                        startValue.Color = graphic.material.GetVector(propId);
                        break;
                    case ThemePropertyTypes.Texture:
                        startValue.Texture = graphic.material.GetTexture(propId);
                        break;
                    case ThemePropertyTypes.ShaderFloat:
                    case ThemePropertyTypes.ShaderRange:
                        startValue.Float = graphic.material.GetFloat(propId);
                        break;
                    default:
                        break;
                }
            }
            return startValue;
        }

        public static float GetFloat(GameObject host, int propId)
        {
            if (host == null)
            {
                return 0;
            }

            MaterialPropertyBlock block = InteractableThemeShaderUtils.GetPropertyBlock(host);
            return block.GetFloat(propId);
        }

        public static Color GetColor(GameObject host, int propId)
        {
            if (host == null)
            {
                return Color.white;
            }

            MaterialPropertyBlock block = InteractableThemeShaderUtils.GetPropertyBlock(host);
            return block.GetVector(propId);
        }
    }
}
