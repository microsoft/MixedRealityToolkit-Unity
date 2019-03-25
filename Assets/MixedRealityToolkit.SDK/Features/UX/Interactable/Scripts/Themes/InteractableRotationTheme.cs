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
            ThemeProperties.Add(
                new InteractableThemeProperty()
                {
                    Name = "Rotation",
                    Type = InteractableThemePropertyValueTypes.Vector3,
                    Values = new List<InteractableThemePropertyValue>(),
                    Default = new InteractableThemePropertyValue() { Vector3 = Vector3.zero }
                });
        }

        public override void Init(GameObject host, InteractableThemePropertySettings settings)
        {
            base.Init(host, settings);

            hostTransform = Host.transform;
        }

        public override InteractableThemePropertyValue GetProperty(InteractableThemeProperty property)
        {
            InteractableThemePropertyValue start = new InteractableThemePropertyValue();
            start.Vector3 = hostTransform.eulerAngles;
            return start;
        }

        public override void SetValue(InteractableThemeProperty property, int index, float percentage)
        {
            hostTransform.localRotation = Quaternion.Euler( Vector3.Lerp(property.StartValue.Vector3, property.Values[index].Vector3, percentage));
        }
    }
}
