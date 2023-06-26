// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A static class containing types related to hand shapes.
    /// </summary>
    public static class HandshapeTypes

    {
        /// <summary>
        /// Supported hand gestures.
        /// </summary>
        public enum HandshapeId
        {
            /// <summary>
            /// Unspecified hand shape
            /// </summary>
            None = 0,

            /// <summary>
            /// Flat hand with fingers spread out
            /// </summary>
            Flat,

            /// <summary>
            /// Relaxed hand pose in the 'ready' position
            /// </summary>
            Open,

            /// <summary>
            /// Index finger and Thumb touching, grab point does not move
            /// </summary>
            Pinch,

            /// <summary>
            /// Index finger and Thumb touching, wrist does not move
            /// </summary>
            PinchSteadyWrist,

            /// <summary>
            /// Index finger stretched out
            /// </summary>
            Poke,

            /// <summary>
            /// Grab with whole hand, fist shape
            /// </summary>
            Grab,

            /// <summary>
            /// OK sign
            /// </summary>
            ThumbsUp,

            /// <summary>
            /// Victory sign
            /// </summary>
            Victory,

            /// <summary>
            /// Relaxed hand pose, grab point does not move
            /// </summary>
            OpenSteadyGrabPoint,

            /// <summary>
            /// Hand facing upwards, Index and Thumb stretched out to start a teleport
            /// </summary>
            TeleportStart,

            /// <summary>
            /// Hand facing upwards, Index curled in to finish a teleport
            /// </summary>
            TeleportEnd,
        }
    }
}
