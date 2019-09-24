// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Theme engine to control the color for all Renderer children under the initialized GameObject based on state changes
    /// </summary>
    public class InteractableColorChildrenTheme : InteractableShaderTheme
    {
        public struct BlocksAndRenderer
        {
            public MaterialPropertyBlock Block;
            public Renderer Renderer;
        }

        private List<BlocksAndRenderer> propertyBlocks;

        protected new const string DefaultShaderProperty = "_Color";

        public InteractableColorChildrenTheme()
        {
            Types = new Type[] {  };
            Name = "Color Children Theme";
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
                        Name = "Color",
                        Type = ThemePropertyTypes.Color,
                        Values = new List<ThemePropertyValue>(),
                        Default = new ThemePropertyValue() { Color = Color.white},
                        TargetShader = StandardShaderUtility.MrtkStandardShader,
                        ShaderPropertyName = DefaultShaderProperty,
                    }
                },
                CustomProperties = new List<ThemeProperty>(),
            };
        }

        /// <inheritdoc />
        public override void Init(GameObject host, ThemeDefinition settings)
        {
            base.Init(host, settings);

            propertyBlocks = new List<BlocksAndRenderer>();
            Renderer[] list = host.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < list.Length; i++)
            {
                MaterialPropertyBlock block = InteractableThemeShaderUtils.InitMaterialPropertyBlock(list[i].gameObject, shaderProperties);
                BlocksAndRenderer bAndR = new BlocksAndRenderer();
                bAndR.Renderer = list[i];
                bAndR.Block = block;

                propertyBlocks.Add(bAndR);
            }
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue color = new ThemePropertyValue();

            int propId = property.GetShaderPropertyId();

            if (propertyBlocks.Count > 0)
            {
                BlocksAndRenderer bAndR = propertyBlocks[0];
                color.Color = bAndR.Block.GetVector(propId);
            }

            return color;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            Color color = Color.Lerp(property.StartValue.Color, property.Values[index].Color, percentage);

            int propId = property.GetShaderPropertyId();

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
