// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractableShaderUguiTheme : InteractableThemeBase
    {
        private static InteractableThemePropertyValue emptyValue = new InteractableThemePropertyValue();

        protected List<ShaderProperties> shaderProperties;
        protected Material material;

        private InteractableThemePropertyValue startValue = new InteractableThemePropertyValue();

        public InteractableShaderUguiTheme()
        {
            Types = new Type[] { typeof(Image) };
            Name = "Shader Float Ugui";
            ThemeProperties.Add(
                new InteractableThemeProperty()
                {
                    Name = "Shader",
                    Type = InteractableThemePropertyValueTypes.ShaderFloat,
                    Values = new List<InteractableThemePropertyValue>(),
                    Default = new InteractableThemePropertyValue() { Float = 0 }
                });
        }

        /// <inheritdoc />
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

            material = host.GetComponent<Image>()?.material;
        }

        /// <inheritdoc />
        public override void SetValue(InteractableThemeProperty property, int index, float percentage)
        {
            if (Host == null)
                return;

            int propId = property.GetShaderPropertyId();
            float newValue;
            switch (property.Type)
            {
                case InteractableThemePropertyValueTypes.Color:
                    Color newColor = Color.Lerp(property.StartValue.Color, property.Values[index].Color, percentage);
                    material.SetColor(propId, newColor);
                    break;
                case InteractableThemePropertyValueTypes.ShaderFloat:
                    newValue = LerpFloat(property.StartValue.Float, property.Values[index].Float, percentage);
                    material.SetFloat(propId, newValue);
                    break;
                case InteractableThemePropertyValueTypes.ShaderRange:
                    newValue = LerpFloat(property.StartValue.Float, property.Values[index].Float, percentage);
                    material.SetFloat(propId, newValue);
                    break;
                default:
                    break;
            }
        }

        /// <inheritdoc />
        public override InteractableThemePropertyValue GetProperty(InteractableThemeProperty property)
        {
            if (Host == null)
                return emptyValue;

            startValue.Reset();

            int propId = property.GetShaderPropertyId();
            switch (property.Type)
            {
                case InteractableThemePropertyValueTypes.Color:
                    startValue.Color = material.GetVector(propId);
                    break;
                case InteractableThemePropertyValueTypes.ShaderFloat:
                    startValue.Float = material.GetFloat(propId);
                    break;
                case InteractableThemePropertyValueTypes.ShaderRange:
                    startValue.Float = material.GetFloat(propId);
                    break;
                default:
                    break;
            }

            return startValue;
        }
    }
}
