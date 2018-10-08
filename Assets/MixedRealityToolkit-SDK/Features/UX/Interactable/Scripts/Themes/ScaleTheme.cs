// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    public class ScaleTheme : ThemeBase
    {

        public ScaleTheme()
        {
            Types = new Type[] { typeof(Transform) };
            Name = "Scale Theme";
            ThemeProperties.Add(
                new ThemeProperty()
                {
                    Name = "Scale",
                    Type = ThemePropertyValueTypes.Vector3,
                    Values = new List<ThemePropertyValue>(),
                    Default = new ThemePropertyValue() { Vector3 = Vector3.one}
                });
        }

        public override ThemePropertyValue GetProperty(ThemeProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();
            start.Vector3 = Host.transform.localScale;
            return start;
        }

        public override void SetValue(ThemeProperty property, int index, float percentage)
        {
            Host.transform.localScale = Vector3.Lerp(property.StartValue.Vector3, property.Values[index].Vector3, percentage);
        }
    }
}
