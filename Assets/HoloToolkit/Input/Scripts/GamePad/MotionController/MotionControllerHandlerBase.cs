// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace HoloToolkit.Unity.InputModule
{

    public class MotionControllerHandlerBase : GamePadHandlerBase, IMotionControllerHandler
    {
        public virtual void OnMotionControllerInputUpdate(MotionControllerEventData eventData)
        {
        }
    }
}
