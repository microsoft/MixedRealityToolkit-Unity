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
        private TMPro.TextMeshPro meshPro;
        private TextMesh mesh;
        private Text text;

        public InteractableStringTheme()
        {
            Types = new Type[] { typeof(TMPro.TextMeshPro), typeof(TextMesh), typeof(Text) };
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

            meshPro = Host.GetComponentInChildren<TMPro.TextMeshPro>();
            mesh = Host.GetComponentInChildren<TextMesh>();
            text = Host.GetComponentInChildren<Text>();
        }

        public override InteractableThemePropertyValue GetProperty(InteractableThemeProperty property)
        {
            InteractableThemePropertyValue start = new InteractableThemePropertyValue();
            start.String = "";

            if (meshPro != null)
            {
                start.String = meshPro.text;
                return start;
            }

            if (mesh != null)
            {
                start.String = mesh.text;
                return start;
            }

            if (text != null)
            {
                start.String = text.text;
            }
            return start;
        }

        public override void SetValue(InteractableThemeProperty property, int index, float percentage)
        {
            if (meshPro != null)
            {
                meshPro.text = property.Values[index].String;
                return;
            }
            if (mesh != null)
            {
                mesh.text = property.Values[index].String;
                return;
            }
            if (text != null)
            {
                text.text = property.Values[index].String;
            }
        }
    }
}
