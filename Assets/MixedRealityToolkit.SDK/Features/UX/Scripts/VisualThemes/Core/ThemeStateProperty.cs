// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Class to store information about a Theme property that contains values per available state
    /// </summary>
    [Serializable]
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

        // Properties below are outdated. They remain only for backward compatibility and migration purposes
        [SerializeField]
        private int PropId = -1; // i.e OptionIndex

        [SerializeField]
        private List<ShaderProperties> ShaderOptions = new List<ShaderProperties>();

        [SerializeField]
        private List<string> ShaderOptionNames = new List<string>();

        [SerializeField]
        private string ShaderName = "";

        // TODO: Troy - Add comment
        public bool MigrateShaderData()
        {
            if (ShaderOptions != null && ShaderOptions.Count > 0)
            {
                TargetShader = Shader.Find(ShaderName);
                ShaderPropertyName = ShaderOptionNames[PropId];

                // Invalidate old fields so we can deprecate in future
                ShaderOptions.Clear();
                ShaderOptionNames.Clear();
                ShaderName = string.Empty;

                return true;
            }

            return false;
        }
    }
}
