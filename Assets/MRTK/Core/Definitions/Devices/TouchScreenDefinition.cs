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
        /// Constructor.
        /// </summary>
        public TouchScreenDefinition() : base(Handedness.None)
        { }

        /// <inheritdoc />
        protected override MixedRealityInputActionMapping[] DefaultMappings => new[]
        {
            new MixedRealityInputActionMapping("Touch Pointer Delta", AxisType.DualAxis, DeviceInputType.PointerPosition),
            new MixedRealityInputActionMapping("Touch Pointer Position", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInputActionMapping("Touch Press", AxisType.Digital, DeviceInputType.PointerClick),
        };
    }
}
