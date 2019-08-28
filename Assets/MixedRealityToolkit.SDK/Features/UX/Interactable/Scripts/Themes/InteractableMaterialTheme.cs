// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractableMaterialTheme : InteractableThemeBase
    {
        private Material material = null;
        private Renderer renderer;

        public InteractableMaterialTheme()
        {
            Types = new Type[] { typeof(Renderer) };
            Name = "Material Theme";
            NoEasing = true;
            StateProperties = GetDefaultStateProperties();
        }

        /// <inheritdoc />
        public override List<ThemeStateProperty> GetDefaultStateProperties()
        {
            return new List<ThemeStateProperty>()
            {
                new ThemeStateProperty()
                {
                    Name = "Material",
                    Type = ThemePropertyTypes.Material,
                    Values = new List<ThemePropertyValue>(),
                    Default = new ThemePropertyValue() { Material = null }
                }
            };
        }

        /// <inheritdoc />
        public override List<ThemeProperty> GetDefaultThemeProperties()
        {
            return new List<ThemeProperty>();
        }

        /// <inheritdoc />
        public override void Init(GameObject host, ThemeDefinition settings)
        {
            base.Init(host, settings);

            renderer = Host.GetComponent<Renderer>();
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();

            material = renderer.material;
            start.Material = material;
            return start;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            Host.SetActive(property.Values[index].Bool);

            material = property.Values[index].Material;
            renderer.material = material;
        }
    }
}
