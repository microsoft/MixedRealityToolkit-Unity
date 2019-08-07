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
        public GameObject Target;
        public Theme Theme;

        public List<InteractableThemeBase> CreateThemeEngines()
        {
            List<InteractableThemeBase> results = new List<InteractableThemeBase>();

            if (Theme != null)
            {
                foreach (var setting in Theme.Settings)
                {
                    Type themeType = Type.GetType(setting.AssemblyQualifiedName);
                    InteractableThemeBase theme = (InteractableThemeBase)Activator.CreateInstance(themeType);
                    theme.Init(Target, setting);

                    results.Add(theme);
                }
            }

            return results;
        }
    }
}
