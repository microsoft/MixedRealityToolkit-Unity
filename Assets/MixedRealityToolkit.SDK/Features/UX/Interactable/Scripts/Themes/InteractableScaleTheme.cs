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

        public override void Init(GameObject host, InteractableThemePropertySettings settings)
        {
            base.Init(host, settings);

            hostTransform = Host.transform;
        }

        public InteractableScaleTheme()
        {
            Types = new Type[] { typeof(Transform) };
            Name = "Scale Theme";
            ThemeProperties.Add(
                new InteractableThemeProperty()
                {
                    Name = "Scale",
                    Type = InteractableThemePropertyValueTypes.Vector3,
                    Values = new List<InteractableThemePropertyValue>(),
                    Default = new InteractableThemePropertyValue() { Vector3 = Vector3.one}
                });
        }

        public override InteractableThemePropertyValue GetProperty(InteractableThemeProperty property)
        {
            InteractableThemePropertyValue start = new InteractableThemePropertyValue();
            start.Vector3 = hostTransform.localScale;
            return start;
        }

        public override void SetValue(InteractableThemeProperty property, int index, float percentage)
        {
            hostTransform.localScale = Vector3.Lerp(property.StartValue.Vector3, property.Values[index].Vector3, percentage);
        }
    }
}
