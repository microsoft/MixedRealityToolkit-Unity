// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input
{
    /// <summary>
    /// A Windows Mixed Reality GGV (Gaze, Gesture, and Voice) hand instance.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.GGVHand,
        new[] { Handedness.Left, Handedness.Right, Handedness.None })]
    public class WindowsMixedRealityGGVHand : BaseWindowsMixedRealitySource
    {
        public WindowsMixedRealityGGVHand(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, new SimpleHandDefinition(controllerHandedness))
        { }
    }
}
