// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractableTextureTheme : InteractableThemeBase
    {
        private MaterialPropertyBlock propertyBlock;
        private Renderer renderer;

        public InteractableTextureTheme()
        {
            Types = new Type[] { typeof(Renderer) };
            Name = "Texture Theme";
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
                    Name = "Texture",
                    Type = ThemePropertyTypes.Texture,
                    Values = new List<ThemePropertyValue>(),
                    Default = new ThemePropertyValue() { Texture = null }
                },
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
            propertyBlock = InteractableThemeShaderUtils.GetMaterialPropertyBlock(host, new ShaderProperties[0]);
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
