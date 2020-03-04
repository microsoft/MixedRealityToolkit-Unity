// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Interface for defining a bounds target used by <see cref="AppBar"/>
    /// Implement this interface to enable AppBar controlled modifications
    /// </summary>
    internal interface IBoundsTargetProvider
    {
        /// <summary>
        /// Indicates if the provider is currently active
        /// </summary>
        bool Active
        {
            get;
            set;
        }

        /// <summary>
        /// The object that this component is targeting
        /// </summary>
        GameObject Target
        {
            get;
            set;
        }

        /// <summary>
        /// The collider reference tracking the bounds utilized by this component during runtime
        /// </summary>
        BoxCollider TargetBounds
        {
            get;
        }
    }
}
