// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection
{
    /// <summary>
    /// Describes how a marker is expected to be moving while it's detected.
    /// </summary>
    public enum MarkerPositionBehavior
    {
        /// <summary>
        /// The marker is expected to be completely stationary (for example, resting on a table).
        /// </summary>
        Stationary,

        /// <summary>
        /// The marker is expected to have some movement (for example, a handheld marker).
        /// </summary>
        Moving
    }
}