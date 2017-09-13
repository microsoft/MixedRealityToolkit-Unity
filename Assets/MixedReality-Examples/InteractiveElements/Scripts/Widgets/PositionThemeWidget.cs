// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using HoloToolkit.Examples.Prototyping;
using System;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// updates the button position based on the button theme
    /// </summary>
    public class PositionThemeWidget : InteractiveThemeWidget
    {
        [Tooltip("A tag for finding the theme in the scene")]
        public string ThemeTag = "defaultPosition";

        [Tooltip("Move to Position, a component for animating position")]
        public MoveToPosition MovePositionTweener;

        private Vector3InteractiveTheme mPositionTheme;

        private string mCheckThemeTag = "";

        /// <summary>
        /// Get Move to Position
        /// </summary>
        private void Awake()
        {
            if (MovePositionTweener == null)
            {
                MovePositionTweener = GetComponent<MoveToPosition>();
            }
        }

        /// <summary>
        /// Get the theme
        /// </summary>
        private void Start()
        {
            if (mPositionTheme == null)
            {
                SetTheme();
            }

            RefreshIfNeeded();
        }

        public override void SetTheme()
        {
            mPositionTheme = GetVector3Theme(ThemeTag);
            mCheckThemeTag = ThemeTag;
        }

        /// <summary>
        /// Set the position
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            
            if (mPositionTheme != null)
            {
                if (MovePositionTweener != null)
                {
                    MovePositionTweener.TargetValue = mPositionTheme.GetThemeValue(state);
                    MovePositionTweener.StartRunning();
                }
                else
                {
                    transform.localPosition = mPositionTheme.GetThemeValue(state);
                }
            }
        }

        private void Update()
        {
            if (!mCheckThemeTag.Equals(ThemeTag))
            {
                SetTheme();
                RefreshIfNeeded();
            }
        }
    }
}
