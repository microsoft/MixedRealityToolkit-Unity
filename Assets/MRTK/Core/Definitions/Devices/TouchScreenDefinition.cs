// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class TouchScreenDefinition : BaseInputSourceDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        public TouchScreenDefinition() : base(Handedness.None) { }

        /// <inheritdoc />
        protected override MixedRealityInteractionMapping[] DefaultMappings => new[]
        {
            new MixedRealityInteractionMapping(0, "Touch Pointer Delta", AxisType.DualAxis, DeviceInputType.PointerPosition),
            new MixedRealityInteractionMapping(1, "Touch Pointer Position", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping(2, "Touch Press", AxisType.Digital, DeviceInputType.PointerClick),
        };
    }
}
