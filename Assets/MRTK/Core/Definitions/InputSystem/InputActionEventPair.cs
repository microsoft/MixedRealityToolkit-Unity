// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Data class that maps <see cref="MixedRealityInputAction"/>s to <see cref="UnityEvent"/>s wired up in the inspector.
    /// </summary>
    [Serializable]
    public struct InputActionEventPair
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public InputActionEventPair(MixedRealityInputAction inputAction, UnityEvent unityEvent)
        {
            this.inputAction = inputAction;
            this.unityEvent = unityEvent;
        }

        [SerializeField]
        [Tooltip("The MixedRealityInputAction to listen for to invoke the UnityEvent.")]
        private MixedRealityInputAction inputAction;

        /// <summary>
        /// The <see cref="MixedRealityInputAction"/> to listen for to invoke the <see cref="UnityEvent"/>.
        /// </summary>
        public MixedRealityInputAction InputAction => inputAction;

        [SerializeField]
        [Tooltip("The UnityEvent to invoke when MixedRealityInputAction is raised.")]
        private UnityEvent unityEvent;

        /// <summary>
        /// The <see cref="UnityEvent"/> to invoke when <see cref="MixedRealityInputAction"/> is raised.
        /// </summary>
        public UnityEvent UnityEvent => unityEvent;
    }
}