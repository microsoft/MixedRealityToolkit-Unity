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
    public class BaseControllerDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        public BaseControllerDefinition(IMixedRealityInputSource source, Handedness handedness)
        {
            inputSource = source;
            this.handedness = handedness;
        }

        /// <summary>
        /// 
        /// </summary>
        protected readonly IMixedRealityInputSource inputSource;
        // todo

        /// <summary>
        /// 
        /// </summary>
        protected readonly Handedness handedness;
        // todo

        /// <summary>
        /// 
        /// </summary>
        public virtual MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Select", AxisType.Digital, DeviceInputType.Select),
        };

        // todo

    }
}