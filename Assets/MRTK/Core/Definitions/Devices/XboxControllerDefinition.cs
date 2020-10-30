// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Defines the base interactions and data that an controller can provide.
    /// </summary>
    public class XboxControllerDefinition : BaseControllerDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public XboxControllerDefinition(
            IMixedRealityInputSource source) : base(source, Handedness.None)
        { }

        // todo
    }   
}
