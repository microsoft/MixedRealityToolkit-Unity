// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Theme Engine to change the string value on a Text type object based on state changes
    /// Finds the first available component searching in order of TextMesh, Text, TextMeshPro, TextMeshProUGUI
    /// </summary>
    public class InteractableStringTheme : InteractableThemeBase
    {
        /// <inheritdoc />
        public override bool IsEasingSupported => false;

        private TextMesh mesh;
        private Text text;
        private TMPro.TextMeshPro meshPro;
        private TMPro.TextMeshProUGUI meshProUGUI;

        public InteractableStringTheme()
        {
            Types = new Type[] { typeof(TextMesh), typeof(Text), typeof(TextMeshPro), typeof(TextMeshProUGUI) };
            Name = "String Theme";
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
            mesh = host.GetComponent<TextMesh>();
            text = host.GetComponent<Text>();
            meshPro = host.GetComponent<TextMeshPro>();
            meshProUGUI = host.GetComponent<TextMeshProUGUI>();

            base.Init(host, settings);
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();
            start.String = string.Empty;

            if (mesh != null)
            {
                start.String = mesh.text;
            }
            else if (text != null)
            {
                start.String = text.text;
            }
            else if (meshPro != null)
            {
                start.String = meshPro.text;
            }
            else if (meshProUGUI != null)
            {
                start.String = meshProUGUI.text;
            }

            return start;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            SetValue(property, property.Values[index]);
        }

        /// <inheritdoc />
        protected override void SetValue(ThemeStateProperty property, ThemePropertyValue value)
        {
            if (mesh != null)
            {
                mesh.text = value.String;
            }
            else if (text != null)
            {
                text.text = value.String;
            }
            else if (meshPro != null)
            {
                meshPro.text = value.String;
            }
            else if (meshProUGUI != null)
            {
                meshProUGUI.text = value.String;
            }
        }
    }
}
