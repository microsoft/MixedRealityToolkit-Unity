// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Examples.UX.Widgets;
using UnityEngine;

namespace MixedRealityToolkit.Examples.UX.Controls
{
    /// <summary>
    /// Set an elements activity (true/false) based on Interactive focus state
    /// </summary>
    public class ButtonFocusShowHideWidget : InteractiveWidget
    {
        [Tooltip("Array of gameObjects to turn on and off during focus")]
        public GameObject[] Targets;

        private bool SelfHosted = false;
        private void Awake()
        {
            if (Targets.Length == 0)
            {
                Targets = new GameObject[1] { this.gameObject };
                SelfHosted = true;
            }
        }

        protected override void OnDisable()
        {
            // ignore if we are the only object in the list
            if (!SelfHosted)
            {
                base.OnDisable();
            }
        }

        /// <summary>
        /// Set active state
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            bool show = (state == Interactive.ButtonStateEnum.FocusSelected || state == Interactive.ButtonStateEnum.Focus || state == Interactive.ButtonStateEnum.Press || state == Interactive.ButtonStateEnum.PressSelected);

            for (int i = 0; i < Targets.Length; ++i)
            {
                Targets[i].SetActive(show);
            }
        }
    }
}
