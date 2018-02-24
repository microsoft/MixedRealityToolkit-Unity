// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.UX.Buttons.Enums;
using System;
using UnityEngine;

namespace MixedRealityToolkit.UX.Buttons.Utilities
{
    [Serializable]
    public struct AnimatorControllerAction
    {
        public ButtonStateEnum ButtonState;
        public string ParamName;
        public AnimatorControllerParameterType ParamType;
        public bool BoolValue;
        public int IntValue;
        public float FloatValue;
        public bool InvalidParam;
    }
}
