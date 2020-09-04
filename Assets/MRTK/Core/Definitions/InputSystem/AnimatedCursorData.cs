// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [Serializable]
    public class AnimatedCursorStateData : AnimatedCursorData<CursorStateEnum> { }

    [Serializable]
    public class AnimatedCursorContextData : AnimatedCursorData<CursorContextEnum> { }

    /// <summary>
    /// Data struct for cursor state information for the Animated Cursor, which leverages the Unity animation system.
    /// This defines a modification to an Unity animation parameter, based on cursor state.
    /// </summary>
    [Serializable]
    public class AnimatedCursorData<T>
    {

        [SerializeField]
        [Tooltip("The name of this specific cursor state.")]
        protected string name;

        /// <summary>
        /// The name of this specific cursor state.
        /// </summary>
        public string Name => name;

        [SerializeField]
        [Tooltip("The Cursor State for this specific animation.")]
        protected T cursorState;

        /// <summary>
        /// The Cursor State for this specific animation.
        /// </summary>
        public T CursorState => cursorState;

        [SerializeField]
        [Tooltip("Animator parameter definition for this cursor state.")]
        protected AnimatorParameter parameter;

        /// <summary>
        /// Animator parameter definition for this cursor state.
        /// </summary>
        public AnimatorParameter Parameter => parameter;
    }
}