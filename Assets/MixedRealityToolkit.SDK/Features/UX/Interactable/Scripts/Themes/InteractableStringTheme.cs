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
                        Name = "String",
                        Type = ThemePropertyTypes.String,
                        Values = new List<ThemePropertyValue>(),
                        Default = new ThemePropertyValue() { String = "" }
                    },
                },
                CustomProperties = new List<ThemeProperty>(),
            };
        }

        /// <inheritdoc />
        public override void Init(GameObject host, ThemeDefinition settings)
        {
            base.Init(host, settings);

            mesh = Host.GetComponent<TextMesh>();
            text = Host.GetComponent<Text>();
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();
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

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
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
