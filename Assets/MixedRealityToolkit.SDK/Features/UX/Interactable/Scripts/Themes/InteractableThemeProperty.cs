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

        public int GetShaderPropertyId()
        {
            // Lazy load Shader Properties
            if (ShaderPropertyIDs == null)
            {
                ShaderPropertyIDs = new List<int>(ShaderOptionNames.Count);
                for(int i = 0; i < this.ShaderOptionNames.Count; i++)
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

        public string GetShaderPropertyName()
        {
            if (ShaderOptions.Count > PropId)
            {
                return ShaderOptionNames[PropId];
            }

            return DefaultProperty;
        }
    }
}
