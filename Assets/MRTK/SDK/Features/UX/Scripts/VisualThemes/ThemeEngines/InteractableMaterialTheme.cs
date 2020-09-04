// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Theme Engine to control the material used based on state changes
    /// Changes the material for the first Renderer component on the initialized GameObject
    /// </summary>
    public class InteractableMaterialTheme : InteractableThemeBase
    {
        /// <inheritdoc />
        public override bool IsEasingSupported => false;

        private Material initMaterial;
        private Material material = null;
        private Renderer renderer;

        public InteractableMaterialTheme()
        {
            Types = new Type[] { typeof(Renderer) };
            Name = "Material Theme";
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
                        Name = "Material",
                        Type = ThemePropertyTypes.Material,
                        Values = new List<ThemePropertyValue>(),
                        Default = new ThemePropertyValue() { Material = null }
                    },
                },
                CustomProperties = new List<ThemeProperty>(),
            };
        }

        /// <inheritdoc />
        public override void Init(GameObject host, ThemeDefinition settings)
        {
            renderer = host.GetComponent<Renderer>();
            if (renderer != null)
            {
                initMaterial = renderer.material;
            }
            else
            {
                Debug.LogError($"Host GameObject {host} does not have a {typeof(Renderer).Name} component. InteractableMaterialTheme cannot execute.");
            }

            base.Init(host, settings);
        }

        /// <inheritdoc />
        public override void Reset()
        {
            if (renderer != null)
            {
                renderer.material = initMaterial;
            }
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();

            if (renderer != null)
            {
                material = renderer.material;
                start.Material = material;
            }

            return start;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            SetValue(property, property.Values[index]);
        }

        /// <inheritdoc />
        protected override void SetValue(ThemeStateProperty property, ThemePropertyValue value)
        {
            if (renderer != null)
            {
                material = value.Material;
                renderer.material = material;
            }
        }
    }
}
