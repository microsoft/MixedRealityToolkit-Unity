using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    public class ColorChildrenTheme : ShaderTheme
    {
        public struct BlocksAndRenderer
        {
            public MaterialPropertyBlock Block;
            public Renderer Renderer;
        }

        private List<BlocksAndRenderer> propertyBlocks;

        public ColorChildrenTheme()
        {
            Types = new Type[] {  };
            Name = "Color Children Theme";
            ThemeProperties = new List<ThemeProperty>();
            ThemeProperties.Add(
                new ThemeProperty()
                {
                    Name = "Color",
                    Type = ThemePropertyValueTypes.Color,
                    Values = new List<ThemePropertyValue>(),
                    Default = new ThemePropertyValue() { Color = Color.white}
                });
        }

        public override void Init(GameObject host, ThemePropertySettings settings)
        {
            base.Init(host, settings);

            shaderProperties = new List<ShaderProperties>();
            for (int i = 0; i < ThemeProperties.Count; i++)
            {
                ThemeProperty prop = ThemeProperties[i];
                if (prop.ShaderOptions.Count > 0)
                {
                    shaderProperties.Add(prop.ShaderOptions[prop.PropId]);
                }
            }

            propertyBlocks = new List<BlocksAndRenderer>();
            Renderer[] list = host.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < list.Length; i++)
            {
                MaterialPropertyBlock block = GetMaterialPropertyBlock(list[i].gameObject, shaderProperties.ToArray());
                BlocksAndRenderer bAndR = new BlocksAndRenderer();
                bAndR.Renderer = list[i];
                bAndR.Block = block;

                propertyBlocks.Add(bAndR);
            }
        }

        public override ThemePropertyValue GetProperty(ThemeProperty property)
        {
            ThemePropertyValue color = new ThemePropertyValue();

            string propId = property.GetShaderPropId();

            if (propertyBlocks.Count > 0)
            {
                BlocksAndRenderer bAndR = propertyBlocks[0];
                color.Color = bAndR.Block.GetVector(propId);
            }

            return color;
        }

        public override void SetValue(ThemeProperty property, int index, float percentage)
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
