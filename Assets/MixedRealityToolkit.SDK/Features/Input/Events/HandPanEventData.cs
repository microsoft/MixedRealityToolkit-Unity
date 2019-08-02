// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Information associated with a particular hand pan event.
    /// </summary>
    public class HandPanEventData
    {
        /// <summary>
        /// Hand pan delta
        /// </summary>
        public Vector2 PanDelta { get; set; }
    }
}
