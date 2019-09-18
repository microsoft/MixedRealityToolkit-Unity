// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractableShaderUguiTheme : InteractableThemeBase
    {
        /// <inheritdoc />
        public override bool AreShadersSupported => true;

        private static ThemePropertyValue emptyValue = new ThemePropertyValue();

        protected List<ThemeStateProperty> shaderProperties;
        protected Material material;

        private ThemePropertyValue startValue = new ThemePropertyValue();

        protected const string DefaultShaderProperty = "_Color";

        public InteractableShaderUguiTheme()
        {
            Types = new Type[] { typeof(Image) };
            Name = "Shader Float Ugui";
        }

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
        public override void Init(GameObject host, ThemeDefinition settings)
        {
            base.Init(host, settings);

            shaderProperties = new List<ThemeStateProperty>();
            foreach (var prop in StateProperties)
            {
                if (ThemeStateProperty.IsShaderPropertyType(prop.Type))
                {
                    shaderProperties.Add(prop);
                }
            }

            material = host.GetComponent<Image>()?.material;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            if (Host == null)
                return;

            int propId = property.GetShaderPropertyId();
            float newValue;
            switch (property.Type)
            {
                case ThemePropertyTypes.Color:
                    Color newColor = Color.Lerp(property.StartValue.Color, property.Values[index].Color, percentage);
                    material.SetColor(propId, newColor);
                    break;
                case ThemePropertyTypes.ShaderFloat:
                    newValue = LerpFloat(property.StartValue.Float, property.Values[index].Float, percentage);
                    material.SetFloat(propId, newValue);
                    break;
                case ThemePropertyTypes.ShaderRange:
                    newValue = LerpFloat(property.StartValue.Float, property.Values[index].Float, percentage);
                    material.SetFloat(propId, newValue);
                    break;
                default:
                    break;
            }
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            if (Host == null)
                return emptyValue;

            startValue.Reset();

            int propId = property.GetShaderPropertyId();
            switch (property.Type)
            {
                case ThemePropertyTypes.Color:
                    startValue.Color = material.GetVector(propId);
                    break;
                case ThemePropertyTypes.ShaderFloat:
                    startValue.Float = material.GetFloat(propId);
                    break;
                case ThemePropertyTypes.ShaderRange:
                    startValue.Float = material.GetFloat(propId);
                    break;
                default:
                    break;
            }

            return startValue;
        }
    }
}
