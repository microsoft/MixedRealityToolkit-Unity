// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Blend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Widgets
{
    /// <summary>
    /// a widget to control the fading of an interactive element
    /// </summary>

    [RequireComponent(typeof(BlendFade))]
    public class FadeInOutWidget : InteractiveWidget
    {
        [Tooltip("Alpha values for each state, Requires transparent material.")]
        public float DefaultAlpha = 1;
        public float FocusAlpha = 1;
        public float PressAlpha = 1;
        public float SelectedAlpha = 1;
        public float FocusSelectedAlpha = 1;
        public float PressSelectedAlpha = 1;
        public float DisabledAlpha = 1;
        public float DisabledSelectedAlpha = 1;

        private BlendFade mTransition;

        private void Awake()
        {
            mTransition = GetComponent<BlendFade>();
        }

        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            float to = 1;

            switch (state)
            {
                case Interactive.ButtonStateEnum.Default:
                    to = DefaultAlpha;
                    break;
                case Interactive.ButtonStateEnum.Focus:
                    to = FocusAlpha;
                    break;
                case Interactive.ButtonStateEnum.Press:
                    to = PressAlpha;
                    break;
                case Interactive.ButtonStateEnum.Selected:
                    to = SelectedAlpha;
                    break;
                case Interactive.ButtonStateEnum.FocusSelected:
                    to = FocusSelectedAlpha;
                    break;
                case Interactive.ButtonStateEnum.PressSelected:
                    to = PressSelectedAlpha;
                    break;
                case Interactive.ButtonStateEnum.Disabled:
                    to = DisabledAlpha;
                    break;
                case Interactive.ButtonStateEnum.DisabledSelected:
                    to = DisabledSelectedAlpha;
                    break;
                default:
                    break;
            }

            if (mTransition != null)
            {
                mTransition.TargetValue = to;
                mTransition.Play();
            }
        }
    }
}
