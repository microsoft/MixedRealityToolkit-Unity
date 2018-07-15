// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.UX.Widgets
{
    /// <summary>
    /// An InteractiveWidget for fading in elements based on Interactive focus state
    /// </summary>
    [RequireComponent(typeof(FadeColors))]
    public class FadeInOnFocusWidget : InteractiveWidget
    {
        private FadeColors mFadeController;

        /// <summary>
        /// Set the Fade Controller
        /// </summary>
        private void Awake()
        {
            if (mFadeController == null)
            {
                mFadeController = GetComponent<FadeColors>();
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
        public override void SetState(ButtonStateEnum state)
        {
            base.SetState(state);

            if (state == ButtonStateEnum.FocusSelected || state == ButtonStateEnum.Focus || state == ButtonStateEnum.Press || state == ButtonStateEnum.PressSelected)
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