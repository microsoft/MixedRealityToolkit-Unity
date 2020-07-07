// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

        protected MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        protected List<ThemeStateProperty> shaderProperties;
        protected Renderer renderer;
        private Graphic graphic;

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
                CustomProperties = new List<ThemeProperty>(),
            };
        }

        /// <inheritdoc />
        public override void Init(GameObject host, ThemeDefinition definition)
        {
            renderer = host.GetComponent<Renderer>();
            graphic = host.GetComponent<Graphic>();

            base.Init(host, definition);

            shaderProperties = new List<ThemeStateProperty>();
            foreach (var prop in StateProperties)
            {
                if (ThemeStateProperty.IsShaderPropertyType(prop.Type))
                {
                    shaderProperties.Add(prop);
                }
            }

            if (renderer != null)
            {
                propertyBlock = InteractableThemeShaderUtils.InitMaterialPropertyBlock(host, shaderProperties);
            }
            else if (graphic != null)
            {
                UIMaterialInstantiator.TryCreateMaterialCopy(graphic);
            }

            // Need to update reset history tracking now that property blocks are populated correctly via above code
            var keys = new List<ThemeStateProperty>(originalStateValues.Keys);
            foreach (var value in keys)
            {
                originalStateValues[value] = GetProperty(value);
            }
        }
        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            var result = new ThemePropertyValue();

            int propId = property.GetShaderPropertyId();

            if (renderer != null)
            {
                renderer.GetPropertyBlock(propertyBlock);
                switch (property.Type)
                {
                    case ThemePropertyTypes.Color:
                        result.Color = propertyBlock.GetVector(propId);
                        break;
                    case ThemePropertyTypes.Texture:
                        result.Texture = propertyBlock.GetTexture(propId);
                        break;
                    case ThemePropertyTypes.ShaderFloat:
                    case ThemePropertyTypes.ShaderRange:
                        result.Float = propertyBlock.GetFloat(propId);
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
                        result.Color = graphic.material.GetVector(propId);
                        break;
                    case ThemePropertyTypes.Texture:
                        result.Texture = graphic.material.GetTexture(propId);
                        break;
                    case ThemePropertyTypes.ShaderFloat:
                    case ThemePropertyTypes.ShaderRange:
                        result.Float = graphic.material.GetFloat(propId);
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            var propValue = property.Values[index];

            Color newColor = property.StartValue.Color;
            float newFloatValue = property.StartValue.Float;

            switch (property.Type)
            {
                case ThemePropertyTypes.Color:
                    newColor = Color.Lerp(newColor, propValue.Color, percentage);
                    break;
                case ThemePropertyTypes.ShaderFloat:
                case ThemePropertyTypes.ShaderRange:
                    newFloatValue = LerpFloat(newFloatValue, propValue.Float, percentage);
                    break;
                default:
                    break;
            }

            SetShaderValue(property, newColor, propValue.Texture, newFloatValue);
        }

        /// <inheritdoc />
        protected override void SetValue(ThemeStateProperty property, ThemePropertyValue value)
        {
            SetShaderValue(property, value.Color, value.Texture, value.Float);
        }

        private void SetShaderValue(ThemeStateProperty property, Color color, Texture tex, float floatValue)
        {
            int propId = property.GetShaderPropertyId();

            if (renderer != null)
            {
                renderer.GetPropertyBlock(propertyBlock);

                switch (property.Type)
                {
                    case ThemePropertyTypes.Color:
                        propertyBlock.SetColor(propId, color);
                        break;
                    case ThemePropertyTypes.Texture:
                        if (tex != null)
                        {
                            propertyBlock.SetTexture(propId, tex);
                        }
                        break;
                    case ThemePropertyTypes.ShaderFloat:
                    case ThemePropertyTypes.ShaderRange:
                        propertyBlock.SetFloat(propId, floatValue);
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
                        graphic.material.SetColor(propId, color);
                        break;
                    case ThemePropertyTypes.Texture:
                        graphic.material.SetTexture(propId, tex);
                        break;
                    case ThemePropertyTypes.ShaderFloat:
                    case ThemePropertyTypes.ShaderRange:
                        graphic.material.SetFloat(propId, floatValue);
                        break;
                    default:
                        break;
                }
            }
        }

        #region Obsolete

        [System.Obsolete("GetFloat is no longer supported. Access the material block directly on the GameObject provided.")]
        public static float GetFloat(GameObject host, int propId)
        {
            if (host == null)
            {
                return 0;
            }

            MaterialPropertyBlock block = InteractableThemeShaderUtils.GetPropertyBlock(host);
            return block.GetFloat(propId);
        }

        [System.Obsolete("GetColor is no longer supported. Access the material block directly on the GameObject provided.")]
        public static Color GetColor(GameObject host, int propId)
        {
            if (host == null)
            {
                return Color.white;
            }

            MaterialPropertyBlock block = InteractableThemeShaderUtils.GetPropertyBlock(host);
            return block.GetVector(propId);
        }

        #endregion
    }
}
