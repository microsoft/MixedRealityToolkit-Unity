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
        private delegate bool SetColorOnText(Color color);
        private delegate bool GetColorFromText(out Color color);
        private SetColorOnText SetColorValue = null;
        private GetColorFromText GetColorValue = null;

        public InteractableColorTheme()
        {
            Types = new Type[] { typeof(Renderer), typeof(TextMesh), typeof(Text), typeof(TextMeshPro), typeof(TextMeshProUGUI) };
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
            if (GetColorValue != null)
            {
                GetColorValue(out color.Color);
                return color;
            }
            else
            {
                if (GetTextMeshProColor(out color.Color))
                {
                    GetColorValue = GetTextMeshProColor;
                    return color;
                }

                if (GetTextMeshProUGUIColor(out color.Color))
                {
                    GetColorValue = GetTextMeshProUGUIColor;
                    return color;
                }

                if (GetTextMeshColor(out color.Color))
                {
                    GetColorValue = GetTextMeshColor;
                    return color;
                }

                if (GetTextColor(out color.Color))
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
                SetColorValue(color);
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
        protected bool GetTextColor(out Color color)
        {
            Color colour = Color.white;
            Text text = Host.GetComponent<Text>();
            if (text != null)
            {
                color = text.color;
                return true;
            }
            color = colour;
            return false;
        }

        /// <summary>
        /// Get color from TextMesh
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        protected bool GetTextMeshColor(out Color color)
        {
            Color colour = Color.white;
            TextMesh mesh = Host.GetComponent<TextMesh>();
            if (mesh != null)
            {
                color = mesh.color;
                return true;
            }
            color = colour;
            return false;
        }

        /// <summary>
        /// Get color from TextMeshPro
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        protected bool GetTextMeshProColor(out Color color)
        {
            Color colour = Color.white;
            TextMeshPro tmp = Host.GetComponent<TextMeshPro>();
            if (tmp)
            {
                color = tmp.color;
                return true;
            }
            color = colour;
            return false;
        }

        /// <summary>
        /// Get color from TextMeshProUGUI
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        protected bool GetTextMeshProUGUIColor(out Color color)
        {
            Color colour = Color.white;
            TextMeshProUGUI tmp = Host.GetComponent<TextMeshProUGUI>();
            if (tmp)
            {
                
                color = tmp.color;
                return true;
            }
            
            color = colour;
            return false;
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

        public static bool HasTextComponentOnObject(GameObject host)
        {
            TextMeshPro tmp = host.GetComponent<TextMeshPro>();
            if(tmp != null)
            {
                return true;
            }

            TextMeshProUGUI tmpUGUI = host.GetComponent<TextMeshProUGUI>();
            if (tmpUGUI != null)
            {
                return true;
            }

            TextMesh mesh = host.GetComponent<TextMesh>();
            if (mesh != null)
            {
                return true;
            }

            Text text = host.GetComponent<Text>();
            if (text != null)
            {
                return true;
            }

            return false;
        }
        
    }
}
