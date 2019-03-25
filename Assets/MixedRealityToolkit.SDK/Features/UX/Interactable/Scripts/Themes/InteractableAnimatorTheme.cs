// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Uses Animator to control Interactable feedback based on state changes.
    /// </summary>
    public class InteractableAnimatorTheme : InteractableThemeBase
    {
        private int lastIndex = 0;
        private Animator controller;

        public InteractableAnimatorTheme()
        {
            Types = new Type[] { typeof(Transform) };
            Name = "AnimatorTheme";
            ThemeProperties.Add(
                new InteractableThemeProperty()
                {
                    Name = "Animator Trigger",
                    Type = InteractableThemePropertyValueTypes.AnimatorTrigger,
                    Values = new List<InteractableThemePropertyValue>(),
                    Default = new InteractableThemePropertyValue() { String = "Default" }
                });
        }

        public override void Init(GameObject host, InteractableThemePropertySettings settings)
        {
            base.Init(host, settings);
            controller = Host.GetComponent<Animator>();
        }

        public override InteractableThemePropertyValue GetProperty(InteractableThemeProperty property)
        {
            InteractableThemePropertyValue start = new InteractableThemePropertyValue();
            start.String = property.Values[lastIndex].String;
            return start;
        }

        public override void SetValue(InteractableThemeProperty property, int index, float percentage)
        {
            if(lastIndex != index)
            {
                if(controller != null)
                {
                    controller.SetTrigger(property.Values[index].String);
                }
                lastIndex = index;
            }
        }
    }
}
