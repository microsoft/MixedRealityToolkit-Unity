// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

        private List<BlocksAndRenderer> propertyBlocks = new List<BlocksAndRenderer>();
        private List<BlocksAndRenderer> originalPropertyBlocks = new List<BlocksAndRenderer>();

        private Renderer[] childrenRenderers;

        protected new const string DefaultShaderProperty = "_Color";

        public InteractableColorChildrenTheme()
        {
            Types = Array.Empty<Type>();
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

            childrenRenderers = host.GetComponentsInChildren<Renderer>();

            for (int i = 0; i < childrenRenderers.Length; i++)
            {
                MaterialPropertyBlock block = InteractableThemeShaderUtils.InitMaterialPropertyBlock(childrenRenderers[i].gameObject, shaderProperties);
                BlocksAndRenderer bAndR = new BlocksAndRenderer();
                bAndR.Renderer = childrenRenderers[i];
                bAndR.Block = block;

                propertyBlocks.Add(bAndR);
                originalPropertyBlocks.Add(bAndR);
            }
        }

        /// <inheritdoc />
        public override void Reset()
        {
            foreach (var bAndR in originalPropertyBlocks)
            {
                bAndR.Renderer.SetPropertyBlock(bAndR.Block);
            }
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue color = new ThemePropertyValue();

            if (propertyBlocks.Count > 0)
            {
                int propId = property.GetShaderPropertyId();
                BlocksAndRenderer bAndR = propertyBlocks[0];
                color.Color = bAndR.Block.GetVector(propId);
            }

            return color;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            SetColor(property, Color.Lerp(property.StartValue.Color, property.Values[index].Color, percentage));
        }

        /// <inheritdoc />
        protected override void SetValue(ThemeStateProperty property, ThemePropertyValue value)
        {
            SetColor(property, value.Color);
        }

        private void SetColor(ThemeStateProperty property, Color color)
        {
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
