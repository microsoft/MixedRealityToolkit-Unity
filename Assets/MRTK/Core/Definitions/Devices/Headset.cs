// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    // TODO - currently not used, consider removing maybe?
    /// <summary>
    /// The headset definition defines the headset as defined by the SDK / Unity.
    /// </summary>
    public struct Headset
    {
        /// <summary>
        /// The ID assigned to the Headset
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The designated hand that the controller is managing, as defined by the SDK / Unity.
        /// </summary>
        public SDKType HeadsetSDKType { get; set; }

        /// <summary>
        /// Indicates whether or not the headset is currently providing position data.
        /// </summary>
        public bool IsPositionAvailable { get; set; }

        /// <summary>
        /// Outputs the current position of the headset, as defined by the SDK / Unity.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Indicates whether or not the headset is currently providing rotation data.
        /// </summary>
        public bool IsRotationAvailable { get; set; }

        /// <summary>
        /// Outputs the current rotation of the headset, as defined by the SDK / Unity.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Outputs the current state of the headset, whether it is tracked or not. As defined by the SDK / Unity.
        /// </summary>
        public TrackingState TrackingState { get; set; }

        /// <summary>
        /// Indicates whether or not the headset display is opaque. As defined by the SDK / Unity.
        /// </summary>
        public bool IsOpaque { get; set; }
    }
}