// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input
{
    /// <summary>
    /// A Windows Mixed Reality GGV hand instance.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.GGVHand,
        new[] { Handedness.Left, Handedness.Right, Handedness.None })]
    public class WindowsMixedRealityGGVHand : BaseWindowsMixedRealitySource
    {
        public WindowsMixedRealityGGVHand(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
        : base(trackingState, controllerHandedness, inputSource, interactions) { }

        /// <summary>
        /// The GGV hand default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right controllers.</remarks>
        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } = new[]
        {
            new MixedRealityInteractionMapping(0, "Select", AxisType.Digital, DeviceInputType.Select),
            new MixedRealityInteractionMapping(1, "Grip Pose", AxisType.SixDof, DeviceInputType.SpatialGrip),
        };
    }
}
