// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace Interact.Widgets
{
    /// <summary>
    /// A widget for hooking up an Animator to Interactive.ButtonStateEnum states
    /// </summary>
    public class AnimatorWidget : InteractiveWidget
    {
        [Tooltip("An animator with the different state animations")]
        public Animator AnimationTheme;
        
        private void Start()
        {
            // look for the Animator is it's not already assigned
            if(AnimationTheme == null)
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
            if (state == State)
            {
                return;
            }

            base.SetState(state);

            if (AnimationTheme != null)
            {
                AnimationTheme.SetTrigger(Animator.StringToHash(state.ToString()));
                
            }
            else
            {
                IsInited = false;
            }
        }
    }
}
