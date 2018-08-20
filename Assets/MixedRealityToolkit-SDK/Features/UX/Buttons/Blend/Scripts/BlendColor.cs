// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Blend
{
    /// <summary>
    /// A color blending animation component, handles multiple materials
    /// </summary>
    public class BlendColor : Blend<Color>
    {
        [HideInInspector]
        public string ShaderPropertyName = ColorAbstraction.DefaultColor;

        private ColorAbstraction colorAbstraction;

        protected override void Awake()
        {
            base.Awake();
            GetColorAbstraction();
            startValue = GetColorAbstraction().GetColor();
        }

        protected void Start()
        {
            startValue = GetColorAbstraction().GetColor();
        }
        
        /// <summary>
        /// a color abstraction that colors materials and text objects
        /// </summary>
        /// <returns></returns>
        private ColorAbstraction GetColorAbstraction()
        {
            if (colorAbstraction == null)
            {
                colorAbstraction = new ColorAbstraction(TargetObject, ShaderPropertyName);
            }

            return colorAbstraction;
        }

        /// <summary>
        /// is the animation complete
        /// </summary>
        /// <param name="value1">Color</param>
        /// <param name="value2">Color</param>
        /// <returns></returns>
        public override bool CompareValues(Color value1, Color value2)
        {
            return value1 == value2;
        }

        /// <summary>
        /// get the current color
        /// </summary>
        /// <returns>Color</returns>
        public override Color GetValue()
        {
            var colorAbs = GetColorAbstraction();
            return colorAbs.GetColor();
        }

        /// <summary>
        /// animate the color
        /// </summary>
        /// <param name="startValue">Color</param>
        /// <param name="targetValue">Color</param>
        /// <param name="percent">Color</param>
        /// <returns>Color</returns>
        public override Color LerpValues(Color startValue, Color targetValue, float percent)
        {
            return Color.Lerp(startValue, targetValue, percent);
        }

        /// <summary>
        /// update the color value
        /// </summary>
        /// <param name="value">Color</param>
        public override void SetValue(Color value)
        {
            GetColorAbstraction().SetColor(value);
        }
    }
}
