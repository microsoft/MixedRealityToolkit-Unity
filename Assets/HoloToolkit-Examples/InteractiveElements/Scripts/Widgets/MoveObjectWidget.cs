// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Examples.Prototyping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HoloToolkit.Examples.InteractiveElements
{

    [RequireComponent(typeof(MoveToPosition))]
    public class MoveObjectWidget : InteractiveWidget
    {

        public Vector3 DefaultPosition;
        public Vector3 FocusPosition;
        private MoveToPosition mMoveToPosition;

        // Use this for initialization
        void Awake()
        {
            mMoveToPosition = GetComponent<MoveToPosition>();

            if (mMoveToPosition == null)
            {
                Debug.LogError("MoveToPosition:mMoveToPosition is not set in MoveObjectWidget!");
                Destroy(this);
            }
        }

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
