// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractableColorTheme : InteractableShaderTheme
    {
        // caching methods to set and get colors from text object
        // this will avoid 4 if statements for every set or get
        private delegate bool SetColorOnText(Color colour);
        private delegate Color GetColorFromText(out bool success);
        private SetColorOnText SetColorValue = null;
        private GetColorFromText GetColorValue = null;

        public InteractableColorTheme()
        {
            Types = new Type[] { typeof(Renderer), typeof(TextMesh), typeof(Text) };
            Name = "Color Theme";
            ThemeProperties = new List<InteractableThemeProperty>();
            ThemeProperties.Add(
                new InteractableThemeProperty()
                {
                    Name = "Color",
                    Type = InteractableThemePropertyValueTypes.Color,
                    Values = new List<InteractableThemePropertyValue>(),
                    Default = new InteractableThemePropertyValue() { Color = Color.white}
                });
        }

        public override void Init(GameObject host, InteractableThemePropertySettings settings)
        {
            base.Init(host, settings);
        }

        public override InteractableThemePropertyValue GetProperty(InteractableThemeProperty property)
        {
            InteractableThemePropertyValue color = new InteractableThemePropertyValue();

            // check if a text object exists and get the color,
            // if not then fall back to renderer based color getting.
            bool success = false;
            if (GetColorValue != null)
            {
                color.Color = GetTextColor(out success);
                return color;
            }
            else
            {
                color.Color = GetTextMeshProColor(out success);
                if (success)
                {
                    GetColorValue = GetTextMeshProColor;
                    return color;
                }

                color.Color = GetTextMeshProUGUIColor(out success);
                if (success)
                {
                    GetColorValue = GetTextMeshProUGUIColor;
                    return color;
                }

                color.Color = GetTextMeshColor(out success);
                if (success)
                {
                    GetColorValue = GetTextMeshColor;
                    return color;
                }

                color.Color = GetTextColor(out success);
                if (success)
                {
                    GetColorValue = GetTextColor;
                    return color;
                }
            }

            return base.GetProperty(property);
        }

        public override void SetValue(InteractableThemeProperty property, int index, float percentage)
        {
            Color color = Color.Lerp(property.StartValue.Color, property.Values[index].Color, percentage);

            // check if a text object exists and set the color,
            // if not then fall back to renderer based color setting.
            if (SetColorValue != null)
            {
                SetTextColor(color);
            }
            else
            {
                if (SetTextMeshProColor(color))
                {
                    SetColorValue = SetTextMeshProColor;
                    return;
                }

                if (SetTextMeshProUGUIColor(color))
                {
                    SetColorValue = SetTextMeshProUGUIColor;
                    return;
                }

                if (SetTextMeshColor(color))
                {
                    SetColorValue = SetTextMeshColor;
                    return;
                }

                if (SetTextColor(color))
                {
                    SetColorValue = SetTextColor;
                    return;
                }
            }

            base.SetValue(property, index, percentage);

        }

        /// <summary>
        /// Get color on UI Text
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        protected Color GetTextColor(out bool success)
        {
            Color colour = Color.white;
            Text text = Host.GetComponent<Text>();
            if (text != null)
            {
                success = true;
                return text.color;
            }
            success = false;
            return colour;
        }

        /// <summary>
        /// Get color from TextMesh
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        protected Color GetTextMeshColor(out bool success)
        {
            Color colour = Color.white;
            TextMesh mesh = Host.GetComponent<TextMesh>();
            if (mesh != null)
            {
                success = true;
                return mesh.color;
            }
            success = false;
            return colour;
        }

        /// <summary>
        /// Get color from TextMeshPro
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        protected Color GetTextMeshProColor(out bool success)
        {
            Color colour = Color.white;
            TextMeshPro tmp = Host.GetComponent<TextMeshPro>();
            if (tmp)
            {
                success = true;
                return tmp.color;
            }
            success = false;
            return colour;
        }

        /// <summary>
        /// Get color from TextMeshProUGUI
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        protected Color GetTextMeshProUGUIColor(out bool success)
        {
            Color colour = Color.white;
            TextMeshProUGUI tmp = Host.GetComponent<TextMeshProUGUI>();
            if (tmp)
            {
                success = true;
                return tmp.color;
            }
            success = false;
            return colour;
        }

        /// <summary>
        /// Set color on UI Text
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        protected bool SetTextColor(Color colour)
        {
            Text text = Host.GetComponent<Text>();
            if (text != null)
            {
                text.color = colour;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set color on TextMesh
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        protected bool SetTextMeshColor(Color colour)
        {
            TextMesh mesh = Host.GetComponent<TextMesh>();
            if (mesh != null)
            {
                mesh.color = colour;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set color on TextMeshPro
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        protected bool SetTextMeshProColor(Color colour)
        {
            TextMeshPro tmp = Host.GetComponent<TextMeshPro>();
            if (tmp)
            {
                tmp.color = colour;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set color on TextMeshProUGUI
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        protected bool SetTextMeshProUGUIColor(Color colour)
        {
            TextMeshProUGUI tmp = Host.GetComponent<TextMeshProUGUI>();
            if (tmp)
            {
                tmp.color = colour;
                return true;
            }

            return false;
        }
        
    }
}
