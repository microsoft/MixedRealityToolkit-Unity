// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A color theme that can set colors on renderers or common text objects
    /// This theme will try to set color on text objects first, if none can be found,
    /// then we fall back to renderer color setting using the parent class.
    /// </summary>
    public class InteractableColorTheme : InteractableShaderTheme
    {
        // caching methods to set and get colors from text object
        // this will avoid 4 if statements for every set or get - also during animation
        private delegate bool SetColorOnText(Color color, InteractableThemeProperty property, int index, float percentage);
        private delegate bool GetColorFromText(InteractableThemeProperty property, out Color color);
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
            // set the delegate to bypass these checks in the future.
            // if no text objects exists then fall back to renderer based color getting.
            if (GetColorValue != null)
            {
                GetColorValue(property, out color.Color);
                return color;
            }
            else
            {
                if (TryGetTextMeshProColor(property, out color.Color))
                {
                    GetColorValue = TryGetTextMeshProColor;
                    return color;
                }

                if (TryGetTextMeshProUGUIColor(property, out color.Color))
                {
                    GetColorValue = TryGetTextMeshProUGUIColor;
                    return color;
                }

                if (TryGetTextMeshColor(property, out color.Color))
                {
                    GetColorValue = TryGetTextMeshColor;
                    return color;
                }

                if (TryGetTextColor(property, out color.Color))
                {
                    GetColorValue = TryGetTextColor;
                    return color;
                }

                // no text components exist, fallback to renderer
                TryGetRendererColor(property, out color.Color);
                GetColorValue = TryGetRendererColor;
                return color;
            }
        }

        public override void SetValue(InteractableThemeProperty property, int index, float percentage)
        {
            Color color = Color.Lerp(property.StartValue.Color, property.Values[index].Color, percentage);

            // check if a text object exists and set the color,
            // set the delegate to bypass these checks in the future.
            // if no text objects exists then fall back to renderer based color getting.
            if (SetColorValue != null)
            {
                SetColorValue(color, property, index, percentage);
            }
            else
            {
                if (TrySetTextMeshProColor(color, property, index, percentage))
                {
                    SetColorValue = TrySetTextMeshProColor;
                    return;
                }

                if (TrySetTextMeshProUGUIColor(color, property, index, percentage))
                {
                    SetColorValue = TrySetTextMeshProUGUIColor;
                    return;
                }

                if (TrySetTextMeshColor(color, property, index, percentage))
                {
                    SetColorValue = TrySetTextMeshColor;
                    return;
                }

                if (TrySetTextColor(color, property, index, percentage))
                {
                    SetColorValue = TrySetTextColor;
                    return;
                }

                TrySetRendererColor(color, property, index, percentage);
                SetColorValue = TrySetRendererColor;
            }
        }

        /// <summary>
        /// Try to get a color from UI Text
        /// if no color is found, a text component does not exist on this object
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        protected bool TryGetTextColor(InteractableThemeProperty property, out Color color)
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
        /// Try to get color from TextMesh
        /// If no color is found, not TextMesh on this object
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        protected bool TryGetTextMeshColor(InteractableThemeProperty property, out Color color)
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
        /// Try to get color from TextMeshPro
        /// If no color is found, TextMeshPro is not on the object
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        protected bool TryGetTextMeshProColor(InteractableThemeProperty property, out Color color)
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
        /// Try to get color from TextMeshProUGUI
        /// If no color is found, TextMeshProUGUI is not on the object
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        protected bool TryGetTextMeshProUGUIColor(InteractableThemeProperty property, out Color color)
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
        /// Try to get color from the renderer
        /// return true, no text components exists, so falling back to base
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        protected bool TryGetRendererColor(InteractableThemeProperty property, out Color color)
        {
            color = base.GetProperty(property).Color;
            return true;
        }

        /// <summary>
        /// Try to set color on UI Text
        /// If false, no UI Text was found
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        protected bool TrySetTextColor(Color colour, InteractableThemeProperty property, int index, float percentage)
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
        /// Try to set color on TextMesh
        /// If false, no TextMesh was found
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        protected bool TrySetTextMeshColor(Color colour, InteractableThemeProperty property, int index, float percentage)
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
        /// Try to set color on TextMeshPro
        /// If false, no TextMeshPro was found
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        protected bool TrySetTextMeshProColor(Color colour, InteractableThemeProperty property, int index, float percentage)
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
        /// Try to set color on TextMeshProUGUI
        /// If false, no TextMeshProUGUI was found
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        protected bool TrySetTextMeshProUGUIColor(Color colour, InteractableThemeProperty property, int index, float percentage)
        {
            TextMeshProUGUI tmp = Host.GetComponent<TextMeshProUGUI>();
            if (tmp)
            {
                tmp.color = colour;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to set color on a renderer
        /// should just return true - falling back to base
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        protected bool TrySetRendererColor(Color colour, InteractableThemeProperty property, int index, float percentage)
        {
            base.SetValue(property, index, percentage);
            return true;
        }

        /// <summary>
        /// Looks to see if a text component exists on the host
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
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
