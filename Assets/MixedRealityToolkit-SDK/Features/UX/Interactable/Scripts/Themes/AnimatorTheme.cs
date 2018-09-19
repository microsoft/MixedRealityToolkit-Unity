// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    public class AnimatorTheme : ThemeBase
    {
        // if no animator exist, show create button
        // if noEasing, hide blends
        // how to inject special inspector instructions?

        private int lastIndex = 0;

        public AnimatorTheme()
        {
            Types = new Type[] { typeof(Transform) };
            Name = "Animator Theme";
            ThemeProperties.Add(
                new ThemeProperty()
                {
                    Name = "Animator Trigger",
                    Type = ThemePropertyValueTypes.String,
                    Values = new List<ThemePropertyValue>(),
                    Default = new ThemePropertyValue() { String = "Default" },
                    NoEasing = true
                });
        }

        public override ThemePropertyValue GetProperty(ThemeProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();
            start.String = property.Values[lastIndex].String;
            return start;
        }

        public override void SetValue(ThemeProperty property, int index, float percentage)
        {
            if(lastIndex != index)
            {
                Animator controller = Host.GetComponent<Animator>();
                if(controller != null)
                {
                    controller.SetTrigger(property.Values[index].String);
                }
                lastIndex = index;
            }
        }
    }
}
