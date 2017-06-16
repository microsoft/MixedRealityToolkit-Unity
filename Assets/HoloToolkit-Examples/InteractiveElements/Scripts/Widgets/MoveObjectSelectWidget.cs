// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Examples.Prototyping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// Changes the position of an element based on the Interactive selected state
    /// </summary>
    [RequireComponent(typeof(MoveToPosition))]
    public class MoveObjectSelectWidget : InteractiveWidget
    {
        [Tooltip("Default position for the default state")]
        public Vector3 DefaultPosition;

        [Tooltip("Position for the selected state")]
        public Vector3 SelectPosition;

        private MoveToPosition mMoveToPosition;

        /// <summary>
        /// Get the Move to Position component
        /// </summary>
        void Awake()
        {
            mMoveToPosition = GetComponent<MoveToPosition>();
        }

        /// <summary>
        /// Animate the position
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            if (mMoveToPosition != null)
            {
                if (state == Interactive.ButtonStateEnum.FocusSelected || state == Interactive.ButtonStateEnum.Selected || state == Interactive.ButtonStateEnum.PressSelected || state == Interactive.ButtonStateEnum.DisabledSelected)
                {
                    mMoveToPosition.TargetValue = SelectPosition;
                    mMoveToPosition.StartRunning();
                }
                else
                {
                    mMoveToPosition.TargetValue = DefaultPosition;
                    mMoveToPosition.StartRunning();
                }
            }
        }
    }
}
