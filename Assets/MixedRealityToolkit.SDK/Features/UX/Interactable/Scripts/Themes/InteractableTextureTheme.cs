// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.Themes
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
            ThemeProperties.Add(
                new InteractableThemeProperty()
                {
                    Name = "Texture",
                    Type = InteractableThemePropertyValueTypes.Texture,
                    Values = new List<InteractableThemePropertyValue>(),
                    Default = new InteractableThemePropertyValue() { Texture = null }
                });
        }

        public override void Init(GameObject host, InteractableThemePropertySettings settings)
        {
            base.Init(host, settings);
            propertyBlock = InteractableThemeShaderUtils.GetMaterialPropertyBlock(host, new ShaderProperties[0]);
            renderer = Host.GetComponent<Renderer>();
        }

        public override InteractableThemePropertyValue GetProperty(InteractableThemeProperty property)
        {
            InteractableThemePropertyValue start = new InteractableThemePropertyValue();
            start.Texture = propertyBlock.GetTexture("_MainTex");
            return start;
        }

        public override void SetValue(InteractableThemeProperty property, int index, float percentage)
        {
            propertyBlock.SetTexture("_MainTex", property.Values[index].Texture);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }
}
