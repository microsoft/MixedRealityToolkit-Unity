// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractableActivateTheme : InteractableThemeBase
    {

        public InteractableActivateTheme()
        {
            Types = new Type[] { typeof(Transform) };
            Name = "Activate Theme";
            NoEasing = true;
            ThemeProperties.Add(
                new InteractableThemeProperty()
                {
                    Name = "Activate",
                    Type = InteractableThemePropertyValueTypes.Bool,
                    Values = new List<InteractableThemePropertyValue>(),
                    Default = new InteractableThemePropertyValue() { Bool = true }
                });
        }
        

        public override InteractableThemePropertyValue GetProperty(InteractableThemeProperty property)
        {
            InteractableThemePropertyValue start = new InteractableThemePropertyValue();
            start.Bool = Host.activeSelf;
            return start;
        }

        public override void SetValue(InteractableThemeProperty property, int index, float percentage)
        {
            Host.SetActive(property.Values[index].Bool);
        }
    }
}
