// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Theme Engine that can set colors on a Renderer or text object based on state changes
    /// This Theme will try to set color on first available text object in order of TextMesh, Text, TextMeshPro, and TextMeshProUGUI
    /// If no text-based component can be found, then will fall back to first Renderer component found on the initialized GameObject
    /// and target the color shader property provided in the ThemeDefinition.
    /// </summary>
    public class InteractableColorTheme : InteractableShaderTheme
    {
        // caching methods to set and get colors from text object
        // this will avoid 4 if statements for every set or get - also during animation
        private delegate bool SetColorOnText(Color color, ThemeStateProperty property, int index, float percentage);
        private delegate bool GetColorFromText(ThemeStateProperty property, out Color color);

        private SetColorOnText SetColorValue = null;
        private GetColorFromText GetColorValue = null;

        protected new const string DefaultShaderProperty = "_Color";

        public InteractableColorTheme()
        {
            Types = new Type[] { typeof(Renderer), typeof(TextMesh), typeof(Text), typeof(TextMeshPro), typeof(TextMeshProUGUI) };
            Name = "Color Theme";
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
                        Name = "Color",
                        Type = ThemePropertyTypes.Color,
                        Values = new List<ThemePropertyValue>(),
                        Default = new ThemePropertyValue() { Color = Color.white},
                        TargetShader = StandardShaderUtility.MrtkStandardShader,
                        ShaderPropertyName = DefaultShaderProperty
                    }
                },
                CustomProperties = new List<ThemeProperty>(),
            };
        }

        /// <inheritdoc />
        public override void Init(GameObject host, ThemeDefinition settings)
        {
            base.Init(host, settings);
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue color = new ThemePropertyValue();

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

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
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
        /// <param name="color">Color to try to get, returns white if no Text component found</param>
        /// <returns>true if succesfully get color on Text</returns>
        protected bool TryGetTextColor(ThemeStateProperty property, out Color color)
        {
            color = Color.white;
            Text text = Host.GetComponent<Text>();
            if (text != null)
            {
                color = text.color;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to get color from TextMesh
        /// If no color is found, not TextMesh on this object
        /// </summary>
        /// <param name="color">Color to try to get, returns white if no TextMesh component found</param>
        /// <returns>true if succesfully get color on TextMesh</returns>
        protected bool TryGetTextMeshColor(ThemeStateProperty property, out Color color)
        {
            color = Color.white;
            TextMesh mesh = Host.GetComponent<TextMesh>();
            if (mesh != null)
            {
                color = mesh.color;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to get color from TextMeshPro
        /// If no color is found, TextMeshPro is not on the object
        /// </summary>
        /// <param name="color">Color to try to get, returns white if no TextMesh component found</param>
        /// <returns>true if succesfully get color on TextMeshPro</returns>
        protected bool TryGetTextMeshProColor(ThemeStateProperty property, out Color color)
        {
            color = Color.white;
            TextMeshPro tmp = Host.GetComponent<TextMeshPro>();
            if (tmp)
            {
                color = tmp.color;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Try to get color from TextMeshProUGUI
        /// If no color is found, TextMeshProUGUI is not on the object
        /// </summary>
        /// <param name="color">Color to try to get, returns white if no TextMeshProUGUI component found</param>
        /// <returns>true if succesfully get color on TextMeshProUGUI</returns>
        protected bool TryGetTextMeshProUGUIColor(ThemeStateProperty property, out Color color)
        {
            color = Color.white;
            TextMeshProUGUI tmp = Host.GetComponent<TextMeshProUGUI>();
            if (tmp)
            {
                
                color = tmp.color;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to get color from the renderer
        /// return true, no text components exists, so falling back to base
        /// </summary>
        /// <param name="color">Color to try to set</param>
        /// <returns>true if succesfully set color on Renderer</returns>
        protected bool TryGetRendererColor(ThemeStateProperty property, out Color color)
        {
            color = base.GetProperty(property).Color;
            return true;
        }

        /// <summary>
        /// Try to set color on UI Text
        /// If false, no UI Text was found
        /// </summary>
        /// <param name="color">Color to try to set</param>
        /// <returns>true if succesfully set color on Text</returns>
        protected bool TrySetTextColor(Color color, ThemeStateProperty property, int index, float percentage)
        {
            Text text = Host.GetComponent<Text>();
            if (text != null)
            {
                text.color = color;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to set color on TextMesh
        /// If false, no TextMesh was found
        /// </summary>
        /// <param name="color">Color to try to set</param>
        /// <returns>true if succesfully set color on TextMesh</returns>
        protected bool TrySetTextMeshColor(Color color, ThemeStateProperty property, int index, float percentage)
        {
            TextMesh mesh = Host.GetComponent<TextMesh>();
            if (mesh != null)
            {
                mesh.color = color;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to set color on TextMeshPro
        /// If false, no TextMeshPro was found
        /// </summary>
        /// <param name="color">Color to try to set</param>
        /// <returns>true if succesfully set color on TextMeshPro</returns>
        protected bool TrySetTextMeshProColor(Color colour, ThemeStateProperty property, int index, float percentage)
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
        /// <param name="color">Color to try to set</param>
        /// <returns>true if succesfully set color on TextMeshProUGUI</returns>
        protected bool TrySetTextMeshProUGUIColor(Color color, ThemeStateProperty property, int index, float percentage)
        {
            TextMeshProUGUI tmp = Host.GetComponent<TextMeshProUGUI>();
            if (tmp)
            {
                tmp.color = color;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to set color on a renderer
        /// should just return true - falling back to base
        /// </summary>
        /// <param name="color">Color to try to set</param>
        /// <returns>true if succesfully set color on Renderer</returns>
        protected bool TrySetRendererColor(Color colour, ThemeStateProperty property, int index, float percentage)
        {
            base.SetValue(property, index, percentage);
            return true;
        }

        /// <summary>
        /// Looks to see if a text component exists on the host
        /// </summary>
        /// <param name="host">GameObject to test</param>
        /// <returns>true if host is not null and contains a text-type component, false otherwise</returns>
        public static bool HasTextComponentOnObject(GameObject host)
        {
            return host != null &&
                (host.GetComponent<TextMeshPro>() != null ||
                host.GetComponent<TextMeshProUGUI>() != null ||
                host.GetComponent<TextMesh>() != null ||
                host.GetComponent<Text>() != null);
        }
        
    }
}
