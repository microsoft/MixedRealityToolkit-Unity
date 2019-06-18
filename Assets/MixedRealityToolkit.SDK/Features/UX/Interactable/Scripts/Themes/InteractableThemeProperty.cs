// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Base class for themes
    /// </summary>

    [System.Serializable]
    public class InteractableThemeProperty
    {
        public string Name;
        public InteractableThemePropertyValueTypes Type;
        public List<InteractableThemePropertyValue> Values;
        public InteractableThemePropertyValue StartValue;
        public int PropId; // i.e OptionIndex
        public List<ShaderProperties> ShaderOptions;
        public List<string> ShaderOptionNames;
        public InteractableThemePropertyValue Default;
        public string ShaderName;

        private List<int> ShaderPropertyIDs = null;
        private const string DefaultProperty = "_Color";

        /// <summary>
        /// This method gets the integer key assigned by Unity at runtime for the current shader property. 
        /// It will also lazy load the array of possible key values on first access using Unity's Shader.PropertyToID()
        /// It is generally preferred to use the integer key over the string key with Unity to avoid perf cost for the dictionary lookup on every get/set.
        /// ex: On SetFloat(string key), Unity will perform Shader.PropertyToID() itself every call
        /// </summary>
        /// <returns>integer key for current shader property to get/set shader values. Returns default backup property in case of failure</returns>
        public int GetShaderPropertyId()
        {
            // Lazy load Shader Properties
            if (ShaderPropertyIDs == null)
            {
                ShaderPropertyIDs = new List<int>(ShaderOptionNames.Count);
                for (int i = 0; i < this.ShaderOptionNames.Count; i++)
                {
                    ShaderPropertyIDs.Add(Shader.PropertyToID(this.ShaderOptionNames[i]));
                }
            }

            if (ShaderPropertyIDs.Count > PropId)
            {
                return ShaderPropertyIDs[PropId];
            }

            return Shader.PropertyToID(DefaultProperty);
        }

        /// <summary>
        /// Get the current shader property name. Again it is preferred to utilize the integer key over the string key in Unity
        /// </summary>
        /// <returns>string name of current property. Returns default backup property in case of failure</returns>
        public string GetShaderPropertyName()
        {
            if (ShaderOptionNames.Count > PropId)
            {
                return ShaderOptionNames[PropId];
            }

            return DefaultProperty;
        }
    }
}
