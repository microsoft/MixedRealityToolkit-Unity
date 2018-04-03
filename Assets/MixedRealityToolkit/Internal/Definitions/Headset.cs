// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Internal.Definitions
{
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
        /// Outputs the current position of the headset, as defined by the SDK / Unity.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Outputs the current rotation of the headset, as defined by the SDK / Unity.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Outputs the current state of the headset, whether it is tracked or not. As defined by the SDK / Unity.
        /// </summary>
        public ControllerState ControllerState { get; set; }
    }
}