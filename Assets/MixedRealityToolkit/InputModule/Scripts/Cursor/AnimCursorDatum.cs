// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.Cursor
{
    /// <summary>
    /// Data struct for cursor state information for the Animated Cursor, which leverages the Unity animation system..
    /// This defines a modification to an Unity animation parameter, based on cursor state.
    /// </summary>
    [Serializable]
    public struct AnimCursorDatum
    {
        public string Name;
        public CursorStateEnum CursorState;
        public AnimatorParameter Parameter;
    }
}