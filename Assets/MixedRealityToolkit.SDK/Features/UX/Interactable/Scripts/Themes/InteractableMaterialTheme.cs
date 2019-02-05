// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.Themes
{
    public class InteractableMaterialTheme : InteractableThemeBase
    {
        private Material material = null;

        public InteractableMaterialTheme()
        {
            Types = new Type[] { typeof(Renderer) };
            Name = "Material Theme";
            NoEasing = true;
            ThemeProperties.Add(
                new InteractableThemeProperty()
                {
                    Name = "Material",
                    Type = InteractableThemePropertyValueTypes.Material,
                    Values = new List<InteractableThemePropertyValue>(),
                    Default = new InteractableThemePropertyValue() { Material = null }
                });
        }

        public override InteractableThemePropertyValue GetProperty(InteractableThemeProperty property)
        {
            InteractableThemePropertyValue start = new InteractableThemePropertyValue();
            Renderer renderer = Host.GetComponent<Renderer>();
            material = renderer.material;
            start.Material = material;
            return start;
        }

        public override void SetValue(InteractableThemeProperty property, int index, float percentage)
        {
            Host.SetActive(property.Values[index].Bool);

            Renderer renderer = Host.GetComponent<Renderer>();
            material = property.Values[index].Material;
            renderer.material = material;
        }
    }
}
