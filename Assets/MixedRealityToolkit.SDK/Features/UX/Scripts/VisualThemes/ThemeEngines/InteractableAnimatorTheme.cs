// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// ThemeEngine that controls Animator state based on state changes
    /// Targets first Animator component returned on initialized GameObject
    /// </summary>
    public class InteractableAnimatorTheme : InteractableThemeBase
    {
        private int lastIndex = 0;
        private Animator controller;

        public InteractableAnimatorTheme()
        {
            Types = new Type[] { typeof(Transform) };
            Name = "AnimatorTheme";
        }

        /// <inheritdoc />
        public override ThemeDefinition GetDefaultThemeDefinition()
        {
            return new ThemeDefinition()
            {
                ThemeType = GetType(),
                StateProperties = new List<ThemeStateProperty>()
                {
                    new ThemeStateProperty()
                    {
                        Name = "Animator Trigger",
                        Type = ThemePropertyTypes.AnimatorTrigger,
                        Values = new List<ThemePropertyValue>(),
                        Default = new ThemePropertyValue() { String = "Default" }
                    },
                },
                CustomProperties = new List<ThemeProperty>(),
            };
        }

        /// <inheritdoc />
        public override void Init(GameObject host, ThemeDefinition settings)
        {
            base.Init(host, settings);
            controller = Host.GetComponent<Animator>();
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();
            start.String = property.Values[lastIndex].String;
            return start;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            if(lastIndex != index)
            {
                if(controller != null)
                {
                    controller.SetTrigger(property.Values[index].String);
                }
                lastIndex = index;
            }
        }
    }
}
