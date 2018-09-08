// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem
{
    /// <summary>
    /// Data struct for cursor state information for the Animated Cursor, which leverages the Unity animation system.
    /// This defines a modification to an Unity animation parameter, based on cursor state.
    /// </summary>
    [Serializable]
    public struct AnimatedCursorData
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the animation state for the cursor.</param>
        /// <param name="cursorState">The enum state for the cursor's animation.</param>
        /// <param name="parameter">The linked animation parameter to used for the cursor state.</param>
        public AnimatedCursorData(string name, CursorStateEnum cursorState, AnimatorParameter parameter)
        {
            this.name = name;
            this.cursorState = cursorState;
            this.parameter = parameter;
        }

        [SerializeField]
        [Tooltip("The name of this specific cursor state.")]
        private string name;

        /// <summary>
        /// The name of this specific cursor state.
        /// </summary>
        public string Name => name;

        [SerializeField]
        [Tooltip("The Cursor State for this specific animation.")]
        private CursorStateEnum cursorState;

        /// <summary>
        /// The Cursor State for this specific animation.
        /// </summary>
        public CursorStateEnum CursorState => cursorState;

        [SerializeField]
        [Tooltip("Animator parameter definition for this cursor state.")]
        private AnimatorParameter parameter;

        /// <summary>
        /// Animator parameter definition for this cursor state.
        /// </summary>
        public AnimatorParameter Parameter => parameter;
    }
}