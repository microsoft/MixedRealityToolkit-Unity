// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractableColorChildrenTheme : InteractableShaderTheme
    {
        public struct BlocksAndRenderer
        {
            public MaterialPropertyBlock Block;
            public Renderer Renderer;
        }

        private List<BlocksAndRenderer> propertyBlocks;

        public InteractableColorChildrenTheme()
        {
            Types = new Type[] {  };
            Name = "Color Children Theme";
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

            propertyBlocks = new List<BlocksAndRenderer>();
            Renderer[] list = host.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < list.Length; i++)
            {
                MaterialPropertyBlock block = InteractableThemeShaderUtils.GetMaterialPropertyBlock(list[i].gameObject, shaderProperties.ToArray());
                BlocksAndRenderer bAndR = new BlocksAndRenderer();
                bAndR.Renderer = list[i];
                bAndR.Block = block;

                propertyBlocks.Add(bAndR);
            }
        }

        public override InteractableThemePropertyValue GetProperty(InteractableThemeProperty property)
        {
            InteractableThemePropertyValue color = new InteractableThemePropertyValue();

            string propId = property.GetShaderPropId();

            if (propertyBlocks.Count > 0)
            {
                BlocksAndRenderer bAndR = propertyBlocks[0];
                color.Color = bAndR.Block.GetVector(propId);
            }

            return color;
        }

        public override void SetValue(InteractableThemeProperty property, int index, float percentage)
        {
            Color color = Color.Lerp(property.StartValue.Color, property.Values[index].Color, percentage);

            string propId = property.GetShaderPropId();

            for (int i = 0; i < propertyBlocks.Count; i++)
            {
                BlocksAndRenderer bAndR = propertyBlocks[i];
                bAndR.Block.SetColor(propId, color);
                bAndR.Renderer.SetPropertyBlock(bAndR.Block);
                propertyBlocks[i] = bAndR;
            }
        }
    }
}
