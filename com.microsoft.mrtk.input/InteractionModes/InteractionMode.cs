// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A structure representing an interactor's mode or state.
    /// </summary>
    [Serializable]
    public struct InteractionMode
    {
        [SerializeField]
        [Tooltip("The name of the interaction mode.")]
        private string name;

        /// <summary>
        /// The name of the interaction mode.
        /// </summary>
        public string Name => name;

        [SerializeField]
        [FormerlySerializedAs("id")]
        [Tooltip("The priority of the interaction mode.")]
        private int priority;

        /// <summary>
        /// The priority of the interaction mode.
        /// </summary>
        public int Priority => priority;

        /// <summary>
        /// Returns a string that represents this <see cref="InteractionMode"/>.
        /// </summary>
        /// <Returns>
        /// The `name` field for this <see cref="InteractionMode"/>.
        /// </Returns>
        public override string ToString()
        {
            return name;
        }
    }
}
