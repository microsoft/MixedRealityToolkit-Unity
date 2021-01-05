// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class GGVHandControllerDefinition : BaseControllerDefinition
    {
        public GGVHandControllerDefinition(Handedness handedness) : base(handedness) { }

        /// <inheritdoc />
        protected override MixedRealityInteractionMapping[] DefaultMappings => new[]
        {
            new MixedRealityInteractionMapping(0, "Select", AxisType.Digital, DeviceInputType.Select),
            new MixedRealityInteractionMapping(1, "Grip Pose", AxisType.SixDof, DeviceInputType.SpatialGrip),
        };
    }
}
