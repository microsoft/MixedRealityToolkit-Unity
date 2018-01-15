// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Examples.Prototyping;
using MixedRealityToolkit.Examples.UX.Themes;
using MixedRealityToolkit.Examples.UX.Widgets;
using UnityEngine;

namespace MixedRealityToolkit.Examples.UX.Controls
{
    /// <summary>
    /// updates the button label color, position, and text based on Interactive state
    /// </summary>
    public class ButtonThemeWidgetLabel : InteractiveThemeWidget
    {
        [Tooltip("LabelTheme for switching the default and selected labels")]
        public LabelTheme ButtonLabels;

        [Tooltip("tag for the color theme")]
        public string ColorThemeTag = "defaultColor";

        [Tooltip("tag for the position theme")]
        public string PositionThemeTag = "defaultPosition";

        [Tooltip("position animation component: optional")]
        public MoveToPosition MovePosition;

        // themes
        private ColorInteractiveTheme mColorTheme;
        private Vector3InteractiveTheme mPositionTheme;

        // the TextMesh
        private TextMesh mText;

        private string mCheckColorThemeTag = "";
        private string mCheckPositionThemeTag = "";
        
        /// <summary>
        /// Get the TextMesh and position animation component
        /// </summary>
        private void Awake()
        {
            if (MovePosition == null)
            {
                MovePosition = GetComponent<MoveToPosition>();
            }

            mText = this.gameObject.GetComponent<TextMesh>();
        }

        private void Start()
        {
            SetTheme();
            RefreshIfNeeded();
        }

        public override void SetTheme()
        {
            if (ColorThemeTag != "")
            {
                mColorTheme = GetColorTheme(ColorThemeTag);
                mCheckColorThemeTag = ColorThemeTag;
            }

            if (PositionThemeTag != "")
            {
                mPositionTheme = GetVector3Theme(PositionThemeTag);
                mCheckPositionThemeTag = PositionThemeTag;
            }
        }

        /// <summary>
        /// Set colors, position and label text
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            
            if (mText != null)
            {
                if (mColorTheme != null)
                {
                    mText.color = mColorTheme.GetThemeValue(state);
                }

                if (ButtonLabels != null)
                {
                    if (InteractiveHost.IsSelected)
                    {
                        if (ButtonLabels.Selected != "")
                        {
                            mText.text = ButtonLabels.Selected;
                        }
                        else
                        {
                            mText.text = ButtonLabels.Default;
                        }
                    }
                    else
                    {
                        mText.text = ButtonLabels.Default;
                    }
                }
            }

            if (mPositionTheme != null)
            {
                if (MovePosition != null)
                {
                    MovePosition.TargetValue = mPositionTheme.GetThemeValue(state);
                    MovePosition.StartRunning();
                }
                else
                {
                    transform.localPosition = mPositionTheme.GetThemeValue(state);
                }
            }
        }

        private void Update()
        {
            if (!mCheckPositionThemeTag.Equals(PositionThemeTag) || !mCheckColorThemeTag.Equals(ColorThemeTag))
            {
                SetTheme();
                RefreshIfNeeded();
            }
        }
    }
}
