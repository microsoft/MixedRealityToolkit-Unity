// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Examples.Prototyping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// Updates the position of an element based on the Interactive focus state
    /// </summary>
    [RequireComponent(typeof(MoveToPosition))]
    public class MoveObjectWidget : InteractiveWidget
    {
        [Tooltip("Starting default position")]
        public Vector3 DefaultPosition;

        [Tooltip("Target position on focus")]
        public Vector3 FocusPosition;

        private MoveToPosition mMoveToPosition;

        /// <summary>
        /// Get the Move to Position component
        /// </summary>
        void Awake()
        {
            mMoveToPosition = GetComponent<MoveToPosition>();

            if (mMoveToPosition == null)
            {
                Debug.LogError("MoveToPosition:mMoveToPosition is not set in MoveObjectWidget!");
                Destroy(this);
            }
        }

        /// <summary>
        /// Set or animate the position
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            if (mMoveToPosition != null)
            {
                if (state == Interactive.ButtonStateEnum.FocusSelected || state == Interactive.ButtonStateEnum.Focus || state == Interactive.ButtonStateEnum.Press || state == Interactive.ButtonStateEnum.PressSelected)
                {
                    mMoveToPosition.TargetValue = FocusPosition;
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
