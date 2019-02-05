// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.Themes
{
    public class InteractableColorTheme : InteractableShaderTheme
    {
        public InteractableColorTheme()
        {
            Types = new Type[] { typeof(Renderer), typeof(TextMesh), typeof(Text) };
            Name = "Color Theme";
            ThemeProperties = new List<InteractableThemeProperty>();
            ThemeProperties.Add(
                new InteractableThemeProperty()
                {
                    Name = "Color",
                    Type = InteractableThemePropertyValueTypes.Color,
                    Values = new List<InteractableThemePropertyValue>(),
                    Default = new InteractableThemePropertyValue() { Color = Color.white}
                });
        }

        public override InteractableThemePropertyValue GetProperty(InteractableThemeProperty property)
        {
            InteractableThemePropertyValue color = new InteractableThemePropertyValue();
            TextMesh mesh = Host.GetComponent<TextMesh>();
            if (mesh != null)
            {
                color.Color = mesh.color;
                return color;
            }

            Text text = Host.GetComponent<Text>();
            if (text != null)
            {
                color.Color = text.color;
                return color;
            }

            return base.GetProperty(property);
        }

        public override void SetValue(InteractableThemeProperty property, int index, float percentage)
        {
            Color color = Color.Lerp(property.StartValue.Color, property.Values[index].Color, percentage);
            TextMesh mesh = Host.GetComponent<TextMesh>();
            if (mesh != null)
            {
                mesh.color = color;
                return;
            }

            Text text = Host.GetComponent<Text>();
            if (text != null)
            {
                text.color = color;
                return;
            }

           base.SetValue(property, index, percentage);

        }
    }
}
