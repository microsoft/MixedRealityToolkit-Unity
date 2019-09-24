// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// This class is used to apply Color changes to shader properties on Unity UI based elements, since they do not support MaterialPropertyBlocks. 
    /// Use this only on a Unity UI based target component.
    /// </summary>
    public class ShaderUnityUiColorTheme : ShaderUnityUiTheme
    {
        private ThemePropertyValue startValue = new ThemePropertyValue();

        private const string DefaultShaderProperty = "_Color";

        public ShaderUnityUiColorTheme()
        {
            Types = new Type[] { typeof(Graphic) };
            Name = "Shader Color Ugui";
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
                        Name = "Shader Value",
                        Type = ThemePropertyTypes.Color,
                        Values = new List<ThemePropertyValue>(),
                        Default = new ThemePropertyValue() { Color = Color.white},
                        TargetShader = StandardShaderUtility.MrtkStandardShader,
                        ShaderPropertyName = DefaultShaderProperty,
                    },
                },
                CustomProperties = new List<ThemeProperty>(),
            };
        }
    }
}
