// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Interact.Themes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Interact.Widgets
{
    /// <summary>
    /// An Interactive Theme Widget for swapping textures based on interactive state
    /// </summary>
    public class StringThemeWidget : InteractiveThemeWidget
    {

        [Tooltip("The target Text or TextMesh object : optional, leave blank for self")]
        public GameObject Target;

        /// <summary>
        /// The theme with the texture states
        /// </summary>
        protected StringInteractiveTheme mStringTheme;
        
        private TextMesh mTextMesh;
        private Text mText;

        void Awake()
        {
            // set the target
            if (Target == null)
            {
                Target = this.gameObject;
            }

            mText = Target.GetComponent<Text>();
            mTextMesh = Target.GetComponent<TextMesh>();

            if (mTextMesh == null && mText == null)
            {
                Debug.LogError("Textmesh or Text is not available to StringThemeWidget!");
                Destroy(this);
            }
        }

        public override void SetTheme()
        {
            mStringTheme = InteractiveThemeManager.GetStringTheme(ThemeTag);
        }

        protected override bool HasTheme()
        {
            return mStringTheme != null;
        }

        /// <summary>
        /// From InteractiveWidget
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            if (mStringTheme != null)
            {
                if (mTextMesh != null)
                {
                    mTextMesh.text = mStringTheme.GetThemeValue(state);
                }
                else if (mText != null)
                {
                    mText.text = mStringTheme.GetThemeValue(state);
                }
            }
            else
            {
                IsInited = false;
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
