// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

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
        public int PropId;
        public List<ShaderProperties> ShaderOptions;
        public List<string> ShaderOptionNames;
        public InteractableThemePropertyValue Default;
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
