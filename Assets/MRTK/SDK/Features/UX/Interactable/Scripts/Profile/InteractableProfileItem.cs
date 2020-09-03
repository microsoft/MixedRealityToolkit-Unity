// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// The foundation of profiles that exist on an Interactable
    /// Profiles pair ThemeContainers with the objects they manipulate, based on state changes
    /// </summary>
    [System.Serializable]
    public class InteractableProfileItem
    {
        /// <summary>
        /// GameObject to target with associated Themes
        /// </summary>
        public GameObject Target;

        /// <summary>
        /// List of Theme configuration data to initialize with an Interactable
        /// </summary>
        public List<Theme> Themes = new List<Theme>();
    }
}
