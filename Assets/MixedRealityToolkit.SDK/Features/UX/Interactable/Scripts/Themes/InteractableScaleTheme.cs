// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractableScaleTheme : InteractableThemeBase
    {
        private Transform hostTransform;

        public InteractableScaleTheme()
        {
            Types = new Type[] { typeof(Transform) };
            Name = "Scale Theme";
            StateProperties = GetDefaultStateProperties();
        }

        /// <inheritdoc />
        public override List<ThemeStateProperty> GetDefaultStateProperties()
        {
            return new List<ThemeStateProperty>()
            {
                new ThemeStateProperty()
                {
                    Name = "Scale",
                    Type = ThemePropertyTypes.Vector3,
                    Values = new List<ThemePropertyValue>(),
                    Default = new ThemePropertyValue() { Vector3 = Vector3.one}
                },
            };
        }

        /// <inheritdoc />
        public override List<ThemeProperty> GetDefaultThemeProperties()
        {
            return new List<ThemeProperty>();
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
            start.Vector3 = hostTransform.localScale;
            return start;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            hostTransform.localScale = Vector3.Lerp(property.StartValue.Vector3, property.Values[index].Vector3, percentage);
        }
    }
}
