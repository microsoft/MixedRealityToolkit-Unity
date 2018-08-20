// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Blend
{
    /// <summary>
    /// A shader animation component, handles multiple materials
    /// </summary>
    public class BlendShaderFloat : Blend<float>
    {
        [HideInInspector]
        public string ShaderFloatName;

        [HideInInspector]
        public Vector2 Range = new Vector2(0, 1);

        private string ShaderPropertyName = ColorAbstraction.DefaultColor;
        
        private ColorAbstraction colorAbstraction;

        protected override void Awake()
        {
            base.Awake();
            GetColorAbstraction();
            startValue = GetColorAbstraction().GetShaderFloat(ShaderFloatName);
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
        public override bool CompareValues(float value1, float value2)
        {
            return value1 == value2;
        }

        /// <summary>
        /// get the current color
        /// </summary>
        /// <returns>Color</returns>
        public override float GetValue()
        {
            var colorAbs = GetColorAbstraction();
            return colorAbs.GetShaderFloat(ShaderFloatName);
        }

        /// <summary>
        /// animate the color
        /// </summary>
        /// <param name="startValue">Color</param>
        /// <param name="targetValue">Color</param>
        /// <param name="percent">Color</param>
        /// <returns>Color</returns>
        public override float LerpValues(float startValue, float targetValue, float percent)
        {
            return (targetValue - startValue) * percent + startValue;
        }

        /// <summary>
        /// update the color value
        /// </summary>
        /// <param name="value">Color</param>
        public override void SetValue(float value)
        {
            GetColorAbstraction().SetShaderFloat(ShaderFloatName, Mathf.Clamp(value, Range.x, Range.y));
        }
    }
}
