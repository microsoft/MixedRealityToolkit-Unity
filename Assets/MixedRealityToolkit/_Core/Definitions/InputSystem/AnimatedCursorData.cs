// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem
{
    /// <summary>
    /// Data struct for cursor state information for the Animated Cursor, which leverages the Unity animation system.
    /// This defines a modification to an Unity animation parameter, based on cursor state.
    /// </summary>
    [Serializable]
    public struct AnimatedCursorData
    {
        public string Name;
        public CursorStateEnum CursorState;
        public AnimatorParameter Parameter;
    }
}