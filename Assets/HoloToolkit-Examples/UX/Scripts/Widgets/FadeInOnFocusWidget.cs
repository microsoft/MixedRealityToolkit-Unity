// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Examples.Prototyping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// An InteractiveWidget for fading in elements based on Interactive focus state
    /// </summary>
    [RequireComponent(typeof(Prototyping.FadeColors))]
    public class FadeInOnFocusWidget : InteractiveWidget
    {
        private Prototyping.FadeColors mFadeController;

        /// <summary>
        /// Set the Fade Controller
        /// </summary>
        private void Awake()
        {
            if (mFadeController == null)
            {
                mFadeController = GetComponent<Prototyping.FadeColors>();
            }

            if (mFadeController == null)
            {
                Debug.LogError("FadeMaterialColor:FadeController is not set in FadeInOnFocusWidget!");
                Destroy(this);
            }
        }

        /// <summary>
        /// Fade in or out based on focus
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            if (state == Interactive.ButtonStateEnum.FocusSelected || state == Interactive.ButtonStateEnum.Focus || state == Interactive.ButtonStateEnum.Press || state == Interactive.ButtonStateEnum.PressSelected)
            {
                mFadeController.FadeIn();
            }
            else
            {
                mFadeController.FadeOut();
            }
        }
    }
}
