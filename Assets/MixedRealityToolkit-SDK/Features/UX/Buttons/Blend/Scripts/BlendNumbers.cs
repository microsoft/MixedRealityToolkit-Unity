// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blend
{
    /// <summary>
    /// Handles blending numeric values
    /// </summary>
    public class BlendNumbers : Blend<string>
    {
        public string Format = "0.##";

        private TextMesh textMesh;
        private Text text;

        protected override void Awake()
        {
            textMesh = GetComponent<TextMesh>();
            text = GetComponent<Text>();

            base.Awake();
        }

        public override bool CompareValues(string value1, string value2)
        {
            return value1 == value2;
        }

        public override string GetValue()
        {
            if(textMesh != null)
            {
                return textMesh.text;
            }
            else if(text != null)
            {
                return text.text;
            }

            return "";
        }

        public override string LerpValues(string startValue, string targetValue, float percent)
        {
            float start = float.Parse(startValue);
            float target = float.Parse(targetValue);

            float newValue = start + (target - start) * percent;

            string output = "";

            if (Format.IndexOf('.') > -1)
            {
                output = newValue.ToString(Format);
            }
            else
            {
                output = Mathf.Round(newValue).ToString(Format);
            }

            return output;

        }

        public override void SetValue(string value)
        {
            if (textMesh != null)
            {
                textMesh.text = value;
            }
            else if (text != null)
            {
                text.text = value;
            }
        }
    }
}
