// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities
{
    /// <summary>
    /// The ExperienceScale identifies the environment for which the experience is designed.
    /// </summary>
    [System.Serializable]
    public enum ExperienceScale
    {
        /// <summary>
        /// An experience which utilizes only the headset orientantion and is gravity aligned.
        /// </summary>
        OrientationOnly = 0,
        /// <summary>
        /// An experience designed for seated use.
        /// </summary>
        Seated,
        /// <summary>
        /// An experience designed for stationary standing use.
        /// </summary>
        Standing,
        /// <summary>
        /// An experience designed to support movement thoughtout a room.
        /// </summary>
        Room,
        /// <summary>
        /// An experience designed to utilize and move through the physical world.
        /// </summary>
        World
    }
}