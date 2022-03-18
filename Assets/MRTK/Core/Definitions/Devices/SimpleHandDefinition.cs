// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class SimpleHandDefinition : BaseInputSourceDefinition
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handedness">The handedness that this definition instance represents.</param>
        public SimpleHandDefinition(Handedness handedness) : base(handedness)
        { }

        /// <inheritdoc />
        protected override MixedRealityInputActionMapping[] DefaultMappings => new[]
        {
            new MixedRealityInputActionMapping("Select", AxisType.Digital, DeviceInputType.Select),
            new MixedRealityInputActionMapping("Grip Pose", AxisType.SixDof, DeviceInputType.SpatialGrip),
        };
    }
}
