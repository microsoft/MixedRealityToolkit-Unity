// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Profile container for theme and for loading theme settings
    /// </summary>
    [Serializable]
    public class VisualProfile
    {
        /// <summary>
        /// GameObject to Target
        /// </summary>
        public GameObject Target;

        /// <summary>
        /// Theme definition to build
        /// </summary>
        public Theme Theme;

        /// <summary>
        /// Create and initialize Theme Engines with the associated Target and Theme property
        /// </summary>
        /// <returns>List of Theme Engine instances</returns>
        public List<InteractableThemeBase> CreateThemeEngines()
        {
            List<InteractableThemeBase> results = new List<InteractableThemeBase>();

            if (Theme != null)
            {
                foreach (var definition in Theme.Definitions)
                {
                    results.Add(InteractableThemeBase.CreateAndInitTheme(definition, Target));
                }
            }

            return results;
        }
    }
}
