// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractableTextureTheme : InteractableThemeBase
    {
        /// <inheritdoc />
        public override bool IsEasingSupported => false;

        private MaterialPropertyBlock propertyBlock;
        private Renderer renderer;

        public InteractableTextureTheme()
        {
            Types = new Type[] { typeof(Renderer) };
            Name = "Texture Theme";
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
                        Name = "Texture",
                        Type = ThemePropertyTypes.Texture,
                        Values = new List<ThemePropertyValue>(),
                        Default = new ThemePropertyValue() { Texture = null }
                    },
                },
                CustomProperties = new List<ThemeProperty>(),
            };
        }

        /// <inheritdoc />
        public override void Init(GameObject host, ThemeDefinition settings)
        {
            base.Init(host, settings);

            propertyBlock = InteractableThemeShaderUtils.GetPropertyBlock(host);

            renderer = Host.GetComponent<Renderer>();
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();
            start.Texture = propertyBlock.GetTexture("_MainTex");
            return start;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            propertyBlock.SetTexture("_MainTex", property.Values[index].Texture);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }
}
