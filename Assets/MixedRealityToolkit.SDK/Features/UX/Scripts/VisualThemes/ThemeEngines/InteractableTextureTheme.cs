// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractableTextureTheme : InteractableShaderTheme
    {
        /// <inheritdoc />
        public override bool IsEasingSupported => false;

        protected new const string DefaultShaderProperty = "_MainTex";

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
                        Default = new ThemePropertyValue() { Texture = null },
                        TargetShader = Shader.Find(DefaultShaderName),
                        ShaderPropertyName = DefaultShaderProperty,
                    },
                },
                CustomProperties = new List<ThemeProperty>(),
            };
        }

        /// <inheritdoc />
        public override void Init(GameObject host, ThemeDefinition definition)
        {
            // Temporary workaround for backward compatbility
            // If any properties are targeting textures but do not have the appropriate TargetShader or property name set, then set defaults here
            foreach (var prop in definition.StateProperties)
            {
                if (prop.Type == ThemePropertyTypes.Texture && string.IsNullOrEmpty(prop.ShaderPropertyName))
                {
                    prop.TargetShader = Shader.Find(DefaultShaderName);
                    prop.ShaderPropertyName = DefaultShaderProperty;
                }
            }

            base.Init(host, definition);
        }
    }
}
