// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Theme Engine to control a GameObject's rotation based on state changes
    /// </summary>
    public class InteractableRotationTheme : InteractableThemeBase
    {
        protected Vector3 originalRotation;
        protected Transform hostTransform;

        public InteractableRotationTheme()
        {
            Types = new Type[] { typeof(Transform) };
            Name = "Rotation Theme";
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
                        Name = "Rotation",
                        Type = ThemePropertyTypes.Vector3,
                        Values = new List<ThemePropertyValue>(),
                        Default = new ThemePropertyValue() { Vector3 = Vector3.zero }
                    },
                },
                CustomProperties = new List<ThemeProperty>()
                {
                    new ThemeProperty()
                    {
                        Name = "Relative Rotation",
                        Tooltip = "Should the rotation be added to initial Gameobject rotation, or absolute",
                        Type = ThemePropertyTypes.Bool,
                        Value = new ThemePropertyValue() { Bool = false }
                    },
                },
            };
        }

        /// <inheritdoc />
        public override void Init(GameObject host, ThemeDefinition settings)
        {
            base.Init(host, settings);

            hostTransform = Host.transform;
            originalRotation = hostTransform.localEulerAngles;
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();
            start.Vector3 = hostTransform.eulerAngles;
            return start;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            Vector3 lerpTarget = property.Values[index].Vector3;

            bool relative = Properties[0].Value.Bool;
            if (relative)
            {
                lerpTarget = originalRotation + lerpTarget;
            }

            hostTransform.localRotation = Quaternion.Euler(Vector3.Lerp(property.StartValue.Vector3, lerpTarget, percentage));
        }
    }
}
