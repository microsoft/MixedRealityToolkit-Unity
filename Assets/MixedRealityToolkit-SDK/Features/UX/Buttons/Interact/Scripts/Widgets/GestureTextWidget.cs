// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Interact.Widgets
{
    public class GestureTextWidget : GestureWidget
    {
        public Vector2 OutputRange = new Vector2(0, 1);
        public string TextFormat = "0.##";

        private TextMesh textMesh;
        private Text text;

        protected override void Awake()
        {
            base.Awake();
            text = GetComponent<Text>();
            textMesh = GetComponent<TextMesh>();
        }

        protected override void UpdateValues(float percent)
        {
            float displayValue = OutputRange.x + (OutputRange.y - OutputRange.x) * percent;

            string outtput = "";
            if (TextFormat.IndexOf('.') > -1)
            {
                outtput = displayValue.ToString(TextFormat);
            }
            else
            {
                outtput = Mathf.Round(displayValue).ToString(TextFormat);
            }

            if (textMesh != null)
            {
                textMesh.text = outtput;
            }
            else if (text != null)
            {
                text.text = outtput;
            }
        }
    }
}
