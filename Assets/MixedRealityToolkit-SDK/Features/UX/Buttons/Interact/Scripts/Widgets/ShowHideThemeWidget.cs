// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Blend;
using Interact.Themes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Widgets
{
    /// <summary>
    /// a widget to show or hide objects based on a theme and state
    /// </summary>
    public class ShowHideThemeWidget : InteractiveThemeWidget
    {

        public GameObject[] ObjectList;

        private BoolInteractiveTheme mTheme;

        public override void SetTheme()
        {
            mTheme = InteractiveThemeManager.GetBoolTheme(ThemeTag);
        }

        protected override bool HasTheme()
        {
            return mTheme != null;
        }

        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            if (mTheme != null)
            {
                for (int i = 0; i < ObjectList.Length; i++)
                {
                    BlendFade transition = ObjectList[i].GetComponent<BlendFade>();
                    if (transition != null)
                    {
                        transition.TargetValue = mTheme.GetThemeValue(state) ? 1 : 0;
                        transition.Play();
                    }
                    else
                    {
                        ObjectList[i].SetActive(mTheme.GetThemeValue(state));
                    }
                }
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
