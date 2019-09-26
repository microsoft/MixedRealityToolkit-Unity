// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractableOffsetTheme : InteractableThemeBase
    {
        private Vector3 startPosition;
        private Transform hostTransform;

        public InteractableOffsetTheme()
        {
            Types = new Type[] { typeof(Transform) };
            Name = "Offset Theme";
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
                        Name = "Offset",
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
            startPosition = hostTransform.localPosition;
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();
            start.Vector3 = hostTransform.localPosition;
            return start;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            hostTransform.localPosition = Vector3.Lerp(property.StartValue.Vector3, startPosition + property.Values[index].Vector3, percentage);
        }
    }
}
