// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Interact.Themes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Widgets
{
    /// <summary>
    /// An Interactive Theme Widget for playing sound effects based on interactive state
    /// </summary>
    public class AudioThemeWidget : InteractiveThemeWidget
    {

        [Tooltip("The target object with the material to swap textures on : optional, leave blank for self")]
        public GameObject Target;
        
        /// <summary>
        /// The theme with the texture states
        /// </summary>
        protected AudioInteractiveTheme mAudioTheme;
        
        void Awake()
        {
            // set the target
            if (Target == null)
            {
                Target = this.gameObject;
            }
        }

        public override void SetTheme()
        {
            mAudioTheme = InteractiveThemeManager.GetAudioTheme(ThemeTag);
        }

        protected override bool HasTheme()
        {
            return mAudioTheme != null;
        }

        /// <summary>
        /// From InteractiveWidget
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            if (mAudioTheme != null)
            {
                AudioClip clip = mAudioTheme.GetThemeValue(state);

                if (clip != null)
                {
                    mAudioTheme.PlayStateAudio(clip, gameObject);
                }
            }
        }

        /// <summary>
        /// Plays the Tap audio clip set by the theme
        /// </summary>
        public void PlayTap()
        {
            if (mAudioTheme != null)
            {
                mAudioTheme.PlayTap(gameObject);
            }
        }

		protected override void ApplyBlendValues(float percent)
        {
            //throw new System.NotImplementedException();
        }

        protected override void GetBlendValues(Interactive.ButtonStateEnum state)
        {
            //throw new System.NotImplementedException();
        }	
    }
}
