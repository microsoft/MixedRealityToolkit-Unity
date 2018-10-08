// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    /// <summary>
    /// Base class for themes
    /// </summary>
    
    [System.Serializable]
    public class ThemeProperty
    {
        public string Name;
        public ThemePropertyValueTypes Type;
        public List<ThemePropertyValue> Values;
        public ThemePropertyValue StartValue;
        public int PropId;
        public List<ShaderProperties> ShaderOptions;
        public List<string> ShaderOptionNames;
        public ThemePropertyValue Default;
        public string ShaderName;

        public string GetShaderPropId()
        {
            if (ShaderOptionNames.Count > PropId)
            {
                return ShaderOptionNames[PropId];
            }

            return "_Color";
        }
    }
}
