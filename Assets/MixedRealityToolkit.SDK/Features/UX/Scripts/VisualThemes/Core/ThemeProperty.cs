// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A simple property with name value and type, used for serialization
    /// The custom settings are used in themes to expose properties needed to enhance theme functionality
    /// </summary>

    [System.Serializable]
    public class ThemeProperty
    {
        public string Name;
        public ThemePropertyTypes Type;
        public ThemePropertyValue Value;
    }
}
