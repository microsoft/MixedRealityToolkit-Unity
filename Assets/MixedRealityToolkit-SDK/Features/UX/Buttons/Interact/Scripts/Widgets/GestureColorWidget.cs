// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Blend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Widgets
{

    public class GestureColorWidget : GestureWidget
    {

        public Color StartColor = Color.white;
        public Color EndColor = Color.blue;
        public string ColorProperty;

        private ColorAbstraction color;
        

        protected override void Start()
        {
            base.Start();

            color = new ColorAbstraction(this.gameObject, ColorProperty);
        }

        protected override void UpdateValues(float percent)
        {
            if (color != null)
            {
                color.SetColor(Color.Lerp(StartColor, EndColor, percent));
            }
        }
    }
}
