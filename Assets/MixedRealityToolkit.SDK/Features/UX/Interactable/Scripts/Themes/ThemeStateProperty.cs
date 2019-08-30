// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Base class for themes
    /// </summary>

    [System.Serializable]
    public class ThemeStateProperty
    {
        // TODO: Troy Add comments
        public string Name;
        public ThemePropertyTypes Type;
        public List<ThemePropertyValue> Values;
        public ThemePropertyValue StartValue;
        public ThemePropertyValue Default;

        public Shader TargetShader;
        public string ShaderPropertyName;
        [System.NonSerialized]
        protected int ShaderPropertyID = -1;

        private static readonly ThemePropertyTypes[] ShaderTypes =
            { ThemePropertyTypes.Color, ThemePropertyTypes .ShaderFloat, ThemePropertyTypes.ShaderRange};

        public static bool IsShaderPropertyType(ThemePropertyTypes type)
        {
            return ShaderTypes.Contains(type);
        }

        /// <summary>
        /// Lazy loads shader property ID from Unity for the ShaderPropertyName
        /// </summary>
        /// <returns>integer key for current shader property to get/set shader values</returns>
        public int GetShaderPropertyId()
        {
            if (ShaderPropertyID == -1)
            {
                ShaderPropertyID = Shader.PropertyToID(ShaderPropertyName);
            }

            return ShaderPropertyID;
        }


        // TODO: Troy Mark as protected but still serializable? and then can also modify to obsolete?
        public int PropId; // i.e OptionIndex
        public List<ShaderProperties> ShaderOptions;
        public List<string> ShaderOptionNames;
        public string ShaderName;

        public void MigrateData()
        {
            if (ShaderOptions != null && ShaderOptions.Count > 0)
            {
                TargetShader = Shader.Find(ShaderName);
                ShaderPropertyName = ShaderOptionNames[PropId];
            }
        }
    }
}
