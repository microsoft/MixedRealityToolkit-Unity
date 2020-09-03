// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A simple property with name, tooltip, value and type, used for serialization
    /// The custom settings are used in themes to expose properties needed to enhance theme functionality
    /// </summary>

    [System.Serializable]
    public class ThemeProperty
    {
        /// <summary>
        /// Displayed as Label in Inspector
        /// </summary>
        public string Name;

        /// <summary>
        /// Tooltip associated with Name
        /// </summary>
        public string Tooltip;

        /// <summary>
        /// inner type for that property
        /// </summary>
        public ThemePropertyTypes Type;

        /// <summary>
        /// inner value, filled for corresponding Type
        /// </summary>
        public ThemePropertyValue Value;
    }
}
