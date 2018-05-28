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
        /// <summary>
        /// Name of the Cursor
        /// </summary>
        public string Name;

        /// <summary>
        /// The current state of the cursor
        /// E.G. Hover, interact, etc
        /// </summary>
        public CursorStateEnum CursorState;

        /// <summary>
        /// Animator parameter definition to map to for the cursor
        /// </summary>
        public AnimatorParameter Parameter;
    }
}