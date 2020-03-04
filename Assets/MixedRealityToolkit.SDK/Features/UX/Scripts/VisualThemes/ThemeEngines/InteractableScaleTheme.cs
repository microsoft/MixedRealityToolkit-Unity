// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Theme Engine to control initialized GameObject's scale based on state changes
    /// </summary>
    public class InteractableScaleTheme : InteractableThemeBase
    {
        protected Vector3 originalScale;
        protected Transform hostTransform;

        public InteractableScaleTheme()
        {
            Types = new Type[] { typeof(Transform) };
            Name = "Scale Theme";
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
                        Name = "Scale",
                        Type = ThemePropertyTypes.Vector3,
                        Values = new List<ThemePropertyValue>(),
                        Default = new ThemePropertyValue() { Vector3 = Vector3.one}
                    },
                },
                CustomProperties = new List<ThemeProperty>()
                {
                    new ThemeProperty()
                    {
                        Name = "Relative Scale",
                        Tooltip = "Should the scale be relative to initial Gameobject scale, or absolute",
                        Type = ThemePropertyTypes.Bool,
                        Value = new ThemePropertyValue() { Bool = false }
                    },
                },
            };
        }

        /// <inheritdoc />
        public override void Init(GameObject host, ThemeDefinition settings)
        {
            hostTransform = host.transform;
            originalScale = hostTransform.localScale;

            base.Init(host, settings);
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();
            start.Vector3 = hostTransform != null ? hostTransform.localScale : Vector3.zero;
            return start;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            Vector3 lerpTarget = property.Values[index].Vector3;

            bool relative = Properties[0].Value.Bool;
            if (relative)
            {
                lerpTarget = Vector3.Scale(originalScale, lerpTarget);
            }

            SetScale(Vector3.Lerp(property.StartValue.Vector3, lerpTarget, percentage));
        }

        /// <inheritdoc />
        protected override void SetValue(ThemeStateProperty property, ThemePropertyValue value)
        {
            SetScale(value.Vector3);
        }

        private void SetScale(Vector3 newScale)
        {
            if (hostTransform != null)
            {
                hostTransform.localScale = newScale;
            }
        }
    }
}
