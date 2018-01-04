// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Examples.UX.Widgets;
using UnityEngine;

namespace MixedRealityToolkit.Examples.UX.Tests
{
    /// <summary>
    /// Sample Interactive Widget for changing colors and updating positions based on ButtonStateEnum
    /// </remarks>
    public class InteractiveWidgetTest : InteractiveWidget
    {
        public Color[] EffectColors;
        public Vector3[] EffectScale;
        public Vector3[] EffectPosition;

        private Renderer mRenderer;

        private void Start()
        {
            mRenderer = this.gameObject.GetComponent<Renderer>();
        }
        /// <summary>
        /// Interactive calls this method on state change
        /// </summary>
        /// <param name="state">
        /// Enum containing the following states:
        /// DefaultState: normal state of the button
        /// FocusState: gameObject has gaze
        /// PressState: currently being pressed
        /// SelectedState: selected and has no other interaction
        /// FocusSelected: selected with gaze
        /// PressSelected: selected and pressed
        /// Disabled: button is disabled
        /// DisabledSelected: the button is not interactive, but in it's alternate state (toggle button)
        /// </param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            int colorIndex = -1;
            switch (state)
            {
                case Interactive.ButtonStateEnum.Default:
                    this.gameObject.transform.localScale = EffectScale[0];
                    this.gameObject.transform.localPosition = EffectPosition[0];
                    colorIndex = 0;
                    break;
                case Interactive.ButtonStateEnum.Focus:
                    this.gameObject.transform.localScale = EffectScale[1];
                    this.gameObject.transform.localPosition = EffectPosition[0];
                    colorIndex = 0;
                    break;
                case Interactive.ButtonStateEnum.Press:
                    this.gameObject.transform.localPosition = EffectPosition[1];
                    colorIndex = 0;
                    break;
                case Interactive.ButtonStateEnum.Selected:
                    this.gameObject.transform.localScale = EffectScale[0];
                    this.gameObject.transform.localPosition = EffectPosition[0];
                    colorIndex = 1;
                    break;
                case Interactive.ButtonStateEnum.FocusSelected:
                    this.gameObject.transform.localScale = EffectScale[1];
                    this.gameObject.transform.localPosition = EffectPosition[0];
                    colorIndex = 1;
                    break;
                case Interactive.ButtonStateEnum.PressSelected:
                    this.gameObject.transform.localPosition = EffectPosition[1];
                    colorIndex = 1;
                    break;
                case Interactive.ButtonStateEnum.Disabled:
                    this.gameObject.transform.localScale = EffectScale[0];
                    this.gameObject.transform.localPosition = EffectPosition[0];
                    colorIndex = 0;
                    break;
                case Interactive.ButtonStateEnum.DisabledSelected:
                    this.gameObject.transform.localScale = EffectScale[0];
                    this.gameObject.transform.localPosition = EffectPosition[0];
                    colorIndex = 1;
                    break;
                default:
                    break;
            }

            if (mRenderer != null && colorIndex > -1)
            {
                mRenderer.material.color = EffectColors[colorIndex];
            }
        }
    }
}
