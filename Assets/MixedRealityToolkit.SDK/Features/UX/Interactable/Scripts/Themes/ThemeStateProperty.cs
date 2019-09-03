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
        /// <summary>
        /// Name of property, useful for comparisons and editor displaying
        /// </summary>
        public string Name;

        /// <summary>
        /// Type of value stored in this property
        /// </summary>
        public ThemePropertyTypes Type;

        /// <summary>
        /// List of values corresponding to every available state
        /// </summary>
        public List<ThemePropertyValue> Values;
        
        /// <summary>
        /// The starting value of this property
        /// </summary>
        public ThemePropertyValue StartValue;

        /// <summary>
        /// Default value to use for this property 
        /// </summary>
        public ThemePropertyValue Default;

        /// <summary>
        /// Shader to target for getting/setting values with this property, if applicable
        /// Supported by Themes which have AreShadersSupported set to true
        /// </summary>
        public Shader TargetShader;

        /// <summary>
        /// Name of the shader property, defined in the TargetShader, to utilize for getting/setting values with this property, if applicable
        /// Supported by Themes which have AreShadersSupported set to true
        /// </summary>
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
        [Obsolete("Utilize TargetShader and ShaderPropertyName instead")]
        private int PropId = -1; // i.e OptionIndex

        [SerializeField]
        [Obsolete("Utilize TargetShader and ShaderPropertyName instead")]
        private List<ShaderProperties> ShaderOptions = new List<ShaderProperties>();

        [SerializeField]
        [Obsolete("Utilize TargetShader and ShaderPropertyName instead")]
        private List<string> ShaderOptionNames = new List<string>();

        [SerializeField]
        [Obsolete("Utilize TargetShader and ShaderPropertyName instead")]
        private string ShaderName = "";

        /// <summary>
        /// This temporary function will migrate over the previously set shader data (via the now deprecated properties)
        /// to the new TargetShader and ShaderPropertyName properties
        /// </summary>
        public void MigrateShaderData()
        {
            // Old shader properties have been deprecated but need to ignore compiler errors for migration code
#pragma warning disable 612, 618
            if (ShaderOptions != null && ShaderOptions.Count > 0)
            {
                TargetShader = Shader.Find(ShaderName);
                ShaderPropertyName = ShaderOptionNames[PropId];
            }
#pragma warning restore 612, 618
        }
    }
}
