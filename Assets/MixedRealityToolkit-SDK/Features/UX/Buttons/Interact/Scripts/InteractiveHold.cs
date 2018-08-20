// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Interact
{
    public class InteractiveHold : Interactive
    {
        public bool CaptureHold = true;
        public float PressTime = 0.5f;
        public UnityEvent OnHold;

        protected override void Update()
        {
            base.Update();
            HoldTime = PressTime;
            DetectHold = CaptureHold;
        }

        public override void OnInputHold()
        {
            base.OnInputHold();
            OnHold.Invoke();
        }
    }
}
