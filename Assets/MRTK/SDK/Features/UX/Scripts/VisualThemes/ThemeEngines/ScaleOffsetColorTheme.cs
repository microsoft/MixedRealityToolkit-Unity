// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Theme Engine to control initialized GameObject's scale, local position offset, and color based on state changes
    /// For color, will try to set on first available text object in order of TextMesh, Text, TextMeshPro, and TextMeshProUGUI
    /// If none found, then Theme will target first Renderer component available and target the associated shader property found in ThemeDefinition
    /// </summary>
    public class ScaleOffsetColorTheme : InteractableColorTheme
    {
        protected Vector3 originalPosition;
        protected Vector3 originalScale;
        protected Transform hostTransform;

        public ScaleOffsetColorTheme()
        {
            Types = new Type[] { typeof(Transform), typeof(TextMesh), typeof(TextMesh), typeof(TextMeshPro), typeof(TextMeshProUGUI), typeof(Renderer) };
            Name = "Default: Scale, Offset, Color";
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
                        Name = "Scale",
                        Type = ThemePropertyTypes.Vector3,
                        Values = new List<ThemePropertyValue>(),
                        Default = new ThemePropertyValue() { Vector3 = Vector3.one }
                    },
                    new ThemeStateProperty()
                    {
                        Name = "Offset",
                        Type = ThemePropertyTypes.Vector3,
                        Values = new List<ThemePropertyValue>(),
                        Default = new ThemePropertyValue() { Vector3 = Vector3.zero }
                    },
                    new ThemeStateProperty()
                    {
                        Name = "Color",
                        Type = ThemePropertyTypes.Color,
                        Values = new List<ThemePropertyValue>(),
                        Default = new ThemePropertyValue() { Color = Color.white },
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
            hostTransform = host.transform;
            originalPosition = hostTransform.localPosition;
            originalScale = hostTransform.localScale;

            base.Init(host, settings);
        }

        /// <inheritdoc />
        public override void Reset()
        {
            hostTransform.localPosition = originalPosition;
            hostTransform.localScale = originalScale;
            
            base.Reset();
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();

            switch (property.Name)
            {
                case "Scale":
                    start.Vector3 = hostTransform.localScale;
                    break;
                case "Offset":
                    start.Vector3 = hostTransform.localPosition;
                    break;
                case "Color":
                    start = base.GetProperty(property);
                    break;
                default:
                    break;
            }
            return start;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            switch (property.Name)
            {
                case "Scale":
                    hostTransform.localScale = Vector3.Lerp(property.StartValue.Vector3, Vector3.Scale(originalScale, property.Values[index].Vector3), percentage);
                    break;
                case "Offset":
                    hostTransform.localPosition = Vector3.Lerp(property.StartValue.Vector3, originalPosition + property.Values[index].Vector3, percentage);
                    break;
                case "Color":
                    base.SetValue(property, index, percentage);
                    break;
                default:
                    break;
            }
        }
    }
}
