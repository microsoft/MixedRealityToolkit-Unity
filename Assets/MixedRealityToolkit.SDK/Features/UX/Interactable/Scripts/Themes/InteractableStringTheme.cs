// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Change string on a text object based on state
    /// </summary>
    public class InteractableStringTheme : InteractableThemeBase
    {
        private TextMesh mesh;
        private Text text;

        public InteractableStringTheme()
        {
            Types = new Type[] { typeof(TextMesh), typeof(Text) };
            Name = "String Theme";
            NoEasing = true;
            ThemeProperties.Add(
                new InteractableThemeProperty()
                {
                    Name = "String",
                    Type = InteractableThemePropertyValueTypes.String,
                    Values = new List<InteractableThemePropertyValue>(),
                    Default = new InteractableThemePropertyValue() { String = "" }
                    
                });
        }

        public override void Init(GameObject host, InteractableThemePropertySettings settings)
        {
            base.Init(host, settings);

            mesh = Host.GetComponent<TextMesh>();
            text = Host.GetComponent<Text>();
        }

        public override InteractableThemePropertyValue GetProperty(InteractableThemeProperty property)
        {
            InteractableThemePropertyValue start = new InteractableThemePropertyValue();
            start.String = "";

            if (mesh != null)
            {
                start.String = mesh.text;
                return start;
            }

            if (mesh != null)
            {
                start.String = text.text;
            }
            return start;
        }

        public override void SetValue(InteractableThemeProperty property, int index, float percentage)
        {
            if(mesh != null)
            {
                mesh.text = property.Values[index].String;
                return;
            }
            if (mesh != null)
            {
                text.text = property.Values[index].String;
            }
        }
    }
}
