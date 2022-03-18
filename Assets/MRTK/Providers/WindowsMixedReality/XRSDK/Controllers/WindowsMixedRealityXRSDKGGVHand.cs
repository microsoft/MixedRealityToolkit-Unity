// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.XRSDK.WindowsMixedReality
{
    /// <summary>
    /// A Windows Mixed Reality GGV (Gaze, Gesture, and Voice) hand instance for XR SDK.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.GGVHand,
        new[] { Handedness.Left, Handedness.Right, Handedness.None },
        supportedUnityXRPipelines: SupportedUnityXRPipelines.XRSDK)]
    public class WindowsMixedRealityXRSDKGGVHand : BaseWindowsMixedRealityXRSDKSource
    {
        public WindowsMixedRealityXRSDKGGVHand(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, new SimpleHandDefinition(controllerHandedness))
        { }
    }
}
