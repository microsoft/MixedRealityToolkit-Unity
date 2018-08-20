// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace Interact.Widgets
{
    /// <summary>
    /// A widget for hooking up an Animator to on focus animations
    /// </summary>
    public class AnimatorFocusWidget : InteractiveWidget
    {
        [Tooltip("An animator with the different state animations")]
        public Animator AnimationTheme;

        [Tooltip("If we can expect 2 animations (default and focus) or 4 animations including (selected and focusSelected)")]
        public bool HasSelectedAnimations = false;

        private void Start()
        {
            // look for the Animator is it's not already assigned
            if (AnimationTheme == null)
            {
                if (InteractiveHost == null)
                {
                    AnimationTheme = GetComponentInParent<Animator>();
                }
                else
                {
                    AnimationTheme = InteractiveHost.GetComponentInParent<Animator>();
                }
            }
        }

        /// <summary>
        /// Set the Animator to play the correct animation
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            if (state == State || state == Interactive.ButtonStateEnum.Press || state == Interactive.ButtonStateEnum.PressSelected)
            {
                return;
            }

            base.SetState(state);

            // control which states are set to the animator to be default or focus
            Interactive.ButtonStateEnum stateString =  Interactive.ButtonStateEnum.Default;
            if (state == Interactive.ButtonStateEnum.FocusSelected || state == Interactive.ButtonStateEnum.Focus)
            {
                stateString = Interactive.ButtonStateEnum.Focus;
                if (InteractiveHost.IsSelected && HasSelectedAnimations)
                {
                    stateString = Interactive.ButtonStateEnum.FocusSelected;
                }
            }
            else
            {
                if (InteractiveHost.IsSelected && HasSelectedAnimations)
                {
                    stateString = Interactive.ButtonStateEnum.Selected;
                }
            }

            if (AnimationTheme != null)
            {
                AnimationTheme.SetTrigger(Animator.StringToHash(stateString.ToString()));

            }
            else
            {
                IsInited = false;
            }
        }
    }
}
