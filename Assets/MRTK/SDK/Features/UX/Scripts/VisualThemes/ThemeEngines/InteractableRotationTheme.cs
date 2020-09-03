// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
        /// <summary>
        /// If true, the theme rotation value will be added to the initial Gameobject's rotation. Otherwise, it will set directly as absolute euler angles.
        /// </summary>
        public bool IsRelativeRotation => GetThemeProperty(RelativeRotationPropertyIndex).Value.Bool;

        /// <summary>
        /// If true, the theme manipulate the target's local rotation. Otherwise, the theme will control the world space rotation.
        /// </summary>
        public bool IsLocalRotation => GetThemeProperty(LocalRotationPropertyIndex).Value.Bool;

        protected Vector3 originalLocalRotation;
        protected Vector3 originalRotation;
        protected Transform hostTransform;

        private const int RelativeRotationPropertyIndex = 0;
        private const int LocalRotationPropertyIndex = 1;

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
                        Tooltip = "Should the theme rotation value be added to the initial Gameobject's rotation, or set directly as absolute euler angles",
                        Type = ThemePropertyTypes.Bool,
                        Value = new ThemePropertyValue() { Bool = false }
                    },
                    new ThemeProperty()
                    {
                        Name = "Local Rotation",
                        Tooltip = "Should the theme manipulate the target's local rotation or world space rotation",
                        Type = ThemePropertyTypes.Bool,
                        Value = new ThemePropertyValue() { Bool = true }
                    },
                },
            };
        }

        /// <inheritdoc />
        public override void Init(GameObject host, ThemeDefinition settings)
        {
            hostTransform = host.transform;
            originalLocalRotation = hostTransform.localEulerAngles;
            originalRotation = hostTransform.eulerAngles;

            base.Init(host, settings);
        }

        /// <inheritdoc />
        public override void Reset()
        {
            if (hostTransform != null)
            {
                hostTransform.localEulerAngles = originalLocalRotation;
                hostTransform.eulerAngles = originalRotation;
            }
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();
            start.Vector3 = IsLocalRotation ? hostTransform.localEulerAngles : hostTransform.eulerAngles;
            return start;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            Vector3 lerpTarget = property.Values[index].Vector3;

            if (IsRelativeRotation)
            {
                lerpTarget = (IsLocalRotation ? originalLocalRotation : originalRotation) + lerpTarget;
            }

            SetRotation(Quaternion.Euler(Vector3.Lerp(property.StartValue.Vector3, lerpTarget, percentage)));
        }

        /// <inheritdoc />
        protected override void SetValue(ThemeStateProperty property, ThemePropertyValue value)
        {
            SetRotation(value.Quaternion);
        }

        private void SetRotation(Quaternion rot)
        {
            if (IsLocalRotation)
            {
                hostTransform.localRotation = rot;
            }
            else
            {
                hostTransform.rotation = rot;
            }
        }
    }
}
