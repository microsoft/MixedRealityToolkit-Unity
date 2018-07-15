// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.UX.Widgets
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
        public override void SetState(ButtonStateEnum state)
        {
            base.SetState(state);

            if (mMoveToPosition != null)
            {
                if (state == ButtonStateEnum.FocusSelected || state == ButtonStateEnum.Selected || state == ButtonStateEnum.PressSelected || state == ButtonStateEnum.DisabledSelected)
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