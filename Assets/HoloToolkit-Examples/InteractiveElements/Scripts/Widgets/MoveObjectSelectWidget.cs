// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Examples.Prototyping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{

    [RequireComponent(typeof(MoveToPosition))]
    public class MoveObjectSelectWidget : InteractiveWidget
    {

        public Vector3 DefaultPosition;
        public Vector3 SelectPosition;
        private MoveToPosition mMoveToPosition;

        // Use this for initialization
        void Awake()
        {
            mMoveToPosition = GetComponent<MoveToPosition>();
        }

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
