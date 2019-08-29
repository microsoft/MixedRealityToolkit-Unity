// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractableRotationTheme : InteractableThemeBase
    {
        private Transform hostTransform;

        public InteractableRotationTheme()
        {
            Types = new Type[] { typeof(Transform) };
            Name = "Rotation Theme";
        }

        /// <inheritdoc />
        public override ThemeDefinition GetDefaultThemeDefinition()
        {
            Type t = GetType();
            return new ThemeDefinition()
            {
                ClassName = t.Name,
                AssemblyQualifiedName = t.AssemblyQualifiedName,
                Type = t,
                NoEasing = this.NoEasing,
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
                CustomProperties = new List<ThemeProperty>(),
            };
        }

        /// <inheritdoc />
        public override void Init(GameObject host, ThemeDefinition settings)
        {
            base.Init(host, settings);

            hostTransform = Host.transform;
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
            hostTransform.localRotation = Quaternion.Euler( Vector3.Lerp(property.StartValue.Vector3, property.Values[index].Vector3, percentage));
        }
    }
}
