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
        [Tooltip("The name of the interaction mode.")]
        public string name;

        [FormerlySerializedAs("id")]
        [Tooltip("The priority of the interaction mode.")]
        public int priority;

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
