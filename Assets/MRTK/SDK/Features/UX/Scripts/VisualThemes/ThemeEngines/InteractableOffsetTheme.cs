// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractableOffsetTheme : InteractableThemeBase
    {
        private Vector3 originalPosition;
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
            hostTransform = host.transform;
            originalPosition = hostTransform.localPosition;

            base.Init(host, settings);
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();
            start.Vector3 = hostTransform != null ? hostTransform.localPosition : Vector3.zero;
            return start;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            SetOffset(Vector3.Lerp(property.StartValue.Vector3, originalPosition + property.Values[index].Vector3, percentage));
        }

        /// <inheritdoc />
        protected override void SetValue(ThemeStateProperty property, ThemePropertyValue value)
        {
            SetOffset(value.Vector3);
        }

        private void SetOffset(Vector3 offset)
        {
            if (hostTransform != null)
            {
                hostTransform.localPosition = offset;
            }
        }
    }
}
