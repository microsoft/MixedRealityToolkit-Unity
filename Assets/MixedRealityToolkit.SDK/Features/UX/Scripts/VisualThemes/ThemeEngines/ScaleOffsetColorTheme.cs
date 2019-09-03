// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class ScaleOffsetColorTheme : InteractableColorTheme
    {
        protected Vector3 startPosition;
        protected Vector3 startScale;
        protected Transform hostTransform;

        public override void Init(GameObject host, ThemeDefinition settings)
        {
            base.Init(host, settings);
            hostTransform = Host.transform;
            startPosition = hostTransform.localPosition;
            startScale = hostTransform.localScale;
        }

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
                        Default = new ThemePropertyValue() { Color = Color.white }
                    },
                },
                CustomProperties = new List<ThemeProperty>(),
            };
        }

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

        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            switch (property.Name)
            {
                case "Scale":
                    hostTransform.localScale = Vector3.Lerp(property.StartValue.Vector3, Vector3.Scale(startScale, property.Values[index].Vector3), percentage);
                    break;
                case "Offset":
                    hostTransform.localPosition = Vector3.Lerp(property.StartValue.Vector3, startPosition + property.Values[index].Vector3, percentage);
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
