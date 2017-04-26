// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Examples.Prototyping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    public class MaterialColorWidget : InteractiveWidget
    {
        public ColorButtonTheme ColorTheme;
        public ColorTransition ColorBlender;

        protected Material mMaterial;

        void Awake()
        {
            if (ColorBlender == null)
            {
                ColorBlender = GetComponent<ColorTransition>();
            }

            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                mMaterial = renderer.material;
            }

            if (mMaterial != null && ColorTheme != null)
            {
                mMaterial.color = ColorTheme.GetThemeValue(Interactive.ButtonStateEnum.Default);
            }
        }

        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            if (ColorTheme != null)
            {
                if (ColorBlender != null)
                {
                    ColorBlender.StartTransition(ColorTheme.GetThemeValue(state));
                }
                else if (mMaterial != null)
                {
                    mMaterial.color = ColorTheme.GetThemeValue(state);
                }
            }
        }

        private void OnDestroy()
        {
            if (mMaterial != null)
            {
                GameObject.Destroy(mMaterial);
            }
        }
    }
}
