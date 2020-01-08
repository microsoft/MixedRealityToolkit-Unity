// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Theme Engine to change the texture on a GameObject based on state changes
    /// </summary>
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
                        TargetShader = StandardShaderUtility.MrtkStandardShader,
                        ShaderPropertyName = DefaultShaderProperty,
                    },
                },
                CustomProperties = new List<ThemeProperty>(),
            };
        }
    }
}
