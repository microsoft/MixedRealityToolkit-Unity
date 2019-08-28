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
            StateProperties = GetDefaultStateProperties();
        }

        /// <inheritdoc />
        public override List<ThemeStateProperty> GetDefaultStateProperties()
        {
            return new List<ThemeStateProperty>()
            {
                new ThemeStateProperty()
                {
                    Name = "Activate",
                    Type = ThemePropertyTypes.Bool,
                    Values = new List<ThemePropertyValue>(),
                    Default = new ThemePropertyValue() { Bool = true }
                },
            };
        }

        /// <inheritdoc />
        public override List<ThemeProperty> GetDefaultThemeProperties()
        {
            return new List<ThemeProperty>();
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();
            start.Bool = Host.activeSelf;
            return start;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            Host.SetActive(property.Values[index].Bool);
        }
    }
}
