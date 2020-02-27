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
        private int lastIndex = -1;
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
            controller = host.GetComponent<Animator>();
            base.Init(host, settings);
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();
            start.String = lastIndex != -1 ? property.Values[lastIndex].String : string.Empty;
            return start;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            if (lastIndex != index)
            {
                SetValue(property, property.Values[index]);
                lastIndex = index;
            }
        }

        /// <inheritdoc />
        protected override void SetValue(ThemeStateProperty property, ThemePropertyValue value)
        {
            if (controller != null && !string.IsNullOrEmpty(value.String))
            {
                controller.SetTrigger(value.String);
            }
        }
    }
}
